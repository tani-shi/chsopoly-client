using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.Libs.Extensions;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame
{
    public class IngameCamera : MonoBehaviour
    {
        [SerializeField]
        private Camera _mainCamera = default;
        [SerializeField]
        private Vector2 _offset = new Vector2 (200, 100);

        public Camera MainCamera
        {
            get
            {
                return _mainCamera;
            }
        }

        public Rect ViewRect
        {
            get
            {
                var height = _mainCamera.orthographicSize * 2.0f;
                var width = height * _mainCamera.aspect;
                return new Rect (_transform.position.x - width / 2, _transform.position.y - height / 2, width, height);
            }
        }

        private Vector2 _hBounds = Vector2.zero;
        private Vector2 _vBounds = Vector2.zero;
        private Transform _target = null;
        private Transform _transform = null;

        public void SetTarget (Transform target)
        {
            _target = target;
        }

        public void SetBounds (Vector2 fieldSize)
        {
            var height = _mainCamera.orthographicSize * 2.0f;
            var width = height * _mainCamera.aspect;

            _hBounds.x = width - (fieldSize.x.Half () + width / 2);
            _hBounds.y = (fieldSize.x.Half () + width / 2) - width;
            _vBounds.x = height - height / 2;
            _vBounds.y = (fieldSize.y + height / 2) - height;
        }

        void Awake ()
        {
            _transform = transform;
        }

        void LateUpdate ()
        {
            if (_target == null)
            {
                return;
            }

            var pos = _target.position + new Vector3 (_offset.x, _offset.y);
            pos.x = Mathf.Clamp (pos.x, _hBounds.x, _hBounds.y);
            pos.y = Mathf.Clamp (pos.y, _vBounds.x, _vBounds.y);
            pos.z = transform.position.z;
            transform.position = pos;
        }
    }
}