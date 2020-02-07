using System.Collections;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene;
using Chsopoly.Libs;
using Chsopoly.MasterData.Collection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Chsopoly.BaseSystem
{
    public class AppMain : SingletonMonoBehaviour<AppMain>
    {
        const int FrameRate = 30;

        [SerializeField] GameSceneType _initialScene = GameSceneType.Title;

        void Start ()
        {
            StartCoroutine (InitializeProc ());
        }

        IEnumerator InitializeProc ()
        {
            Application.targetFrameRate = FrameRate;
            Screen.sleepTimeout = UnityEngine.SleepTimeout.SystemSetting;

            yield return Addressables.InitializeAsync ();
            yield return MasterDataManager.Instance.LoadAsync (new MasterDataAccessorObjectCollection ());

            GameSceneManager.Instance.ChangeScene (_initialScene);
        }
    }
}