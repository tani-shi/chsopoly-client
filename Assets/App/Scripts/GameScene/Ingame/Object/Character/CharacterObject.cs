using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Character
{
    public class CharacterObject : BaseObject<CharacterObject.State, CharacterObjectModel.Animation, CharacterObject>
    {
        public enum State
        {
            None,
            Idle,
        }

        public override void Initialize (int id)
        {

        }
    }
}