using Chsopoly.GameScene.Ingame.Object.Character.State;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Character
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
                case State.Jump:
                    return new CharacterStateJump ();
            }
            Debug.LogError ("A unknown character state was specified. " + state.ToString ());
            return new CharacterStateIdle ();
        }

        protected override bool CanInterruptState (State state, CharacterObject owner)
        {
            switch (state)
            {
                case State.Idle:
                    return CurrentState == State.Jump;
                case State.Run:
                    return CurrentState == State.Idle ||
                        CurrentState == State.Run;
                case State.Jump:
                    return CurrentState == State.Idle ||
                        (CurrentState == State.Run && owner.CanJump) ||
                        (CurrentState == State.Jump && owner.CanJump);
            }

            return false;
        }

        protected override bool CanConnectState (State state, CharacterObject owner)
        {
            switch (state)
            {
                case State.Idle:
                    return CurrentState == State.Run ||
                        CurrentState == State.Jump;
                case State.Run:
                    return CurrentState == State.Idle ||
                        CurrentState == State.Run;
                case State.Jump:
                    return CurrentState == State.Idle ||
                        (CurrentState == State.Run && owner.CanJump) ||
                        (CurrentState == State.Jump && owner.CanJump);
            }

            return false;
        }
    }
}