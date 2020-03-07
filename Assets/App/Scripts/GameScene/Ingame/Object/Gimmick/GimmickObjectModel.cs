using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Gimmick
{
    public class GimmickObjectModel : BaseObjectModel<GimmickObjectModel.Animation>
    {
        public enum Animation
        {
            Idle,
            Damage,
            Dying,
        }
    }
}