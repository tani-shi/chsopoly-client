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
        private Vector2 _offset = Vector2.zero;

        private Vector2 _hBounds = Vector2.zero;
        private Vector2 _vBounds = Vector2.zero;
        private Transform _target = null;

        public void SetTarget (Transform target)
        {
            _target = target;
        }

        public void SetBounds (Vector2 fieldSize)
        {
            _hBounds.x = Screen.width - (fieldSize.x.Half () + Screen.width / 2);
            _hBounds.y = (fieldSize.x.Half () + Screen.width / 2) - Screen.width;
            _vBounds.x = Screen.height - (fieldSize.y.Half () + Screen.height / 2);
            _vBounds.y = (fieldSize.y.Half () + Screen.height / 2) - Screen.height;

            _mainCamera.orthographicSize = Screen.height / 2;
        }

        void LateUpdate ()
        {
            if (_target == null)
            {
                return;
            }

            var pos = _target.position;
            pos.x = Mathf.Clamp (pos.x, _hBounds.x, _hBounds.y);
            pos.y = Mathf.Clamp (pos.y, _vBounds.x, _vBounds.y);
            pos.z = transform.position.z;
            transform.position = pos;
        }
    }
}