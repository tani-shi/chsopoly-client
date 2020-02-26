using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Chsopoly.Libs;
using Chsopoly.Libs.Extensions;
using Google.Protobuf;
using Gs2.Core;
using Gs2.Core.Exception;
using Gs2.Core.Model;
using Gs2.Gs2Matchmaking.Model;
using Gs2.Gs2Realtime.Message;
using Gs2.Unity;
using Gs2.Unity.Gs2Account.Result;
using Gs2.Unity.Gs2Gateway.Result;
using Gs2.Unity.Gs2Matchmaking.Model;
using Gs2.Unity.Gs2Matchmaking.Result;
using Gs2.Unity.Gs2Realtime;
using Gs2.Unity.Gs2Realtime.Model;
using Gs2.Unity.Gs2Realtime.Result;
using Gs2.Unity.Util;
using LitJson;
using UnityEngine;

namespace Chsopoly.BaseSystem.Gs2
{
    public class Gs2Manager : SingletonMonoBehaviour<Gs2Manager>
    {
        public event Action<Gs2Exception> onError;
        public event Action<string> onJoinMatchingPlayer;
        public event Action<string> onLeaveMatchingPlayer;
        public event Action<string> onCompleteMatching;
        public event Action<string> onCreateRealtimeRoom;
        public event Action<uint, IGs2PacketModel> onRelayRealtimeMessage;
        public event Action<uint, IGs2PacketModel> onJoinRealtimePlayer;
        public event Action<uint, IGs2PacketModel> onLeaveRealtimePlayer;
        public event Action<uint, IGs2PacketModel> onUpdateRealtimeProfile;
        public event OnCloseHandler onCloseRealtime;
        public event OnErrorHandler onErrorRealtime;
        public event OnGeneralErrorHandler onGeneralErrorRealtime;

        private Profile _profile = null;
        private global::Gs2.Unity.Client _client = null;
        private GameSession _gameSession = null;
        private RealtimeSession _realtimeSession = null;
        private Queue<NotificationMessage> _messageQueue = new Queue<NotificationMessage> ();

        void Update ()
        {
            while (_messageQueue.Count > 0)
            {
                PushNotificationMessage (_messageQueue.Dequeue ());
            }
        }

        public IEnumerator Initialize ()
        {
            Debug.Log ("Gs2-Initialize");

            _profile = new Profile (Gs2Settings.ClientId, Gs2Settings.ClientSecret, new Gs2BasicReopener ());
            _client = new global::Gs2.Unity.Client (_profile);

            AsyncResult<object> result = null;
            yield return _profile.Initialize (r => result = r);

            if (result.Error != null)
            {
                Debug.LogError (result.Error.Message);
                onError.SafeInvoke (result.Error);
                yield break;
            }

            _profile.Gs2Session.OnNotificationMessage += OnNotificationMessage;
        }

        public IEnumerator CreateAccount (Action<AsyncResult<EzCreateResult>> callback)
        {
            Debug.Log ("Gs2-CreateAccount");

            AsyncResult<EzCreateResult> result = null;
            yield return _client.Account.Create (r => result = r, Gs2Settings.AccountNamespaceName);

            if (result.Error != null)
            {
                Debug.LogError (result.Error.Message);
                onError.SafeInvoke (result.Error);
                yield break;
            }

            callback.SafeInvoke (result);
        }

        public IEnumerator LoginAccount (string accountId, string password, Action<AsyncResult<GameSession>> callback)
        {
            Debug.Log (string.Format ("Gs2-LoginAccount: accountId={0} password={1}", accountId, password));

            AsyncResult<GameSession> result1 = null;
            yield return _profile.Login (
                new Gs2AccountAuthenticator (
                    _profile.Gs2Session,
                    Gs2Settings.AccountNamespaceName,
                    Gs2Settings.AccountEncryptionKeyId,
                    accountId,
                    password),
                r =>
                {
                    result1 = r;
                });

            if (result1.Error != null)
            {
                Debug.LogError (result1.Error.Message);
                onError.SafeInvoke (result1.Error);
                yield break;
            }

            AsyncResult<EzSetUserIdResult> result2 = null;
            yield return _client.Gateway.SetUserId (
                r => { result2 = r; },
                result1.Result,
                Gs2Settings.GatewayNamespaceName,
                true
            );

            if (result2.Error != null)
            {
                Debug.LogError (result2.Error.Message);
                onError.SafeInvoke (result2.Error);
                yield break;
            }

            _gameSession = result1.Result;
            callback.Invoke (result1);
        }

        public IEnumerator CreateGathering (int capacity, Action<AsyncResult<EzCreateGatheringResult>> callback)
        {
            Debug.Log ("Gs2-CreateGathering: capacity=" + capacity);

            ValidateGameSession ();

            AsyncResult<EzCreateGatheringResult> result = null;
            yield return _client.Matchmaking.CreateGathering (
                r => { result = r; },
                _gameSession,
                Gs2Settings.MatchingNamespaceName,
                new EzPlayer
                {
                    RoleName = "default"
                },
                new List<EzCapacityOfRole>
                {
                    new EzCapacityOfRole ()
                    {
                        RoleName = "default", Capacity = capacity
                    },
                },
                new List<string> (),
                new List<EzAttributeRange> ()
            );

            if (result.Error != null)
            {
                Debug.LogError (result.Error.Message);
                onError.SafeInvoke (result.Error);
                yield break;
            }

            onJoinMatchingPlayer.SafeInvoke (_gameSession.AccessToken.userId);
            callback.SafeInvoke (result);
        }

