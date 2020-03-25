using System;
using Chsopoly.Libs.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chsopoly.Components.Button
{
    [RequireComponent (typeof (Image))]
    public abstract class BaseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private const string TriggerPressDown = "pressDown";
        private const string TriggerPressUp = "pressUp";

        [Serializable]
        public class PointerEvent : UnityEvent<PointerEventData> { }

        public PointerEvent onClick = default;
        public PointerEvent onPressDown = default;
        public PointerEvent onPressUp = default;
        public PointerEvent onDrag = default;

        [SerializeField]
        private float _secondsToClickThreshold = 0.5f;
        [SerializeField]
        private float _distanceToClickThreshold = 5.0f;
        [SerializeField]
        private float _alphaHitTestMinimumThreshold = 0.1f;
        [SerializeField]
        private bool _alphaHitTest = false;

        public Image Image
        {
            get
            {
                if (_image == null)
                {
                    _image = GetComponent<Image> ();
                }
                return _image;
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

        private Image _image = null;
        private Animator _animator = null;
        private float _lastPressedDownTime = 0f;

        protected virtual void Start ()
        {
            if (_image == null)
            {
                _image = GetComponent<Image> ();
            }
            if (_animator == null)
            {
                _animator = GetComponent<Animator> ();
            }
            if (_alphaHitTest)
            {
                _image.alphaHitTestMinimumThreshold = _alphaHitTestMinimumThreshold;
            }
        }

        void IPointerDownHandler.OnPointerDown (PointerEventData eventData)
        {
            onPressDown.Invoke (eventData);

            _lastPressedDownTime = Time.time;

            if (Animator != null)
            {
                Animator.SetTrigger (TriggerPressDown);
            }
        }

        void IPointerUpHandler.OnPointerUp (PointerEventData eventData)
        {
            onPressUp.Invoke (eventData);

            if ((eventData.pressPosition - eventData.position).sqrMagnitude < (_distanceToClickThreshold * _distanceToClickThreshold) &&
                (Time.time - _lastPressedDownTime) < _secondsToClickThreshold)
            {
                onClick.Invoke (eventData);
            }

            if (Animator != null)
            {
                Animator.SetTrigger (TriggerPressUp);
            }
        }

        void IDragHandler.OnDrag (PointerEventData eventData)
        {
            onDrag.Invoke (eventData);
        }
    }
}