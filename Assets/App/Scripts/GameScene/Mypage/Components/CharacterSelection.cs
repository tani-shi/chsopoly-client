using System.Collections;
using System.Collections.Generic;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame;
using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.Libs.Extensions;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Mypage.Components
{
    public class CharacterSelection : MonoBehaviour
    {
        [SerializeField]
        private Transform _characterContainer = default;
        [SerializeField]
        private Text _descriptionText = default;

        private List<GameObject> _characterObjects = new List<GameObject> ();
        private int _currentIndex = 0;

        public IEnumerator Initialize (List<uint> characterIds)
        {
            foreach (var id in characterIds)
            {
                var data = MasterDataManager.Instance.Get<CharacterDAO> ().Get (id);
                var handle = Addressables.LoadAssetAsync<GameObject> (IngameSettings.Paths.CharacterPrefab (data.assetName));
                yield return handle;
                if (handle.Result != null)
                {
                    var obj = handle.Result.CreateInstance (_characterContainer);
                    obj.SetActive (_characterObjects.Count == _currentIndex);
                    _characterObjects.Add (obj);
                }
            }
        }

        public void OnClickNext ()
        {
            var index = _currentIndex + 1;
            if (index >= _characterObjects.Count)
            {
                index = 0;
            }
            ChangeIndex (index);
        }

        public void OnClickPrevious ()
        {
            var index = _currentIndex - 1;
            if (index < 0)
            {
                index = _characterObjects.Count - 1;
            }
            ChangeIndex (index);
        }

        private void ChangeIndex (int index)
        {
            _currentIndex = index;
            for (int i = 0; i < _characterObjects.Count; i++)
            {
                _characterObjects[i].SetActive (i == index);
            }
        }
    }
}