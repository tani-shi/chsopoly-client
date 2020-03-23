using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.Components.Button
{
    [RequireComponent (typeof (Animator))]
    public abstract class StateButton<T> : BaseButton where T : struct, IConvertible
    {
        public Animator Animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponent<Animator> ();
                }
                return _animator;
            }
        }

        public T State
        {
            get
            {
                return _state;
            }
        }

        private Animator _animator = null;
        private T _state = default (T);

        protected override void Start ()
        {
            base.Start ();

            if (_animator == null)
            {
                _animator = GetComponent<Animator> ();
            }

            onClick.AddListener (_ =>
            {
                var nextState = (int) Enum.ToObject (typeof (T), _state) + 1;
                if (nextState >= Enum.GetValues (typeof (T)).Length)
                {
                    nextState = 0;
                }
                _state = (T) Enum.ToObject (typeof (T), nextState);
                _animator.Play (_state.ToString ());
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