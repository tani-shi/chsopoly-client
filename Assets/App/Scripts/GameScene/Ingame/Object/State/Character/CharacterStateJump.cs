using Chsopoly.GameScene.Ingame.Object.Character;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.State.Character
{
    public class CharacterStateJump : IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>
    {
        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnEnter (CharacterObject owner)
        {
            owner.Model.PlayAnimation (CharacterObjectModel.Animation.Jump);
            owner.StateMachine.SetStateTimerInfinite ();
            owner.Jump ();
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnUpdate (CharacterObject owner)
        {
            owner.MovePosition ();
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnComplete (CharacterObject owner)
        {

        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnExit (CharacterObject owner)
        {

        }
    }
}