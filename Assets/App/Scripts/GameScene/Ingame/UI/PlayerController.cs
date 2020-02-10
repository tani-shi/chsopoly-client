using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.GameScene.Ingame.Object.State.Character;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Chsopoly.GameScene.Ingame.UI
{
    public class PlayerController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private float _tapTimeThreshold = 0.3f;
        [SerializeField]
        private float _tapDistanceThreshold = 10f;
        [SerializeField]
        private float _dragSpeedThreshold = 10f;

        private CharacterObject _playerCharacter = null;
        private bool _dragging = false;
        private bool _draggingDirection = false;
        private float _lastPointDownTime = 0f;
        private Vector2 _lastPointDownPosition = Vector2.zero;

        public void SetPlayer (CharacterObject playerCharacter)
        {
            _playerCharacter = playerCharacter;
        }

        void Update ()
        {
            if (_playerCharacter == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown (KeyCode.A) || Input.GetKey (KeyCode.A))
            {
                _playerCharacter.SetMoveVelocity (false);
                _playerCharacter.SetStateRun ();
            }
            if (Input.GetKeyDown (KeyCode.D) || Input.GetKey (KeyCode.D))
            {
                _playerCharacter.SetMoveVelocity (true);
                _playerCharacter.SetStateRun ();
            }
            if (Input.GetKeyDown (KeyCode.Space))
            {
                _playerCharacter.SetStateJump ();
            }
#endif

            if (_dragging)
            {
                _playerCharacter.SetMoveVelocity (_draggingDirection);
                _playerCharacter.SetStateRun ();
            }
        }

        void IDragHandler.OnDrag (PointerEventData eventData)
        {
            if (_playerCharacter == null)
            {
                return;
            }

            if (eventData.delta.sqrMagnitude > _dragSpeedThreshold)
            {
                _dragging = true;
                _draggingDirection = eventData.delta.x > 0;
            }
        }

        void IPointerDownHandler.OnPointerDown (PointerEventData eventData)
        {
            if (_playerCharacter == null)
            {
                return;
            }

            _lastPointDownPosition = eventData.position;
            _lastPointDownTime = Time.time;
        }

        void IPointerUpHandler.OnPointerUp (PointerEventData eventData)
        {
            if (_playerCharacter == null)
            {
                return;
            }

            _dragging = false;

            if ((_lastPointDownPosition - eventData.position).sqrMagnitude < _tapDistanceThreshold &&
                (Time.time - _lastPointDownTime) < _tapTimeThreshold)
            {
                _playerCharacter.SetStateJump ();
            }
        }
    }
}