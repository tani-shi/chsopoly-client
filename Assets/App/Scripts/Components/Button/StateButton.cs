using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.Components.Button
{
    [RequireComponent (typeof (Animator))]
    public abstract class StateButton<T> : BaseButton where T : struct, IConvertible
    {
        public T State
        {
            get
            {
                return _state;
            }
        }

        private T _state = default (T);

        protected override void Start ()
        {
            base.Start ();

            onClick.AddListener (_ =>
            {
                var nextState = (int) Enum.ToObject (typeof (T), _state) + 1;
                if (nextState >= Enum.GetValues (typeof (T)).Length)
                {
                    nextState = 0;
                }
                _state = (T) Enum.ToObject (typeof (T), nextState);
                Animator.Play (_state.ToString ());
                OnChangedState (_state);
            });
        }

        protected virtual void OnChangedState (T state) { }

        public void SetState (T state)
        {
            if (!_state.Equals (state))
            {
                _state = state;
                Animator.Play (_state.ToString ());
                OnChangedState (state);
            }
        }
    }
}