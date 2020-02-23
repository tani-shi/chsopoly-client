using System.Collections;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.Gs2;
using Chsopoly.BaseSystem.UserData;
using Chsopoly.GameScene.Matching;
using Chsopoly.UserData.Entity;
using Gs2.Core.Exception;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Title
{
    public class TitleScene : BaseGameScene
    {
        private const string TapMessage = "Tap Screen to Start";
        private const string LoginProcessingMessage = "Login Processing Now...";

        [SerializeField]
        private Text _userIdText = default;
        [SerializeField]
        private Text _guideText = default;

        private bool _startedLogin = false;

        void Start ()
        {
            Gs2Manager.Instance.onError += OnErrorGs2;
        }

        void OnDestroy ()
        {
            Gs2Manager.Instance.onError -= OnErrorGs2;
        }

        protected override IEnumerator LoadProc (Param param)
        {
            var account = UserDataManager.Instance.Load<Account> ();
            if (account != null)
            {
                _userIdText.text = account.Gs2AccountId;
            }
            _guideText.text = TapMessage;

            yield break;
        }

        public void OnClickScreen ()
        {
            if (_startedLogin)
            {
                return;
            }

            _startedLogin = true;
            _guideText.text = LoginProcessingMessage;
            StartCoroutine (DoLogin ());
        }

        private IEnumerator DoLogin ()
        {
            var account = UserDataManager.Instance.Load<Account> ();
            yield return Gs2Manager.Instance.LoginAccount (account.Gs2AccountId, account.Gs2Password, _ =>
            {
                GameSceneManager.Instance.ChangeScene (GameSceneType.Matching, new MatchingScene.Param ()
                {
                    capacity = 2,
                });
            });
        }

        private void OnErrorGs2 (Gs2Exception e)
        {
            _startedLogin = false;
            _guideText.text = TapMessage;
            // TODO: Do something, like opening an error popup to show error logs.
        }
    }
}