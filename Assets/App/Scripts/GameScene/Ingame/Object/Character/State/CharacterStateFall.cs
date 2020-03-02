using Chsopoly.GameScene.Ingame.Object.Character;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Character.State
{
    public class CharacterStateFall : IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>
    {
        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnEnter (CharacterObject owner)
        {
            owner.StateMachine.SetStateTimerInfinite ();
            owner.IsLanded = false;
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnUpdate (CharacterObject owner)
        {
            if (owner.IsGround)
            {
                owner.IsLanded = true;

                if (owner.Direction != CharacterObject.MoveDirection.None)
                {
                    owner.StateMachine.SetNextState (CharacterStateMachine.State.Run);
                }
                else
                {
                    owner.StateMachine.SetNextState (CharacterStateMachine.State.Idle);
                }
            }
            else
            {
                owner.Rigidbody.velocity = new Vector2 (owner.MoveVelocity, owner.Rigidbody.velocity.y);
            }

            if (owner.worldPosition.y + (owner.transform as RectTransform).rect.height < 0)
            {
                owner.StateMachine.SetNextState (CharacterStateMachine.State.Dying);
            }
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnComplete (CharacterObject owner)
        {

        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnExit (CharacterObject owner)
        {
            owner.ResetAerialJumpCounter ();
            owner.Rigidbody.velocity = new Vector2 (owner.Rigidbody.velocity.x, 0);
        }
    }
}