using Chsopoly.GameScene.Ingame.Object.Character;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.State.Character
{
    public class CharacterStateMachine : BaseStateMachine<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>
    {
        public enum State
        {
            None,
            Idle,
            Dead,
            Run,
            Jump,
            Squat,
            Damage,
            Appeal,
        }

        public override State DefaultState
        {
            get
            {
                return State.Idle;
            }
        }

        protected override IObjectState<State, CharacterObjectModel.Animation, CharacterObject> CreateState (State state)
        {
            switch (state)
            {
                case State.Idle:
                    return new CharacterStateIdle ();
                case State.Run:
                    return new CharacterStateRun ();
            }
            Debug.LogError ("A unknown character state was specified. " + state.ToString ());
            return new CharacterStateIdle ();
        }

        protected override bool CanInterruptState (State state)
        {
            switch (state)
            {
                case State.Idle:
                    return false;
                case State.Run:
                    return CurrentState == State.Idle ||
                        CurrentState == State.Run;
            }

            return false;
        }

        protected override bool CanConnectState (State state)
        {
            switch (state)
            {
                case State.Idle:
                    return CurrentState == State.Run;
                case State.Run:
                    return CurrentState == State.Idle ||
                        CurrentState == State.Run;
            }

            return false;
        }
    }
}