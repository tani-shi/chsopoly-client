using System;
using System.Collections;
using System.Collections.Generic;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.Gs2;
using Chsopoly.GameScene.Ingame;
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

        public class Param : IGameSceneParam
        {
            public int capacity;
        }

        [SerializeField]
        private Text _progressText = default;

        private int _capacity;

        void OnDestroy ()
        {
            Gs2Manager.Instance.onMatchingComplete -= OnMatchingComplete;
        }

        protected override IEnumerator LoadProc (Param param)
        {
            Gs2Manager.Instance.onMatchingComplete += OnMatchingComplete;
            _progressText.text = MessageFindGathering;
            _capacity = param.capacity;

            StartCoroutine (Gs2Manager.Instance.JoinGathering (r =>
            {
                if (r.Result.Item == null)
                {
                    _progressText.text = MessageCreateGathering;

                    StartCoroutine (Gs2Manager.Instance.CreateGathering (param.capacity, _ => _progressText.text = MessageMatching));
                }
                else
                {
                    _progressText.text = MessageMatching;
                }
            }));

            yield break;
        }

        private void OnMatchingComplete (List<string> playerIds)
        {
            GameSceneManager.Instance.ChangeScene (GameSceneType.Ingame, new IngameScene.Param ()
            {
                stageId = 1,
            });
        }
    }
}