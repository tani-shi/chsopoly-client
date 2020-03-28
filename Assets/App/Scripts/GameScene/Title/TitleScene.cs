using System.Collections;
using System.Linq;
using Chsopoly.Audio;
using Chsopoly.BaseSystem.Audio;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.Gs2;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.BaseSystem.UserData;
using Chsopoly.GameScene.Matching;
using Chsopoly.MasterData.DAO.Ingame;
using Chsopoly.UserData.Entity;
using Gs2.Core.Exception;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Title
{
    public class TitleScene : BaseGameScene
    {
        private const string MessageInitializeGs2 = "Initialize Gs2...";
        private const string MessageCreateAccount = "Create Account...";
        private const string MessageLogin = "Login Processing Now...";
        private const string MessageWaitForTapScreen = "Tap Screen To Start";
        private const string MessageErrorFormat = "{0}\n{1}";

        [SerializeField]
        private Text _userIdText = default;
        [SerializeField]
        private Text _guideText = default;

        private enum State
        {
            None,
            InitializeGs2,
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
                case State.InitializeGs2:
                    _guideText.text = MessageInitializeGs2;
                    break;
                case State.CreateAccount:
                    _guideText.text = MessageCreateAccount;
                    break;
                case State.Login:
                    _guideText.text = MessageLogin;
                    break;
                case State.WaitForTapScreen:
                    _guideText.text = MessageWaitForTapScreen;
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
            AudioManager.Instance.PlayBgm (Bgm.Main);

            if (!Gs2Manager.Instance.NeedAuth)
            {
                WaitForTapScreen ();
                yield break;
            }

            SetState (State.InitializeGs2);

            StartCoroutine (Gs2Manager.Instance.Initialize (() =>
            {
                if (string.IsNullOrEmpty (UserDataManager.Instance.Account.gs2AccountId))
                {
                    SetState (State.CreateAccount);

                    StartCoroutine (Gs2Manager.Instance.CreateAccount (r =>
                    {
                        RegisterUserData (r.Result.Item.UserId, r.Result.Item.Password, r.Result.Item.CreatedAt);
                        DoLogin ();
                    }));
                }
                else
                {
                    DoLogin ();
                }
            }));

            yield break;
        }

        public void OnClickScreen ()
        {
            if (_currentState == State.WaitForTapScreen)
            {
                GameSceneManager.Instance.ChangeScene (GameSceneType.Mypage);
            }
        }

        private void DoLogin ()
        {
            SetState (State.Login);

            var account = UserDataManager.Instance.Account;

            StartCoroutine (Gs2Manager.Instance.LoginAccount (account.gs2AccountId, account.gs2Password, _ =>
            {
                WaitForTapScreen ();
            }));
        }

        private void WaitForTapScreen ()
        {
            SetState (State.WaitForTapScreen);

            var account = UserDataManager.Instance.Account;

            _userIdText.text = account.gs2AccountId;
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

        private void RegisterUserData (string userId, string password, long createdAt)
        {
            var account = UserDataManager.Instance.Account;
            account.gs2AccountId = userId;
            account.gs2Password = password;
            account.gs2CreatedAt = createdAt;
            account.characterId = 1;
            account.IsDirty = true;

            foreach (var data in MasterDataManager.Instance.Get<CharacterDAO> ())
            {
                var character = new Character ();
                character.characterId = data.id;
                UserDataManager.Instance.Character.Add (character);
            }

            foreach (var data in MasterDataManager.Instance.Get<GimmickDAO> ())
            {
                var gimmick = new Gimmick ();
                gimmick.gimmickId = data.id;
                gimmick.isActive = true;
                UserDataManager.Instance.Gimmick.Add (gimmick);
            }

            UserDataManager.Instance.Save ();
        }
    }
}