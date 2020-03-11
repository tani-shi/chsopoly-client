using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Gimmick.Parts
{
    public class GimmickDamageColliderParts : BaseGimmickParts
    {
        private int _damage = 0;

        public override void Initialize (uint id)
        {
            var data = MasterDataManager.Instance.Get<GimmickDAO> ().Get (id);
            _damage = data.damage;
        }

        void OnCollisionEnter2D (Collision2D collision)
        {
            var character = collision.collider.GetComponent<CharacterObject> ();
            if (character != null && character.IsPlayer)
            {
                character.Damage (_damage);
            }
        }

        void OnCollisionStay2D (Collision2D collision)
        {
            OnCollisionEnter2D (collision);
        }
    }
}