using System.Collections.Generic;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Components
{
    public class PlayerLifeGauge : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _lifeIcon = default;

        public void SetLife (int hp)
        {
            for (int i = 0; i < _lifeIcon.Count; i++)
            {
                _lifeIcon[i].SetActive (i < hp);
            }
        }
    }
}