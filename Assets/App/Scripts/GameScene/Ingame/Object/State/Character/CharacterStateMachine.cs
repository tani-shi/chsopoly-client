using Chsopoly.GameScene.Ingame.Object.Character;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.State.Character
{
    public class CharacterStateMachine : BaseStateMachine<CharacterObject.State, CharacterObjectModel.Animation, CharacterObject>
    {
        public override CharacterObject.State DefaultState
        {
            get
            {
                return CharacterObject.State.Idle;
            }
        }

        protected override IObjectState<CharacterObject.State, CharacterObjectModel.Animation, CharacterObject> CreateState (CharacterObject.State state)
        {
            switch (state)
            {
                case CharacterObject.State.Idle:
                    return new CharacterStateIdle ();
            }
            Debug.LogError ("A unknown character state was specified. " + state.ToString ());
            return new CharacterStateIdle ();
        }
    }
}