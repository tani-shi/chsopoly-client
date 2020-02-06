using System.Collections;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.GameScene;
using Chsopoly.Libs;
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

            GameSceneManager.Instance.ChangeScene (_initialScene);
        }
    }
}