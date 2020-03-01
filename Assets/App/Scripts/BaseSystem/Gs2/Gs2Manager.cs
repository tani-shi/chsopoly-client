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
        private const string KeyJoinedGatheringName = "Gs2JoinedGatheringName";

        public event Action<Gs2Exception> onError;
        public event Action<string> onJoinMatchingPlayer;
        public event Action<string> onLeaveMatchingPlayer;
        public event Action<string> onCompleteMatching;
        public event Action<string> onCreateRealtimeRoom;
        public event Action<uint, Gs2PacketModel> onRelayRealtimeMessage;
        public event Action<uint, Gs2PacketModel> onJoinRealtimePlayer;
        public event Action<uint, Gs2PacketModel> onLeaveRealtimePlayer;
        public event Action<uint, Gs2PacketModel> onUpdateRealtimeProfile;
        public event Action<uint, string, bool> onCloseRealtime;
        public event Action<string> onErrorRealtime;
        public event Action<string> onGeneralErrorRealtime;

        private Profile _profile = null;
        private global::Gs2.Unity.Client _client = null;
        private GameSession _gameSession = null;
        private RelayRealtimeSession _realtimeSession = null;
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

            if (!CheckError (result))
            {
                yield break;
            }

            _profile.Gs2Session.OnNotificationMessage += OnNotificationMessage;
        }

        public IEnumerator CreateAccount (Action<AsyncResult<EzCreateResult>> callback)
        {
            Debug.Log ("Gs2-CreateAccount");

            AsyncResult<EzCreateResult> result = null;
            yield return _client.Account.Create (r => result = r, Gs2Settings.AccountNamespaceName);

            if (!CheckError (result))
            {
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

            if (!CheckError (result1))
            {
                yield break;
            }

            AsyncResult<EzSetUserIdResult> result2 = null;
            yield return _client.Gateway.SetUserId (
                r => { result2 = r; },
                result1.Result,
                Gs2Settings.GatewayNamespaceName,
                true
            );

            if (!CheckError (result2))
            {
                yield break;
            }

            _gameSession = result1.Result;

            var joinedGathering = PlayerPrefs.GetString (KeyJoinedGatheringName);
            if (!string.IsNullOrEmpty (joinedGathering))
            {
                yield return CancelGathering (joinedGathering, r =>
                {
                    PlayerPrefs.SetString (KeyJoinedGatheringName, string.Empty);
                });
            }

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
                Gs2Settings.MatchmakingNamespaceName,
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

            if (!CheckError (result))
            {
                yield break;
            }

            if (result.Result.Item != null)
            {
                PlayerPrefs.SetString (KeyJoinedGatheringName, result.Result.Item.GatheringId);
            }

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
                Gs2Settings.MatchmakingNamespaceName,
                new EzPlayer
                {
                    RoleName = "default"
                },
                null
            );

            if (!CheckError (result))
            {
                yield break;
            }

            if (result.Result.Item != null)
            {
                PlayerPrefs.SetString (KeyJoinedGatheringName, result.Result.Item.GatheringId);
            }

            callback.Invoke (result);
        }

        public void CancelGathering (string gatheringName)
        {
            StartCoroutine (CancelGathering (gatheringName, null));
        }

        public IEnumerator CancelGathering (string gatheringName, Action<AsyncResult<EzCancelMatchmakingResult>> callback)
        {
            Debug.Log ("Gs2-CancelGathering: gatheringName=" + gatheringName);

            ValidateGameSession ();

            AsyncResult<EzCancelMatchmakingResult> result = null;
            yield return _client.Matchmaking.CancelMatchmaking (
                r => { result = r; },
                _gameSession,
                Gs2Settings.MatchmakingNamespaceName,
                gatheringName
            );

            callback.Invoke (result);
        }

        public IEnumerator GetRoom (string roomName, Action<AsyncResult<EzGetRoomResult>> callback, int timeout = -1)
        {
            Debug.Log ("Gs2-GetRoom: roomName=" + roomName);

            ValidateGameSession ();

            while (timeout != 0)
            {
                AsyncResult<EzGetRoomResult> result = null;
                yield return _client.Realtime.GetRoom (
                    r => { result = r; },
                    Gs2Settings.RealtimeNamespaceName,
                    roomName
                );

                if (!CheckError (result))
                {
                    yield break;
                }

                if (!string.IsNullOrEmpty (result.Result.Item.IpAddress))
                {
                    callback.SafeInvoke (result);
                    yield break;
                }

                timeout--;

                yield return new WaitForSeconds (1f);
            }
        }

        public IEnumerator ConnectRoom (string ipAddress, int port, string encryptionKey, Gs2PacketModel profile, Action<AsyncResult<RealtimeSession>> callback)
        {
            Debug.Log (string.Format ("Gs2-ConnectRoom: ipAddress={0} port={1} encryptionKey={2} profile={3}", ipAddress, port, encryptionKey, JsonUtility.ToJson (profile)));

            ValidateGameSession ();

            var session = new RelayRealtimeSession (
                _gameSession.AccessToken.token,
                ipAddress,
                port,
                encryptionKey,
                profile.Serialize ()
            );

            session.OnRelayMessage += message =>
            {
                var model = ModelDeserializer.Deserialize (message.Data);
                Debug.Log (string.Format ("Gs2Realtime-OnRelayMessage: {0}:{1} {2}", model.GetType ().Name, message.ConnectionId, JsonUtility.ToJson (model)));
                onRelayRealtimeMessage.SafeInvoke (message.ConnectionId, model);
            };
            session.OnJoinPlayer += player =>
            {
                var model = ModelDeserializer.Deserialize (player.Profile);
                Debug.Log ("Gs2Realtime-OnJoinPlayer: " + player.ConnectionId + " " + JsonUtility.ToJson (model));
                onJoinRealtimePlayer.SafeInvoke (player.ConnectionId, model);
            };
            session.OnLeavePlayer += player =>
            {
                var model = ModelDeserializer.Deserialize (player.Profile);
                Debug.Log ("Gs2Realtime-OnLeavePlayer: " + player.ConnectionId + " " + JsonUtility.ToJson (model));
                onLeaveRealtimePlayer.SafeInvoke (player.ConnectionId, model);
            };
            session.OnUpdateProfile += player =>
            {
                var model = ModelDeserializer.Deserialize (player.Profile);
                Debug.Log ("Gs2Realtime-OnUpdateProfile: " + player.ConnectionId + " " + JsonUtility.ToJson (model));
                onUpdateRealtimeProfile.SafeInvoke (player.ConnectionId, model);
            };
            session.OnClose += args =>
            {
                Debug.Log ("Gs2Realtime-OnCloseRealtime: " + JsonUtility.ToJson (args));
                onCloseRealtime.SafeInvoke (args.Code, args.Reason, args.WasClean);
            };
            session.OnError += args =>
            {
                Debug.LogError ("Gs2Realtime-OnError: " + args.Message);
                onErrorRealtime.SafeInvoke (args.Message);
            };
            session.OnGeneralError += args =>
            {
                Debug.LogError ("Gs2Realtime-OnGeneralError: " + args.Message);
                onGeneralErrorRealtime.SafeInvoke (args.Message);
            };

            AsyncResult<bool> result = null;
            yield return session.Connect (this, r => result = r);

            if (!CheckError (result))
            {
                yield break;
            }

            _realtimeSession = session;
            callback.SafeInvoke (new AsyncResult<RealtimeSession> (session, null));
        }

        public void SendRelayMessage (Gs2PacketModel model)
        {
            StartCoroutine (SendRelayMessage (model, null));
        }

        private IEnumerator SendRelayMessage (Gs2PacketModel model, Action<AsyncResult<bool>> callback)
        {
            Debug.Log (string.Format ("Gs2-SendRelayMessage: {0} {1}", model.GetType ().Name, JsonUtility.ToJson (model)));

            ValidateGameSession ();
            ValidateRealtimeSession ();

            AsyncResult<bool> result = null;
            yield return _realtimeSession.Send (r => result = r, model.Serialize ());

            if (!CheckError (result))
            {
                yield break;
            }

            callback.SafeInvoke (result);
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

        private bool CheckError<T> (AsyncResult<T> result)
        {
            if (result.Error != null)
            {
                Debug.LogError (result.Error.GetType ().FullName + ":" + result.Error.Message);
                onError.SafeInvoke (result.Error);
                return false;
            }

            return true;
        }
    }
}