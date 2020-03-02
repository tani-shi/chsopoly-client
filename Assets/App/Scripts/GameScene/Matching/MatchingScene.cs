using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.Gs2;
using Chsopoly.BaseSystem.UserData;
using Chsopoly.GameScene.Ingame;
using Chsopoly.Gs2.Models;
using Chsopoly.Libs.Extensions;
using Chsopoly.UserData.Entity;
using Gs2.Core.Exception;
using Gs2.Unity.Gs2Matchmaking.Model;
using Gs2.Unity.Gs2Realtime.Result;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Matching
{
    public class MatchingScene : BaseGameScene<MatchingScene.Param>
    {
        private const string MessageFindGathering = "Find Gathering...";
        private const string MessageCreateGathering = "Create Gathering...";
        private const string MessageMatching = "Matching...";
        private const string MessageWaitForCreateRoom = "Waiting For Create Room...";
        private const string MessageConnectRoom = "Connecting Room...";
        private const string MessageWaitForOtherPlayers = "Waiting For Joining Other Players...";
        private const string MessageCloseRoomConnection = "Closing Room Connection...";
        private const string MessageCancelGathering = "Cancel Gathering...";
        private const string MessageErrorFormat = "{0}\n{1}";

        public class Param : IGameSceneParam
        {
            public int capacity;
        }

        [SerializeField]
        private Text _progressText = default;
        [SerializeField]
        private Button _backButton = default;

        private enum State
        {
            None,
            Error,
            FindGathering,
            CreateGathering,
            Matching,
            WaitForCreateRoom,
            ConnectRoom,
            WaitForOtherPlayers,
            CloseRoomConnection,
            CancelGathering,
        }

        private int _capacity = 0;
        private State _currentState = State.None;
        private int _timeoutForWaitForCreateRoom = 120;
        private Dictionary<uint, Profile> _joinedPlayers = new Dictionary<uint, Profile> ();
        private Gs2Exception _exception = null;
        private EzGathering _gathering = null;

        void Update ()
        {
            switch (_currentState)
            {
                case State.None:
                    _progressText.text = string.Empty;
                    break;
                case State.FindGathering:
                    _progressText.text = MessageFindGathering;
                    break;
                case State.CreateGathering:
                    _progressText.text = MessageCreateGathering;
                    break;
                case State.Matching:
                    _progressText.text = MessageMatching;
                    break;
                case State.WaitForCreateRoom:
                    _progressText.text = MessageWaitForCreateRoom;
                    break;
                case State.ConnectRoom:
                    _progressText.text = MessageConnectRoom;
                    break;
                case State.WaitForOtherPlayers:
                    _progressText.text = MessageWaitForOtherPlayers;
                    break;
                case State.CloseRoomConnection:
                    _progressText.text = MessageCloseRoomConnection;
                    break;
                case State.CancelGathering:
                    _progressText.text = MessageCancelGathering;
                    break;
                case State.Error:
                    _progressText.text = string.Format (MessageErrorFormat, _exception.GetType ().FullName, _exception.Message);
                    break;
            }
        }

        void OnDestroy ()
        {
            Gs2Manager.Instance.onCompleteMatching -= OnMatchingComplete;
            Gs2Manager.Instance.onJoinRealtimePlayer -= OnJoinRealtimePlayer;
            Gs2Manager.Instance.onError -= OnGs2Error;
        }

        public void OnClickBackButton ()
        {
            _backButton.interactable = false;

            SetState (State.CloseRoomConnection);

            Gs2Manager.Instance.StartCloseRoomConnection (() =>
            {
                if (_gathering != null)
                {
                    SetState (State.CancelGathering);

                    Gs2Manager.Instance.StartCancelGathering (_gathering.GatheringId, result =>
                    {
                        GameSceneManager.Instance.ChangeScene (GameSceneType.Title);
                    });
                }
                else
                {
                    GameSceneManager.Instance.ChangeScene (GameSceneType.Title);
                }
            });
        }

        protected override IEnumerator LoadProc (Param param)
        {
            Gs2Manager.Instance.onCompleteMatching += OnMatchingComplete;
            Gs2Manager.Instance.onJoinRealtimePlayer += OnJoinRealtimePlayer;
            Gs2Manager.Instance.onError += OnGs2Error;

            SetState (State.FindGathering);
            _capacity = param.capacity;
            _backButton.gameObject.SetActive (false);

            StartCoroutine (Gs2Manager.Instance.JoinGathering (r1 =>
            {
                if (r1.Result.Item == null)
                {
                    SetState (State.CreateGathering);

                    StartCoroutine (Gs2Manager.Instance.CreateGathering (param.capacity, r2 =>
                    {
                        SetState (State.Matching);

                        _gathering = r2.Result.Item;
                    }));
                }
                else
                {
                    SetState (State.Matching);

                    _gathering = r1.Result.Item;
                }
            }));

            yield break;
        }

        private void OnMatchingComplete (string gatheringId)
        {
            StartConnectRoom (gatheringId);
        }

        private void OnJoinRealtimePlayer (uint connectionId, Gs2PacketModel model)
        {
            var profile = model as Profile;
            _joinedPlayers.Add (connectionId, profile);

            if (_joinedPlayers.Count == _capacity - 1)
            {
                var account = UserDataManager.Instance.Load<Account> ();

                GameSceneManager.Instance.ChangeScene (GameSceneType.Ingame, new IngameScene.Param ()
                {
                    stageId = 1, otherPlayers = _joinedPlayers, characterId = account.CharacterId
                });
            }
        }

        private void OnGs2Error (Gs2Exception e)
        {
            SetState (State.Error, true);

            _exception = e;
            _backButton.gameObject.SetActive (true);
            _backButton.interactable = true;
        }

        private void SetState (State state, bool force = false)
        {
            if (force)
            {
                _currentState = state;
            }
            else
            {
                _currentState = (State) Mathf.Max ((int) state, (int) _currentState);
            }
        }

        private void StartConnectRoom (string roomName)
        {
            StartCoroutine (WaitForCreateRoom (roomName, (ipAddress, port, encryptionKey) =>
            {
                SetState (State.ConnectRoom);

                var account = UserDataManager.Instance.Load<Account> ();
                var profile = new Profile ()
                {
                    accountId = account.Gs2AccountId,
                    characterId = 1,
                };

                StartCoroutine (Gs2Manager.Instance.ConnectRoom (
                    ipAddress,
                    port,
                    encryptionKey,
                    profile,
                    r2 =>
                    {
                        SetState (State.WaitForOtherPlayers);
                    }
                ));
            }));
        }

        private IEnumerator WaitForCreateRoom (string roomName, Action<string, int, string> onCreate)
        {
            SetState (State.WaitForCreateRoom);

            yield return Gs2Manager.Instance.GetRoom (roomName,
                r => onCreate.SafeInvoke (r.Result.Item.IpAddress, r.Result.Item.Port, r.Result.Item.EncryptionKey),
                _timeoutForWaitForCreateRoom);
        }
    }
}