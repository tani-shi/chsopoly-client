using Chsopoly.GameScene.Ingame.Object.Character;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Character.State
{
    public class CharacterStateDying : IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>
    {
        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnEnter (CharacterObject owner)
        {
            owner.Model.PlayAnimation (CharacterObjectModel.Animation.Dying);
            owner.StateMachine.SetStateTimer (Application.targetFrameRate);
            owner.SetMoveDirection (CharacterObject.MoveDirection.None);
            owner.Hp = 0;
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnUpdate (CharacterObject owner)
        {
            if (owner.IsGround)
            {
                owner.Rigidbody.isKinematic = true;
                owner.Rigidbody.velocity = Vector2.zero;
            }
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnComplete (CharacterObject owner)
        {
            owner.StateMachine.SetNextState (CharacterStateMachine.State.Dead);
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnExit (CharacterObject owner)
        {

        }
    }
}