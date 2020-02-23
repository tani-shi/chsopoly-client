using System;
using System.Collections;
using System.Collections.Generic;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.Gs2;
using Gs2.Unity.Gs2Matchmaking.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Matching
{
    public class MatchingScene : BaseGameScene<MatchingScene.Param>
    {
        private const string MessageFindGathering = "Find Gathering...";
        private const string MessageCreateGathering = "Create Gathering...";

        public class Param : IGameSceneParam
        {
            public int capacity;
        }

        [SerializeField]
        private List<Text> _playerIdTexts = default;
        [SerializeField]
        private Text _nameText = default;
        [SerializeField]
        private Text _metaDataText = default;
        [SerializeField]
        private Text _createdAtText = default;
        [SerializeField]
        private Text _updatedAtText = default;
        [SerializeField]
        private Text _progressText = default;
        [SerializeField]
        private GameObject _matchingContainer = default;
        [SerializeField]
        private GameObject _loadingContainer = default;
        [SerializeField]
        private Button _startButton = default;

        private int _capacity;

        void OnDestroy ()
        {
            Gs2Manager.Instance.onUpdateJoinedPlayerIds -= OnUpdatedJoinedPlayerIds;
        }

        protected override IEnumerator LoadProc (Param param)
        {
            Gs2Manager.Instance.onUpdateJoinedPlayerIds += OnUpdatedJoinedPlayerIds;
            _progressText.text = MessageFindGathering;
            _startButton.interactable = false;
            _capacity = param.capacity;

            StartCoroutine (Gs2Manager.Instance.JoinGathering (r1 =>
            {
                if (r1.Result.Item == null)
                {
                    _progressText.text = MessageCreateGathering;

                    StartCoroutine (Gs2Manager.Instance.CreateGathering (param.capacity, r2 => UpdateGathering (r2.Result.Item)));
                }
                else
                {
                    UpdateGathering (r1.Result.Item);
                }
            }));

            SetLoadingActive (true);

            yield break;
        }

        private void OnUpdatedJoinedPlayerIds (List<string> playerIds)
        {
            _startButton.interactable = playerIds.Count == _capacity;

            for (int i = 0; i < _playerIdTexts.Count; i++)
            {
                if (i < playerIds.Count)
                {
                    _playerIdTexts[i].gameObject.SetActive (true);
                    _playerIdTexts[i].text = playerIds[i];
                }
                else
                {
                    _playerIdTexts[i].gameObject.SetActive (false);
                }
            }
        }

        private void UpdateGathering (EzGathering gathering)
        {
            _nameText.text = gathering.Name;
            _metaDataText.text = gathering.Metadata;
            _createdAtText.text = gathering.CreatedAt.ToString ();
            _updatedAtText.text = gathering.UpdatedAt.ToString ();

            SetLoadingActive (false);
        }

        private void SetLoadingActive (bool active)
        {
            _loadingContainer.SetActive (active);
            _matchingContainer.SetActive (!active);
        }
    }
}