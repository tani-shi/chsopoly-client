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

        void Start ()
        {
            Application.targetFrameRate = FrameRate;
            Screen.sleepTimeout = UnityEngine.SleepTimeout.SystemSetting;

            Addressables.InitializeAsync ().Completed += (result) => {
                GameSceneManager.Instance.ChangeScene (GameSceneType.Title);
            };
        }
    }
}