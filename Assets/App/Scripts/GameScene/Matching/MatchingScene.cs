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
        private const string MessageErrorFormat = "{0}\n{1}";

        public class Param : IGameSceneParam
        {
            public int capacity;
        }

        [SerializeField]
        private Text _progressText = default;

        private enum State
        {
            None,
            FindGathering,
            CreateGathering,
            Matching,
            WaitForCreateRoom,
            ConnectRoom,
            WaitForOtherPlayers,
            Error,
        }

        private int _capacity = 0;
        private State _currentState = State.None;
        private int _timeoutForWaitForCreateRoom = 60;
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

            if (_gathering != null)
            {
                Gs2Manager.Instance.CancelGathering (_gathering.GatheringId);
            }
        }

        protected override IEnumerator LoadProc (Param param)
        {
            Gs2Manager.Instance.onCompleteMatching += OnMatchingComplete;
            Gs2Manager.Instance.onJoinRealtimePlayer += OnJoinRealtimePlayer;
            Gs2Manager.Instance.onError += OnGs2Error;

            SetState (State.FindGathering);
            _capacity = param.capacity;

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

            if (_joinedPlayers.Count == _capacity)
            {
                GameSceneManager.Instance.ChangeScene (GameSceneType.Ingame, new IngameScene.Param ()
                {
                    stageId = 1, otherPlayers = _joinedPlayers,
                });
            }
        }

        private void OnGs2Error (Gs2Exception e)
        {
            SetState (State.Error);

            _exception = e;
        }

        private void SetState (State state)
        {
            _currentState = (State) Mathf.Max ((int) state, (int) _currentState);
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

            for (int i = 0; i < _timeoutForWaitForCreateRoom; i++)
            {
                EzGetRoomResult result = null;
                yield return Gs2Manager.Instance.GetRoom (roomName, r => result = r.Result);
                if (!string.IsNullOrEmpty (result.Item.IpAddress))
                {
                    onCreate.SafeInvoke (result.Item.IpAddress, result.Item.Port, result.Item.EncryptionKey);
                    yield break;
                }

                yield return new WaitForSeconds (1f);
            }
        }
    }
}