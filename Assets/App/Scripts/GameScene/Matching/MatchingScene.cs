using System;
using System.Collections;
using System.Collections.Generic;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.Gs2;
using Chsopoly.GameScene.Ingame;
using Chsopoly.Gs2.Models;
using Gs2.Unity.Gs2Matchmaking.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Matching
{
    public class MatchingScene : BaseGameScene<MatchingScene.Param>
    {
        private const string MessageFindGathering = "Find Gathering...";
        private const string MessageCreateGathering = "Create Gathering...";
        private const string MessageMatching = "Matching...";
        private const string MessageGetRoomInfo = "Get Room Infos...";
        private const string MessageConnectRoom = "Connecting Room...";
        private const string MessageWaitOtherPlayers = "Waiting For Joining Other Players...";

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
            GetRoomInfo,
            ConnectRoom,
            WaitOtherPlayers,
        }

        private int _capacity = 0;
        private EzGathering _gathering = null;
        private State _currentState = State.None;

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
                case State.GetRoomInfo:
                    _progressText.text = MessageGetRoomInfo;
                    break;
                case State.ConnectRoom:
                    _progressText.text = MessageConnectRoom;
                    break;
                case State.WaitOtherPlayers:
                    _progressText.text = MessageWaitOtherPlayers;
                    break;
            }
        }

        void OnDestroy ()
        {
            Gs2Manager.Instance.onCompleteMatching -= OnMatchingComplete;
            Gs2Manager.Instance.onJoinRealtimePlayer -= OnJoinRealtimePlayer;
        }

        protected override IEnumerator LoadProc (Param param)
        {
            Gs2Manager.Instance.onCompleteMatching += OnMatchingComplete;
            Gs2Manager.Instance.onJoinRealtimePlayer += OnJoinRealtimePlayer;

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
            SetState (State.GetRoomInfo);

            StartCoroutine (Gs2Manager.Instance.GetRoom (gatheringId, r1 =>
            {
                SetState (State.ConnectRoom);

                var profile = new Profile ()
                {
                    characterId = 1,
                };

                StartCoroutine (Gs2Manager.Instance.ConnectRoom (
                    r1.Result.Item.IpAddress,
                    r1.Result.Item.Port,
                    r1.Result.Item.EncryptionKey,
                    profile.Serialize (),
                    r2 =>
                    {
                        SetState (State.WaitOtherPlayers);
                    }
                ));
            }));
        }

        private void OnJoinRealtimePlayer (uint connectionId, IGs2PacketModel model)
        {
            var profile = model as Profile;
            Debug.Log (connectionId + ":" + profile.characterId);
        }

        private void SetState (State state)
        {
            _currentState = (State) Mathf.Max ((int) state, (int) _currentState);
        }
    }
}