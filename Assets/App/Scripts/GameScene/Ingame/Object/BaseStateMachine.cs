using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object
{
    public abstract class BaseStateMachine<STATE, ANIM, OBJ> : MonoBehaviour where STATE : struct, IConvertible where ANIM : struct, IConvertible where OBJ : BaseObject<STATE, ANIM, OBJ>
    {
        private const int StateMaxDepth = 5;

        public event Action<STATE, STATE> onStateChanged = null;

        public abstract STATE DefaultState
        {
            get;
        }

        public STATE CurrentState
        {
            get
            {
                return _currentState;
            }
        }

        public STATE PreviousState
        {
            get
            {
                return _previousState;
            }
        }

        public STATE NextState
        {
            get
            {
                return _nextState;
            }
        }

        public float StateElapsedTime
        {
            get
            {
                return (float) _stateElapsedFrames / (float) Application.targetFrameRate;
            }
        }

        public int StateTimer
        {
            get
            {
                return _stateTimer;
            }
        }

        public OBJ Owner
        {
            get
            {
                return _owner;
            }
        }

        [SerializeField]
        private STATE _currentState = default;

        private STATE _previousState = default;
        private STATE _nextState = default;
        private IObjectState<STATE, ANIM, OBJ> _state = null;
        private Stack<STATE> _stateStack = new Stack<STATE> ();
        private int _stateElapsedFrames = 0;
        private int _stateTimer = 0;
        private OBJ _owner = null;

        public BaseStateMachine ()
        {
            _nextState = DefaultState;
        }

        public virtual void Initialize (OBJ owner)
        {
            _owner = owner;
        }

        public void Execute ()
        {
            _stateStack.Clear ();

            UpdateStateTimer ();
            UpdateState ();
            ExecuteState ();
        }

        public void SetStateTimer (int timer)
        {
            _stateTimer = timer;
        }

        public void SetStateTimerInfinite ()
        {
            _stateTimer = -1;
        }

        public bool SetNextState (STATE state, bool force = false)
        {
            if (CanSetState (state) || force)
            {
                _nextState = state;
                return true;
            }

            return false;
        }

        public bool CanSetState (STATE state)
        {
            if (_stateTimer == 0)
            {
                return CanConnectState (state);
            }
            return CanInterruptState (state);
        }

        protected abstract IObjectState<STATE, ANIM, OBJ> CreateState (STATE state);
        protected abstract bool CanInterruptState (STATE state);
        protected abstract bool CanConnectState (STATE state);

        private void UpdateStateTimer ()
        {
            if (_state != null)
            {
                if (_stateTimer > 0)
                {
                    _stateTimer--;
                }
                _stateElapsedFrames++;
            }
        }

        private void UpdateState ()
        {
            _stateStack.Push (_currentState);
            if (_stateStack.Count > StateMaxDepth)
            {
                string trace = "";
                while (_stateStack.Count > 0)
                {
                    trace += _stateStack.Pop ().ToString ();
                    if (_stateStack.Count > 0)
                    {
                        trace += "<<";
                    }
                }
                Debug.LogWarning ("too deep state depth. trace:" + trace);
                return;
            }

            if (!_nextState.Equals (default (STATE)) || _stateTimer == 0)
            {
                if (_state != null)
                {
                    if (_stateTimer == 0)
                    {
                        _state.OnComplete (_owner);
                    }
                    _state.OnExit (_owner);
                }
                if (!_nextState.Equals (default (STATE)))
                {
                    _state = CreateState (_nextState);
                    _previousState = _currentState;
                    _currentState = _nextState;
                    _nextState = default;
                    _stateElapsedFrames = 0;

                    if (onStateChanged != null)
                    {
                        onStateChanged (_previousState, _currentState);
                    }

                    _state.OnEnter (_owner);
                }
                else
                {
                    Debug.LogWarning ("state has exited but next state is null.");
                    _nextState = DefaultState;
                }
            }
            else if (_stateTimer == 0)
            {
                Debug.LogWarning ("state has completed but next state is null.");
                _nextState = DefaultState;
            }

            // state has completed with a frame.
            if (_stateTimer == 0)
            {
                UpdateState ();
            }
        }

        private void ExecuteState ()
        {
            if (_state != null)
            {
                _state.OnUpdate (_owner);
            }
        }
    }
}