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
        private const string MessageCreateAccount = "Create Account...";
        private const string MessageLogin = "Login Processing Now...";
        private const string MessageErrorFormat = "{0}\n{1}";

        [SerializeField]
        private Text _userIdText = default;
        [SerializeField]
        private Text _guideText = default;
        [SerializeField]
        private GameObject _buttonContainer = default;

        private enum State
        {
            None,
            CreateAccount,
            Login,
            WaitForTapScreen,
            Error,
        }

        private State _currentState = State.None;
        private Gs2Exception _exception = null;

        void Start ()
        {
            Gs2Manager.Instance.onError += OnErrorGs2;
        }

        void Update ()
        {
            switch (_currentState)
            {
                case State.None:
                    _guideText.text = string.Empty;
                    break;
                case State.CreateAccount:
                    _guideText.text = MessageCreateAccount;
                    break;
                case State.Login:
                    _guideText.text = MessageLogin;
                    break;
                case State.WaitForTapScreen:
                    _guideText.text = string.Empty;
                    break;
                case State.Error:
                    _guideText.text = string.Format (MessageErrorFormat, _exception.GetType ().FullName, _exception.Message);
                    break;
            }
        }

        void OnDestroy ()
        {
            Gs2Manager.Instance.onError -= OnErrorGs2;
        }

        protected override IEnumerator LoadProc (Param param)
        {
            _buttonContainer.SetActive (false);

            var account = UserDataManager.Instance.Load<Account> ();
            if (account == null)
            {
                SetState (State.CreateAccount);

                StartCoroutine (Gs2Manager.Instance.CreateAccount (r =>
                {
                    account = new Account ();
                    account.Gs2AccountId = r.Result.Item.UserId;
                    account.Gs2Password = r.Result.Item.Password;
                    account.Gs2CreatedAt = r.Result.Item.CreatedAt;
                    account.CharacterId = 1;
                    UserDataManager.Instance.Save (account);

                    StartCoroutine (DoLogin ());
                }));
            }
            else
            {
                StartCoroutine (DoLogin ());
            }

            yield break;
        }

        public void OnClickMatch ()
        {
            if (_currentState != State.WaitForTapScreen)
            {
                return;
            }

            var param = new MatchingScene.Param ()
            {
                capacity = 2,
            };
            GameSceneManager.Instance.ChangeScene (GameSceneType.Matching, param);

            _buttonContainer.SetActive (false);
        }

        public void OnClickSingle ()
        {
            if (_currentState != State.WaitForTapScreen)
            {
                return;
            }

            var param = new Chsopoly.GameScene.Ingame.IngameScene.Param ()
            {
                stageId = 1,
                otherPlayers = new System.Collections.Generic.Dictionary<uint, Gs2.Models.Profile> (),
                characterId = 1,
            };
            GameSceneManager.Instance.ChangeScene (GameSceneType.Ingame, param);

            _buttonContainer.SetActive (false);
        }

        private IEnumerator DoLogin ()
        {
            SetState (State.Login);

            var account = UserDataManager.Instance.Load<Account> ();
            _userIdText.text = account.Gs2AccountId;

            yield return Gs2Manager.Instance.LoginAccount (account.Gs2AccountId, account.Gs2Password, _ =>
            {
                SetState (State.WaitForTapScreen);

                _buttonContainer.SetActive (true);
            });
        }

        private void OnErrorGs2 (Gs2Exception e)
        {
            SetState (State.Error);

            _exception = e;
        }

        private void SetState (State state)
        {
            _currentState = (State) Mathf.Max ((int) state, (int) _currentState);
        }
    }
}