        public IEnumerator JoinGathering (Action<AsyncResult<EzDoMatchmakingResult>> callback)
        {
            Debug.Log ("Gs2-JoinGathering");

            ValidateGameSession ();

            AsyncResult<EzDoMatchmakingResult> result = null;
            yield return _client.Matchmaking.DoMatchmaking (
                r => { result = r; },
                _gameSession,
                Gs2Settings.MatchingNamespaceName,
                new EzPlayer
                {
                    RoleName = "default"
                },
                null
            );

            if (result.Error != null)
            {
                Debug.LogError (result.Error.Message);
                onError.SafeInvoke (result.Error);
                yield break;
            }

            callback.Invoke (result);
        }

        public IEnumerator GetRoom (string roomName, Action<AsyncResult<EzGetRoomResult>> callback)
        {
            Debug.Log ("Gs2-GetRoom: roomName=" + roomName);

            ValidateGameSession ();

            AsyncResult<EzGetRoomResult> result = null;
            yield return _client.Realtime.GetRoom (
                r => { result = r; },
                Gs2Settings.RealtimeNamespaceName,
                roomName
            );

            if (result.Error != null)
            {
                onError.SafeInvoke (result.Error);
                yield break;
            }

            callback.SafeInvoke (result);
        }

        public IEnumerator ConnectRoom (string ipAddress, int port, string encryptionKey, byte[] profile, Action<AsyncResult<RealtimeSession>> callback)
        {
            Debug.Log (string.Format ("Gs2-ConnectRoom: ipAddress={0} port={1} encryptionKey={2}", ipAddress, port, encryptionKey));

            ValidateGameSession ();

            var session = new RelayRealtimeSession (
                _gameSession.AccessToken.token,
                ipAddress,
                port,
                encryptionKey,
                ByteString.CopyFrom (profile)
            );

            session.OnRelayMessage += message =>
            {
                onRelayRealtimeMessage.SafeInvoke (message.ConnectionId, ModelDeserializer.Deserialize (message.Data.ToByteArray ()));
            };
            session.OnJoinPlayer += player =>
            {
                onJoinRealtimePlayer.SafeInvoke (player.ConnectionId, ModelDeserializer.Deserialize (player.Profile.ToByteArray ()));
            };
            session.OnLeavePlayer += player =>
            {
                onLeaveRealtimePlayer.SafeInvoke (player.ConnectionId, ModelDeserializer.Deserialize (player.Profile.ToByteArray ()));
            };
            session.OnUpdateProfile += player =>
            {
                onUpdateRealtimeProfile.SafeInvoke (player.ConnectionId, ModelDeserializer.Deserialize (player.Profile.ToByteArray ()));
            };
            session.OnClose += onCloseRealtime;
            session.OnError += onErrorRealtime;
            session.OnGeneralError += onGeneralErrorRealtime;

            AsyncResult<bool> result = null;
            yield return session.Connect (this, r => result = r);

            if (!session.Connected)
            {
                Debug.LogError (result.Error.Message);
                onError.SafeInvoke (result.Error);
                yield break;
            }

            _realtimeSession = session;
            callback.SafeInvoke (new AsyncResult<RealtimeSession> (session, null));
        }

        private void OnNotificationMessage (NotificationMessage message)
        {
            _messageQueue.Enqueue (message);
        }

        private void PushNotificationMessage (NotificationMessage message)
        {
            Debug.Log ("Gs2-PushNotificationMessage: " + message.issuer + "\n" + message.payload);

            if (message.issuer.StartsWith ("Gs2Matchmaking:"))
            {
                if (message.issuer.EndsWith (":Join"))
                {
                    var notification = JsonMapper.ToObject<global::Gs2.Gs2Matchmaking.Model.JoinNotification> (message.payload);
                    onJoinMatchingPlayer.SafeInvoke (notification.joinUserId);
                }
                else if (message.issuer.EndsWith (":Leave"))
                {
                    var notification = JsonMapper.ToObject<global::Gs2.Gs2Matchmaking.Model.LeaveNotification> (message.payload);
                    onLeaveMatchingPlayer.SafeInvoke (notification.leaveUserId);
                }
                else if (message.issuer.EndsWith (":Complete"))
                {
                    var notification = JsonMapper.ToObject<global::Gs2.Gs2Matchmaking.Model.CompleteNotification> (message.payload);
                    onCompleteMatching.SafeInvoke (notification.gatheringName);
                }
            }

            if (message.issuer.StartsWith ("Gs2Realtime:"))
            {
                if (message.issuer.EndsWith (":Create"))
                {
                    var notification = JsonMapper.ToObject<global::Gs2.Gs2Realtime.Model.CreateNotification> (message.payload);
                    onCreateRealtimeRoom.SafeInvoke (notification.roomName);
                }
            }
        }

        private void ValidateGameSession ()
        {
            if (_gameSession == null)
            {
                throw new Exception ("Game session is null, do login first.");
            }
        }

        private void ValidateRealtimeSession ()
        {
            if (_realtimeSession == null)
            {
                throw new Exception ("Realtime session is null, do connect room first.");
            }
        }
    }
}