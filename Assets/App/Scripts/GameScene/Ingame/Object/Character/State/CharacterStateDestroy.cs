using Chsopoly.GameScene.Ingame.Object.Character;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Character.State
{
    public class CharacterStateDestroy : IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>
    {
        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnEnter (CharacterObject owner)
        {
            owner.Model.PlayAnimation (CharacterObjectModel.Animation.Destroy);
            owner.StateMachine.SetStateTimer (Application.targetFrameRate / 2);
            owner.SetMoveDirection (CharacterObject.MoveDirection.None);
            owner.Rigidbody.velocity = new Vector2 (0, owner.Rigidbody.velocity.y);
            owner.TargetGimmick.Damage (owner.Power);
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnUpdate (CharacterObject owner)
        {
            if (!owner.IsGround)
            {
                owner.StateMachine.SetNextState (CharacterStateMachine.State.Fall);
            }
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnComplete (CharacterObject owner)
        {
            owner.StateMachine.SetNextState (CharacterStateMachine.State.Idle);
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnExit (CharacterObject owner)
        {

        }
    }
}