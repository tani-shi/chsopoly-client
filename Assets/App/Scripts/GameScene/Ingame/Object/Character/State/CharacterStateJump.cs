using Chsopoly.GameScene.Ingame.Object.Character;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Character.State
{
    public class CharacterStateJump : IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>
    {
        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnEnter (CharacterObject owner)
        {
            owner.Model.PlayAnimation (CharacterObjectModel.Animation.Jump);
            owner.StateMachine.SetStateTimerInfinite ();
            owner.Rigidbody.velocity = new Vector2 ((owner.MoveVelocity * Time.deltaTime) + owner.Rigidbody.velocity.x, Mathf.Sqrt (-2.0f * Physics2D.gravity.y * owner.JumpHeight));

            if (!owner.IsLanded)
            {
                owner.CountAerialJump ();
            }

            owner.IsLanded = false;
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnUpdate (CharacterObject owner)
        {
            if (owner.Rigidbody.velocity.y < 0)
            {
                owner.StateMachine.SetNextState (CharacterStateMachine.State.Fall);
            }

            owner.Rigidbody.velocity = new Vector2 (owner.MoveVelocity, owner.Rigidbody.velocity.y);
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnComplete (CharacterObject owner)
        {

        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnExit (CharacterObject owner)
        {

        }
    }
}