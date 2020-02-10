using Chsopoly.GameScene.Ingame.Object.Character;

namespace Chsopoly.GameScene.Ingame.Object.State.Character
{
    public class CharacterStateIdle : IObjectState<CharacterObject.State, CharacterObjectModel.Animation, CharacterObject>
    {
        void IObjectState<CharacterObject.State, CharacterObjectModel.Animation, CharacterObject>.OnEnter (CharacterObject owner)
        {
            owner.Model.PlayAnimation (CharacterObjectModel.Animation.Idle);
            owner.StateMachine.SetStateTimerInfinite ();
        }

        void IObjectState<CharacterObject.State, CharacterObjectModel.Animation, CharacterObject>.OnUpdate (CharacterObject owner)
        {

        }

        void IObjectState<CharacterObject.State, CharacterObjectModel.Animation, CharacterObject>.OnComplete (CharacterObject owner)
        {

        }

        void IObjectState<CharacterObject.State, CharacterObjectModel.Animation, CharacterObject>.OnExit (CharacterObject owner)
        {

        }
    }
}