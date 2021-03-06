using Chsopoly.GameScene.Ingame.Object.Character;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Character.State
{
    public class CharacterStateRun : IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>
    {
        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnEnter (CharacterObject owner)
        {
            owner.Model.PlayAnimation (CharacterObjectModel.Animation.Run);
            owner.StateMachine.SetStateTimerInfinite ();
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnUpdate (CharacterObject owner)
        {
            if (!owner.IsGround)
            {
                owner.StateMachine.SetNextState (CharacterStateMachine.State.Fall);
            }
            else if (owner.Direction != CharacterObject.MoveDirection.None)
            {
                owner.Rigidbody.MovePosition (owner.Rigidbody.position + new Vector2 (owner.MoveVelocity * Time.deltaTime, 0));
            }
            else
            {
                owner.StateMachine.SetNextState (CharacterStateMachine.State.Idle);
            }
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnComplete (CharacterObject owner)
        {

        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnExit (CharacterObject owner)
        {

        }
    }
}