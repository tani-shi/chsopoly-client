using System;
using System.Collections;
using Chsopoly.Libs.Extensions;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object
{
    public abstract class BaseObjectModel<ANIM> : MonoBehaviour where ANIM : struct, IConvertible
    {
        public float AnimationSpeed
        {
            get
            {
                return _animationSpeed;
            }
            set
            {
                _animationSpeed = value;
                if (Animator != null)
                {
                    Animator.speed = value;
                }
            }
        }

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

        public bool IsPaused
        {
            get
            {
                return _paused;
            }
        }

        private float _animationSpeed = 1.0f;
        private Animator _animator = null;
        private bool _paused = false;

        public bool IsPlayingAnimation (ANIM animation)
        {
            return Animator.IsPlaying (animation.ToString ());
        }

        public void PlayAnimation (ANIM animation)
        {
            if (Animator != null && !IsPlayingAnimation (animation))
            {
                Animator.Play (animation.ToString ());
            }
        }

        public void CrossFadeAnimation (ANIM animation, float duration = 0.1f)
        {
            if (Animator != null && !IsPlayingAnimation (animation))
            {
                Animator.CrossFade (animation.ToString (), duration);
            }
        }

        public void CrossFadeAnimationInFixedTime (ANIM animation, float time = 0.1f)
        {
            if (Animator != null && !IsPlayingAnimation (animation))
            {
                Animator.CrossFadeInFixedTime (animation.ToString (), time);
            }
        }

        public void PauseAnimation ()
        {
            if (!IsPaused)
            {
                Animator.speed = 0f;
                _paused = true;
            }
        }

        public void ResumeAnimation ()
        {
            if (IsPaused)
            {
                Animator.speed = _animationSpeed;
                _paused = false;
            }
        }

        public void UpdateAnimation ()
        {
            Animator.Update (Time.deltaTime);
        }
    }
}