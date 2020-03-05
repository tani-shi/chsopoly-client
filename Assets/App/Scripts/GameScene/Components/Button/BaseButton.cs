using System;
using Chsopoly.Libs.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Components.Button
{
    [RequireComponent (typeof (Image))]
    public abstract class BaseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Serializable]
        public class PointerEvent : UnityEvent<PointerEventData> { }

        public UnityEvent onClick = default;
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

        private Image _image = null;
        private float _lastPressedDownTime = 0f;

        void Start ()
        {
            if (_image == null)
            {
                _image = GetComponent<Image> ();
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
        }

        void IPointerUpHandler.OnPointerUp (PointerEventData eventData)
        {
            onPressUp.Invoke (eventData);

            if ((eventData.pressPosition - eventData.position).sqrMagnitude < (_distanceToClickThreshold * _distanceToClickThreshold) &&
                (Time.time - _lastPressedDownTime) < _secondsToClickThreshold)
            {
                onClick.Invoke ();
            }
        }

        void IDragHandler.OnDrag (PointerEventData eventData)
        {
            onDrag.Invoke (eventData);
        }
    }
}