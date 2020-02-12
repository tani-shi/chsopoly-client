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
            Run,
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
                case State.Run:
                    return new GimmickStateRun ();
            }
            Debug.LogError ("A unknown gimmick state was specified. " + state.ToString ());
            return new GimmickStateIdle ();
        }

        protected override bool CanConnectState (State state, GimmickObject owner)
        {
            switch (state)
            {
                case State.Idle:
                    return false;
                case State.Run:
                    return CurrentState == State.Idle;
            }

            return false;
        }

        protected override bool CanInterruptState (State state, GimmickObject owner)
        {
            switch (state)
            {
                case State.Idle:
                    return false;
                case State.Run:
                    return CurrentState == State.Idle;
            }

            return false;
        }
    }
}