using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.BaseSystem.UserData;
using Chsopoly.GameScene.Ingame;
using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.Libs.Extensions;
using Chsopoly.MasterData.DAO.Ingame;
using Chsopoly.UserData.Entity;
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
        private uint[] _characterIds = null;

        public IEnumerator Initialize (List<uint> characterIds)
        {
            _characterIds = characterIds.ToArray ();

            var account = UserDataManager.Instance.Account;
            _currentIndex = Mathf.Max (0, characterIds.FindIndex (id => id == account.characterId));

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

            UserDataManager.Instance.Account.characterId = _characterIds[index];
            UserDataManager.Instance.Account.IsDirty = true;
            UserDataManager.Instance.Save ();
        }
    }
}