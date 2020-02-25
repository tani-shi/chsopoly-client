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
        private const string MessageWaitForCreateRoom = "Waiting For Create Room...";
        private const string MessageGetRoomInfo = "Get Room Infos...";
        private const string MessageConnectRoom = "Connecting Room...";
        private const string MessageWaitForOtherPlayers = "Waiting For Joining Other Players...";

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
            GetRoomInfo,
            ConnectRoom,
            WaitForOtherPlayers,
        }

        private int _capacity = 0;
        private State _currentState = State.None;
        private bool _createdRoom = false;

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
                case State.GetRoomInfo:
                    _progressText.text = MessageGetRoomInfo;
                    break;
                case State.ConnectRoom:
                    _progressText.text = MessageConnectRoom;
                    break;
                case State.WaitForOtherPlayers:
                    _progressText.text = MessageWaitForOtherPlayers;
                    break;
            }
        }

        void OnDestroy ()
        {
            Gs2Manager.Instance.onCompleteMatching -= OnMatchingComplete;
            Gs2Manager.Instance.onJoinRealtimePlayer -= OnJoinRealtimePlayer;
            Gs2Manager.Instance.onCreateRealtimeRoom -= OnCreateRealtimeRoom;
        }

        protected override IEnumerator LoadProc (Param param)
        {
            Gs2Manager.Instance.onCompleteMatching += OnMatchingComplete;
            Gs2Manager.Instance.onJoinRealtimePlayer += OnJoinRealtimePlayer;
            Gs2Manager.Instance.onCreateRealtimeRoom += OnCreateRealtimeRoom;

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
                    }));
                }
                else
                {
                    SetState (State.Matching);
                }
            }));

            yield break;
        }

        private void OnMatchingComplete (string gatheringId)
        {
            if (_createdRoom)
            {
                SetState (State.WaitForCreateRoom);
            }
            else
            {
                StartConnectRoom (gatheringId);
            }
        }

        private void OnCreateRealtimeRoom (string gatheringId)
        {
            _createdRoom = true;

            if (_currentState == State.WaitForCreateRoom)
            {
                StartConnectRoom (gatheringId);
            }
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

        private void StartConnectRoom (string roomName)
        {
            SetState (State.GetRoomInfo);

            StartCoroutine (Gs2Manager.Instance.GetRoom (roomName, r1 =>
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
                        SetState (State.WaitForOtherPlayers);
                    }
                ));
            }));
        }
    }
}