using Chsopoly.BaseSystem.Gs2;
using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.Gs2.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Chsopoly.GameScene.Ingame.UI
{
    public class PlayerController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        private const int DefaultDraggingId = -999;

        [SerializeField]
        private float _dragSpeedThreshold = 10f;

        private CharacterObject _playerCharacter = null;
        private bool _dragging = false;
        private CharacterObject.MoveDirection _draggingDirection = CharacterObject.MoveDirection.None;

        public void SetPlayer (CharacterObject playerCharacter)
        {
            _playerCharacter = playerCharacter;
        }

        public void OnClickJump ()
        {
            _playerCharacter.StateMachine.SetNextState (CharacterStateMachine.State.Jump);
        }

        void Update ()
        {
            if (_playerCharacter == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown (KeyCode.Space))
            {
                _playerCharacter.StateMachine.SetNextState (CharacterStateMachine.State.Jump);
            }
            if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.S))
            {
                _dragging = true;

                if (Input.GetKey (KeyCode.A))
                {
                    _draggingDirection = CharacterObject.MoveDirection.Left;
                }
                if (Input.GetKey (KeyCode.D))
                {
                    _draggingDirection = CharacterObject.MoveDirection.Right;
                }
                if (Input.GetKey (KeyCode.S))
                {
                    _draggingDirection = CharacterObject.MoveDirection.None;
                }
            }
            else if (Input.GetKeyUp (KeyCode.A) || Input.GetKeyUp (KeyCode.D) || Input.GetKeyUp (KeyCode.S))
            {
                _dragging = false;
            }
#endif

            if (_dragging)
            {
                if (_draggingDirection == CharacterObject.MoveDirection.None)
                {
                    _playerCharacter.StateMachine.SetNextState (CharacterStateMachine.State.Guard);
                }
                else
                {
                    _playerCharacter.StateMachine.SetNextState (CharacterStateMachine.State.Run);
                    _playerCharacter.SetMoveDirection (_draggingDirection);
                }
            }
            else
            {
                _playerCharacter.StateMachine.SetNextState (CharacterStateMachine.State.Idle);
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

                if (Mathf.Abs (eventData.delta.x) * -2.0f > eventData.delta.y)
                {
                    _draggingDirection = CharacterObject.MoveDirection.None;
                }
                else
                {
                    _draggingDirection = eventData.delta.x > 0 ? CharacterObject.MoveDirection.Right : CharacterObject.MoveDirection.Left;
                }
            }
        }

        void IPointerDownHandler.OnPointerDown (PointerEventData eventData)
        {

        }

        void IPointerUpHandler.OnPointerUp (PointerEventData eventData)
        {
            if (_playerCharacter == null)
            {
                return;
            }

            _dragging = false;
            _playerCharacter.SetMoveDirection (CharacterObject.MoveDirection.None);
        }
    }
}