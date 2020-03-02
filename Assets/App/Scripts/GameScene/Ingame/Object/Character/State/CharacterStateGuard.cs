using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.Libs.Extensions;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Character.State
{
    public class CharacterStateGuard : IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>
    {
        private Vector2 _defaultColliderOffset;
        private Vector2 _defaultColliderSize;

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnEnter (CharacterObject owner)
        {
            owner.Model.PlayAnimation (CharacterObjectModel.Animation.Guard);
            owner.StateMachine.SetStateTimerInfinite ();
            owner.SetMoveDirection (CharacterObject.MoveDirection.None);
            owner.Rigidbody.velocity = new Vector2 (0, owner.Rigidbody.velocity.y);

            _defaultColliderOffset = owner.Collider.offset;
            _defaultColliderSize = owner.Collider.size;
            owner.Collider.offset *= new Vector2 (1, 0.5f);
            owner.Collider.offset += new Vector2 (0, ((owner.transform as RectTransform).sizeDelta.y.Half () - _defaultColliderOffset.y).Half ());
            owner.Collider.size *= new Vector2 (1, 0.5f);
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnUpdate (CharacterObject owner)
        {

        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnComplete (CharacterObject owner)
        {

        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnExit (CharacterObject owner)
        {
            owner.Collider.offset = _defaultColliderOffset;
            owner.Collider.size = _defaultColliderSize;
        }
    }
}