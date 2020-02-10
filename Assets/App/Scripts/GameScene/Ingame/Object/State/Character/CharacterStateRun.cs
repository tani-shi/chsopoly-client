using Chsopoly.GameScene.Ingame.Object.Character;

namespace Chsopoly.GameScene.Ingame.Object.State.Character
{
    public class CharacterStateRun : IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>
    {
        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnEnter (CharacterObject owner)
        {
            owner.Model.PlayAnimation (CharacterObjectModel.Animation.Run);
            owner.StateMachine.SetStateTimer (3);
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnUpdate (CharacterObject owner)
        {

        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnComplete (CharacterObject owner)
        {
            owner.SetStateIdle ();
        }

        void IObjectState<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>.OnExit (CharacterObject owner)
        {

        }
    }
}