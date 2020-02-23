using System;
using System.Collections;
using System.Collections.Generic;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.Gs2;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Matching
{
    public class MatchingScene : BaseGameScene<MatchingScene.Param>
    {
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

        void OnDestroy ()
        {
            Gs2Manager.Instance.onUpdateJoinedPlayerIds -= OnUpdatedJoinedPlayerIds;
        }

        protected override IEnumerator LoadProc (Param param)
        {
            Gs2Manager.Instance.onUpdateJoinedPlayerIds += OnUpdatedJoinedPlayerIds;

            yield return Gs2Manager.Instance.CreateGathering (param.capacity, r =>
            {
                _nameText.text = r.Result.Item.Name;
                _metaDataText.text = r.Result.Item.Metadata;
                _createdAtText.text = new DateTime (r.Result.Item.CreatedAt).ToString ();
                _updatedAtText.text = new DateTime (r.Result.Item.UpdatedAt).ToString ();
            });
        }

        private void OnUpdatedJoinedPlayerIds (List<string> playerIds)
        {
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
    }
}