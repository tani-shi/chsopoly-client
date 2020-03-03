using System.Collections;
using Chsopoly.BaseSystem.Audio;
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
            yield return AudioManager.Instance.LoadAsync ();

            GameSceneManager.Instance.ChangeScene (GameSceneType.Title);
        }
    }
}