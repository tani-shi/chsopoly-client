using Chsopoly.GameScene.Ingame.Object.Character;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Character.State
{
    public class CharacterStateDamage : IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>
    {
        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnEnter (CharacterObject owner)
        {
            owner.Model.PlayAnimation (CharacterObjectModel.Animation.Damage);
            owner.StateMachine.SetStateTimer (Application.targetFrameRate);
            owner.Rigidbody.velocity = new Vector2 (
                owner.Direction == CharacterObject.MoveDirection.Left ? IngameSettings.Character.DamageBackForce : -IngameSettings.Character.DamageBackForce,
                Mathf.Sqrt (-2.0f * Physics2D.gravity.y * IngameSettings.Character.DamageJumpHeight));

            if (!owner.IsLanded)
            {
                owner.CountAerialJump ();
            }

            owner.IsLanded = false;
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnUpdate (CharacterObject owner)
        {
            if (owner.Rigidbody.velocity.y <= 0)
            {
                owner.StateMachine.SetNextState (CharacterStateMachine.State.Fall);
            }
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnComplete (CharacterObject owner)
        {
            if (owner.Hp > 0)
            {
                owner.StateMachine.SetNextState (CharacterStateMachine.State.Fall);
            }
            else
            {
                owner.StateMachine.SetNextState (CharacterStateMachine.State.Dying);
            }
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnExit (CharacterObject owner)
        {
            owner.SetStatus (CharacterObject.Status.Invincible, Mathf.FloorToInt ((float) Application.targetFrameRate * IngameSettings.Character.InvincibleTime));
        }
    }
}