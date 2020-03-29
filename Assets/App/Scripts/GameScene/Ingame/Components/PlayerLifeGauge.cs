using System.Collections;
using System.Collections.Generic;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Ingame.Components
{
    public class PlayerLifeGauge : MonoBehaviour
    {
        [SerializeField]
        private List<Image> _lifeIcon = default;

        public IEnumerator Load (uint characterId)
        {
            var data = MasterDataManager.Instance.Get<CharacterDAO> ().Get (characterId);
            var handle = Addressables.LoadAssetAsync<Sprite> (IngameSettings.Paths.CharacterIcon (data.assetName));
            yield return handle;
            foreach (var icon in _lifeIcon)
            {
                icon.sprite = handle.Result;
                icon.SetNativeSize ();
            }
        }

        public void SetPlayer (CharacterObject player)
        {
            player.StateMachine.onStateChanged += (_, state) =>
            {
                if (state == CharacterStateMachine.State.Damage)
                {
                    SetLife (player.Hp);
                }
                else if (state == CharacterStateMachine.State.Dying)
                {
                    SetLife (0);
                }
            };

            SetLife (player.MaxHp);
        }

        private void SetLife (int hp)
        {
            for (int i = 0; i < _lifeIcon.Count; i++)
            {
                _lifeIcon[i].gameObject.SetActive (i < hp);
            }
        }
    }
}