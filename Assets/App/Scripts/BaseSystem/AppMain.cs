using System.Collections;
using Chsopoly.BaseSystem.Audio;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.Gs2;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.BaseSystem.Popup;
using Chsopoly.BaseSystem.UserData;
using Chsopoly.GameScene;
using Chsopoly.Libs;
using Chsopoly.MasterData.Collection;
using Chsopoly.Popup;
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
            UserDataManager.Instance.Load ();

            yield return Addressables.InitializeAsync ();
            yield return MasterDataManager.Instance.LoadAsync (new MasterDataAccessorObjectCollection ());
            yield return AudioManager.Instance.LoadAsync ();
            yield return GameSceneManager.Instance.Initialize ();

            GameSceneManager.Instance.ChangeScene (GameSceneType.Title);

            Application.logMessageReceived += (condition, stackTrace, type) =>
            {
                if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
                {
                    OnLogError (condition, stackTrace);
                }
            };
        }

        private void OnLogError (string condition, string stackTrace)
        {
            var param = new MessagePopup.Param ()
            {
                message = condition + "\n" + stackTrace
            };
            PopupManager.Instance.OpenErrorPopup (PopupType.Message, param, popup =>
            {
                popup.onClosed += () => GameSceneManager.Instance.ChangeScene (GameSceneType.Title);
            });
        }
    }
}