using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.State
{
    public abstract class BaseStateMachine<STATE, ANIM, OBJ> : MonoBehaviour where STATE : struct, IConvertible where ANIM : struct, IConvertible where OBJ : BaseObject<STATE, ANIM, OBJ>
    {
        private const int StateMaxDepth = 5;

        public event Action<STATE, STATE> onStateChanged = null;

        public abstract STATE DefaultState
        {
            get;
        }

        private STATE _previousState = default;
        private STATE _currentState = default;
        private STATE _nextState = default;
        private IObjectState<STATE, ANIM, OBJ> _state = null;
        private Stack<STATE> _stateStack = new Stack<STATE> ();
        private float _stateElapsedTime = 0;
        private float _stateTimer = 0;

        public void Execute (OBJ owner)
        {
            _stateStack.Clear ();

            UpdateStateTimer ();
            UpdateState (owner);
            ExecuteState (owner);
        }

        public void SetStateTimer (float timer)
        {
            _stateTimer = timer;
        }

        public void SetStateTimerInfinite ()
        {
            _stateTimer = -1;
        }

        protected abstract IObjectState<STATE, ANIM, OBJ> CreateState (STATE state);

        private void UpdateStateTimer ()
        {
            if (_state != null)
            {
                if (_stateTimer > 0)
                {
                    _stateTimer--;
                }
                _stateElapsedTime++;
            }
        }

        private void UpdateState (OBJ owner)
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
                        _state.OnComplete (owner);
                    }
                    _state.OnExit (owner);
                }
                if (!_nextState.Equals (default (STATE)))
                {
                    _state = CreateState (_nextState);
                    _previousState = _currentState;
                    _currentState = _nextState;
                    _nextState = default;
                    _stateElapsedTime = 0;

                    if (onStateChanged != null)
                    {
                        onStateChanged (_previousState, _currentState);
                    }

                    _state.OnEnter (owner);
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
                UpdateState (owner);
            }
        }

        private void ExecuteState (OBJ owner)
        {
            if (_state != null)
            {
                _state.OnUpdate (owner);
            }
        }
    }
}