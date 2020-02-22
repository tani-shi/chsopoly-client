using System;
using System.Collections;
using Chsopoly.Libs;
using Chsopoly.Libs.Extensions;
using Gs2.Core;
using Gs2.Core.Exception;
using Gs2.Unity;
using Gs2.Unity.Gs2Account.Result;
using Gs2.Unity.Gs2Gateway.Result;
using Gs2.Unity.Util;
using UnityEngine;

namespace Chsopoly.BaseSystem.Gs2
{
    public class Gs2Manager : SingletonMonoBehaviour<Gs2Manager>
    {
        public event Action<Gs2Exception> onError;

        private Profile _profile = null;
        private Client _client = null;

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

            callback.Invoke (result1);
        }
    }
}