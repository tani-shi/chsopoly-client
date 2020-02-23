using System;
using System.Collections;
using System.Collections.Generic;
using Chsopoly.Libs;
using Chsopoly.Libs.Extensions;
using Gs2.Core;
using Gs2.Core.Exception;
using Gs2.Core.Model;
using Gs2.Gs2Matchmaking.Model;
using Gs2.Unity;
using Gs2.Unity.Gs2Account.Result;
using Gs2.Unity.Gs2Gateway.Result;
using Gs2.Unity.Gs2Matchmaking.Model;
using Gs2.Unity.Gs2Matchmaking.Result;
using Gs2.Unity.Util;
using LitJson;
using UnityEngine;

namespace Chsopoly.BaseSystem.Gs2
{
    public class Gs2Manager : SingletonMonoBehaviour<Gs2Manager>
    {
        public event Action<Gs2Exception> onError;
        public event Action<List<string>> onUpdateJoinedPlayerIds;
        public event Action<string> onJoinPlayer;
        public event Action<string> onLeavePlayer;
        public event Action<List<string>> onMatchingComplete;

        private Profile _profile = null;
        private Client _client = null;
        private GameSession _gameSession = null;
        private List<string> _joinedPlayerIds = new List<string> ();
        private EzGathering _gathering = null;
        private bool _completedMatching = false;

        public IEnumerator Initialize ()
        {
            _profile = new Profile (Gs2Settings.ClientId, Gs2Settings.ClientSecret, new Gs2BasicReopener ());
            _client = new Client (_profile);

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
            if (!ValidateSession ())
            {
                yield break;
            }

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

            _joinedPlayerIds.Clear ();
            _gathering = result.Result.Item;
            _joinedPlayerIds.Add (_gameSession.AccessToken.userId);

            onUpdateJoinedPlayerIds.SafeInvoke (_joinedPlayerIds);
            callback.SafeInvoke (result);
        }

        public IEnumerator JoinGathering (Action<AsyncResult<EzDoMatchmakingResult>> callback)
        {
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

            if (result.Result.Item != null)
            {
                _gathering = result.Result.Item;

                if (_completedMatching)
                {
                    onMatchingComplete.SafeInvoke (_joinedPlayerIds);
                }
            }

            callback.Invoke (result);
        }

        private void OnNotificationMessage (NotificationMessage message)
        {
            Debug.Log ("OnNotificationMessage: " + message.issuer + "\n" + message.payload);

            if (!message.issuer.StartsWith ("Gs2Matchmaking:")) return;

            if (message.issuer.EndsWith (":Join"))
            {
                var notification = JsonMapper.ToObject<JoinNotification> (message.payload);
                _joinedPlayerIds.Add (notification.joinUserId);
                onJoinPlayer.SafeInvoke (notification.joinUserId);
                onUpdateJoinedPlayerIds.Invoke (_joinedPlayerIds);
            }
            else if (message.issuer.EndsWith (":Leave"))
            {
                var notification = JsonMapper.ToObject<LeaveNotification> (message.payload);
                _joinedPlayerIds.Remove (notification.leaveUserId);
                onLeavePlayer.SafeInvoke (notification.leaveUserId);
                onUpdateJoinedPlayerIds.SafeInvoke (_joinedPlayerIds);
            }
            else if (message.issuer.EndsWith (":Complete"))
            {
                if (_gathering != null)
                {
                    onMatchingComplete.SafeInvoke (_joinedPlayerIds);
                }

                _completedMatching = true;
            }
        }

        private bool ValidateSession ()
        {
            if (_gameSession == null)
            {
                Debug.LogError ("Game session is null, do login first.");
                return false;
            }

            return true;
        }
    }
}