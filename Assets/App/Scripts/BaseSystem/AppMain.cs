using System.Collections;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.Gs2;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.BaseSystem.UserData;
using Chsopoly.GameScene;
using Chsopoly.Libs;
using Chsopoly.MasterData.Collection;
using Chsopoly.UserData.Entity;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Chsopoly.BaseSystem
{
    public class AppMain : SingletonMonoBehaviour<AppMain>
    {
        const int FrameRate = 30;

        void Start ()
        {
            StartCoroutine (InitializeProc ());
        }

        IEnumerator InitializeProc ()
        {
            Application.targetFrameRate = FrameRate;
            Screen.sleepTimeout = UnityEngine.SleepTimeout.SystemSetting;
            UserDataManager.Instance.Initialize ();

            yield return Addressables.InitializeAsync ();
            yield return MasterDataManager.Instance.LoadAsync (new MasterDataAccessorObjectCollection ());
            yield return Gs2Manager.Instance.Initialize ();

            var account = UserDataManager.Instance.Load<Account> ();
            if (account == null)
            {
                yield return Gs2Manager.Instance.CreateAccount (r =>
                {
                    account = new Account ();
                    account.Gs2AccountId = r.Result.Item.UserId;
                    account.Gs2Password = r.Result.Item.Password;
                    account.Gs2CreatedAt = r.Result.Item.CreatedAt;
                    UserDataManager.Instance.Save (account);
                });
            }

            GameSceneManager.Instance.ChangeScene (GameSceneType.Title);
        }
    }
}