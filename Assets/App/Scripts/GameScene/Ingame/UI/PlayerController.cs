using Chsopoly.BaseSystem.Gs2;
using Chsopoly.GameScene.Components.Button;
using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.Gs2.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Ingame.UI
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float _dragSpeedThreshold = 10f;

        private enum Mode
        {
            None,
            ControlPlayer,
            DestroyGimmick,
        }

        private CharacterObject _playerCharacter = null;
        private bool _dragging = false;
        private CharacterObject.MoveDirection _draggingDirection = CharacterObject.MoveDirection.None;
        private bool _guarding = false;
        private Mode _mode = Mode.None;

        public void SetPlayer (CharacterObject playerCharacter)
        {
            _playerCharacter = playerCharacter;
            _mode = Mode.ControlPlayer;
        }

        public void OnClickJump ()
        {
            _playerCharacter.StateMachine.SetNextState (CharacterStateMachine.State.Jump);
        }

        public void OnPressDownGuard ()
        {
            _guarding = true;
        }

        public void OnPressUpGuard ()
        {
            _guarding = false;
        }

        public void OnDragController (PointerEventData eventData)
        {
            if (_playerCharacter == null || _mode != Mode.ControlPlayer)
            {
                return;
            }

            if (eventData.delta.sqrMagnitude > _dragSpeedThreshold)
            {
                _dragging = true;
                _draggingDirection = eventData.delta.x > 0 ? CharacterObject.MoveDirection.Right : CharacterObject.MoveDirection.Left;
            }
        }

        public void OnPointerUpController (PointerEventData eventData)
        {
            _dragging = false;
        }

        void Update ()
        {
            if (_playerCharacter == null)
            {
                return;
            }

            switch (_mode)
            {
                case Mode.ControlPlayer:
                    ControlPlayer ();
                    break;
                case Mode.DestroyGimmick:
                    break;
            }
        }

        private void ControlPlayer ()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown (KeyCode.Space))
            {
                _playerCharacter.StateMachine.SetNextState (CharacterStateMachine.State.Jump);
            }
            if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.D))
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
            }
            else if (Input.GetKeyUp (KeyCode.A) || Input.GetKeyUp (KeyCode.D))
            {
                _dragging = false;
            }
#endif

            if (_guarding)
            {
                _playerCharacter.StateMachine.SetNextState (CharacterStateMachine.State.Guard);
            }
            else if (_dragging)
            {
                _playerCharacter.StateMachine.SetNextState (CharacterStateMachine.State.Run);
                _playerCharacter.SetMoveDirection (_draggingDirection);
            }
            else
            {
                _playerCharacter.StateMachine.SetNextState (CharacterStateMachine.State.Idle);
            }
        }
    }
}