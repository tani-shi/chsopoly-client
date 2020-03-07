using Chsopoly.GameScene.Ingame.Object.Gimmick.State;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Gimmick
{
    public class GimmickStateMachine : BaseStateMachine<GimmickStateMachine.State, GimmickObjectModel.Animation, GimmickObject>
    {
        public enum State
        {
            None,
            Idle,
            Damage,
            Dying,
            Dead,
        }

        public override State DefaultState
        {
            get
            {
                return State.Idle;
            }
        }

        protected override IObjectState<GimmickStateMachine.State, GimmickObjectModel.Animation, GimmickObject> CreateState (State state)
        {
            switch (state)
            {
                case State.Idle:
                    return new GimmickStateIdle ();
                case State.Damage:
                    return new GimmickStateDamage ();
                case State.Dying:
                    return new GimmickStateDying ();
                case State.Dead:
                    return new GimmickStateDead ();
            }
            Debug.LogError ("A unknown gimmick state was specified. " + state.ToString ());
            return new GimmickStateIdle ();
        }

        protected override bool CanInterruptState (State state)
        {
            switch (state)
            {
                case State.Damage:
                    return CurrentState == State.Idle ||
                        CurrentState == State.Damage;
                case State.Dying:
                    return CurrentState == State.Idle ||
                        CurrentState == State.Damage;
            }

            return false;
        }

        protected override bool CanConnectState (State state)
        {
            switch (state)
            {
                case State.Idle:
                    return CurrentState == State.Damage;
                case State.Damage:
                    return CurrentState == State.Damage;
                case State.Dying:
                    return CurrentState == State.Damage;
                case State.Dead:
                    return CurrentState == State.Dying;
            }

            return false;
        }
    }
}