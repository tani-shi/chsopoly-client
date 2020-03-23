using System;
using System.Collections;
using Chsopoly.Libs.Extensions;
using UnityEngine;

namespace Chsopoly.BaseSystem.Popup
{
    public abstract class BasePopup<T> : MonoBehaviour, IPopup where T : class, IPopupParam, new ()
    {
        private const string OPEN_STATE = "Open";
        private const string CLOSE_STATE = "Close";

        public event Action onOpened;
        public event Action onClosed;

        public bool IsReady
        {
            get
            {
                return _state == State.Ready;
            }
        }

        public bool IsAnimationPlaying
        {
            get
            {
                return _state == State.Opening || _state == State.Closing;
            }
        }

        public bool IsOpen
        {
            get
            {
                return _state == State.Opening || _state == State.Idle;
            }
        }

        public T param
        {
            get
            {
                return _param;
            }
        }

        private enum State
        {
            None,
            Ready,
            Opening,
            Idle,
            Closing,
        }

        private T _param = null;
        private State _state = State.None;

        public IEnumerator Load (IPopupParam param)
        {
            _param = param as T;
            _state = State.None;

            yield return LoadProc ();
            Initialize ();

            _state = State.Ready;
        }

        public void Open ()
        {
            if (_state != State.Ready)
            {
                return;
            }

            _state = State.Opening;

            if (GetComponent<Animator> () is Animator animator)
            {
                animator.Play (OPEN_STATE);
            }
            else
            {
                transform.localScale = Vector3.zero;

                OnAnimationEnded ();
            }
        }

        public void Close ()
        {
            if (_state != State.Idle)
            {
                return;
            }

            _state = State.Closing;

            if (GetComponent<Animator> () is Animator animator)
            {
                animator.Play (CLOSE_STATE);
            }
            else
            {
                transform.localScale = Vector3.zero;

                OnAnimationEnded ();
            }
        }

        protected virtual IEnumerator LoadProc ()
        {
            yield break;
        }

        protected virtual void Initialize () { }

        protected void OnAnimationEnded ()
        {
            if (_state == State.Opening)
            {
                _state = State.Idle;
                onOpened.SafeInvoke ();
            }
            else if (_state == State.Closing)
            {
                _state = State.Ready;
                onClosed.SafeInvoke ();
            }
        }
    }

    public abstract class BasePopup : BasePopup<BasePopup.Param>
    {
        public class Param : IPopupParam { }
    }
}