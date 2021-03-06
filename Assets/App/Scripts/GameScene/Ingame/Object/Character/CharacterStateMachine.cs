using Chsopoly.GameScene.Ingame.Object.Gimmick;
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
            Dying,
            Dead,
            Run,
            Jump,
            Fall,
            Guard,
            Damage,
            Appeal,
            Destroy,
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
                case State.Fall:
                    return new CharacterStateFall ();
                case State.Appeal:
                    return new CharacterStateAppeal ();
                case State.Dying:
                    return new CharacterStateDying ();
                case State.Dead:
                    return new CharacterStateDead ();
                case State.Guard:
                    return new CharacterStateGuard ();
                case State.Destroy:
                    return new CharacterStateDestroy ();
                case State.Damage:
                    return new CharacterStateDamage ();
            }
            Debug.LogError ("A unknown character state was specified. " + state.ToString ());
            return new CharacterStateIdle ();
        }

        protected override bool CanInterruptState (State state)
        {
            switch (state)
            {
                case State.Idle:
                    return CurrentState == State.Run ||
                        (CurrentState == State.Fall && Owner.IsLanded) ||
                        CurrentState == State.Guard;
                case State.Run:
                    return CurrentState == State.Idle ||
                        (CurrentState == State.Fall && Owner.IsLanded) ||
                        CurrentState == State.Guard;
                case State.Jump:
                    return CurrentState == State.Idle ||
                        (CurrentState == State.Run && Owner.CanJump) ||
                        (CurrentState == State.Fall && Owner.CanJump) ||
                        CurrentState == State.Guard;
                case State.Fall:
                    return CurrentState == State.Run ||
                        CurrentState == State.Jump ||
                        CurrentState == State.Guard;
                case State.Appeal:
                    return CurrentState == State.Idle ||
                        CurrentState == State.Jump ||
                        CurrentState == State.Fall ||
                        CurrentState == State.Run ||
                        CurrentState == State.Guard;
                case State.Dying:
                    return CurrentState == State.Idle ||
                        CurrentState == State.Jump ||
                        CurrentState == State.Run ||
                        CurrentState == State.Fall ||
                        CurrentState == State.Guard;
                case State.Guard:
                    return CurrentState == State.Idle ||
                        CurrentState == State.Run;
                case State.Destroy:
                    return Owner.TargetGimmick != null &&
                        Owner.TargetGimmick.HitPoint > 0 &&
                        (CurrentState == State.Idle || CurrentState == State.Run);
                case State.Damage:
                    return !Owner.HasStatus (CharacterObject.Status.Invincible) &&
                        (CurrentState == State.Idle ||
                            CurrentState == State.Run ||
                            CurrentState == State.Jump ||
                            CurrentState == State.Fall);
            }

            return false;
        }

        protected override bool CanConnectState (State state)
        {
            switch (state)
            {
                case State.Dead:
                    return CurrentState == State.Dying;
                case State.Fall:
                    return CurrentState == State.Jump ||
                        CurrentState == State.Damage;
                case State.Idle:
                    return CurrentState == State.Destroy;
                case State.Dying:
                    return CurrentState == State.Damage;
                case State.Damage:
                    return !Owner.HasStatus (CharacterObject.Status.Invincible) &&
                        CurrentState == State.Jump;
            }

            return false;
        }
    }
}