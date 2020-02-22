using System.Collections;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.Gs2;
using Chsopoly.BaseSystem.UserData;
using Chsopoly.GameScene.Ingame;
using Chsopoly.UserData.Entity;
using Gs2.Core.Exception;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Title
{
    public class TitleScene : BaseGameScene
    {
        [SerializeField]
        private Text _userIdText = default;

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

            yield break;
        }

        public void OnClickScreen ()
        {
            if (_startedLogin)
            {
                return;
            }

            _startedLogin = true;
            StartCoroutine (DoLogin ());
        }

        private IEnumerator DoLogin ()
        {
            var account = UserDataManager.Instance.Load<Account> ();
            yield return Gs2Manager.Instance.LoginAccount (account.Gs2AccountId, account.Gs2Password, _ =>
            {
                GameSceneManager.Instance.ChangeScene (GameSceneType.Ingame, new IngameScene.Param ()
                {
                    stageId = 1,
                });
            });
        }

        private void OnErrorGs2 (Gs2Exception e)
        {
            _startedLogin = false;
            // TODO: Do something, like opening an error popup to show error logs.
        }
    }
}