using System.Collections.Generic;
using Chsopoly.GameScene.Ingame.Object.Character;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Components
{
    public class PlayerLifeGauge : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _lifeIcon = default;

        public void SetPlayer (CharacterObject player)
        {
            player.StateMachine.onStateChanged += (_, state) =>
            {
                if (state == CharacterStateMachine.State.Damage)
                {
                    SetLife (player.Hp);
                }
            };

            SetLife (player.MaxHp);
        }

        private void SetLife (int hp)
        {
            for (int i = 0; i < _lifeIcon.Count; i++)
            {
                _lifeIcon[i].SetActive (i < hp);
            }
        }
    }
}