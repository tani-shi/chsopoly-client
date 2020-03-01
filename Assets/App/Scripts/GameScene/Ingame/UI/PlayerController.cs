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
        private float _lastPointDownTime = 0f;
        private Vector2 _lastPointDownPosition = Vector2.zero;

        public void SetPlayer (CharacterObject playerCharacter)
        {
            _playerCharacter = playerCharacter;
        }

        public void OnClickJump ()
        {
            _playerCharacter.StateMachine.SetNextState (CharacterStateMachine.State.Jump);
        }

        void IDragHandler.OnDrag (PointerEventData eventData)
        {
            if (_playerCharacter == null)
            {
                return;
            }

            if (eventData.delta.sqrMagnitude > _dragSpeedThreshold)
            {
                if (_playerCharacter.StateMachine.CurrentState == CharacterStateMachine.State.Idle ||
                    _playerCharacter.StateMachine.CurrentState == CharacterStateMachine.State.Fall ||
                    _playerCharacter.StateMachine.CurrentState == CharacterStateMachine.State.Run ||
                    _playerCharacter.StateMachine.CurrentState == CharacterStateMachine.State.Jump)
                {
                    _playerCharacter.StateMachine.SetNextState (CharacterStateMachine.State.Run);
                    _playerCharacter.SetMoveDirection (eventData.delta.x > 0 ? CharacterObject.MoveDirection.Right : CharacterObject.MoveDirection.Left);
                }
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

            _playerCharacter.SetMoveDirection (CharacterObject.MoveDirection.None);
        }
    }
}