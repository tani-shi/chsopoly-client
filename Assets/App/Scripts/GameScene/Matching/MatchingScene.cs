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

        private int _capacity = 0;
        private EzGathering _gathering = null;

        void OnDestroy ()
        {
            Gs2Manager.Instance.onCompleteMatching -= OnMatchingComplete;
            Gs2Manager.Instance.onJoinRealtimePlayer -= OnJoinRealtimePlayer;
        }

        protected override IEnumerator LoadProc (Param param)
        {
            Gs2Manager.Instance.onCompleteMatching += OnMatchingComplete;
            Gs2Manager.Instance.onJoinRealtimePlayer += OnJoinRealtimePlayer;

            _progressText.text = MessageFindGathering;
            _capacity = param.capacity;

            StartCoroutine (Gs2Manager.Instance.JoinGathering (r1 =>
            {
                if (r1.Result.Item == null)
                {
                    _progressText.text = MessageCreateGathering;

                    StartCoroutine (Gs2Manager.Instance.CreateGathering (param.capacity, r2 =>
                    {
                        _progressText.text = MessageMatching;
                        _gathering = r2.Result.Item;
                    }));
                }
                else
                {
                    _progressText.text = MessageMatching;
                    _gathering = r1.Result.Item;
                }
            }));

            yield break;
        }

        private void OnMatchingComplete (string gatheringId)
        {
            _progressText.text = MessageGetRoomInfo;

            StartCoroutine (Gs2Manager.Instance.GetRoom (gatheringId, r1 =>
            {
                _progressText.text = MessageConnectRoom;

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
                        _progressText.text = MessageWaitOtherPlayers;
                    }
                ));
            }));
        }

        private void OnJoinRealtimePlayer (uint connectionId, IGs2PacketModel model)
        {
            var profile = model as Profile;
            Debug.Log (connectionId + ":" + profile.characterId);
        }
    }
}