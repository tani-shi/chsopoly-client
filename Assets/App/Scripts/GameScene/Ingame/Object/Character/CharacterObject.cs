using System;
using Chsopoly.BaseSystem.Gs2;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Event;
using Chsopoly.Gs2.Models;
using Chsopoly.Libs.Extensions;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Character
{
    public class CharacterObject : BaseObject<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>
    {
        public event Action<MoveDirection> onChangedMoveDirection;

        public bool IsPlayer
        {
            get
            {
                return _connectionId == IngameSettings.Gs2.PlayerConnectionId;
            }
        }

        public uint ConnectionId
        {
            get
            {
                return _connectionId;
            }
        }

        public float MoveSpeed
        {
            get
            {
                return _moveSpeed;
            }
        }

        public float MoveVelocity
        {
            get
            {
                if (_direction != MoveDirection.None)
                {
                    return _moveSpeed * (_direction == MoveDirection.Right ? 1 : -1);
                }
                return 0;
            }
        }

        public float JumpHeight
        {
            get
            {
                return _jumpHeight;
            }
        }

        public bool CanJump
        {
            get
            {
                if (IsLanded || (StateMachine.CurrentState != CharacterStateMachine.State.Jump && _aerialJumpCounter < _aerialJumpCount))
                {
                    return true;
                }
                return _aerialJumpCounter < _aerialJumpCount && StateMachine.StateElapsedTime > IngameSettings.Character.JumpIntervalTime;
            }
        }

        public bool IsGround
        {
            get
            {
                var layerMask = LayerMask.GetMask (nameof (IngameSettings.Layers.Field), nameof (IngameSettings.Layers.Gimmick));
                return Physics2D.Raycast (worldPosition + new Vector2 (_collider.offset.x * _transform.localScale.x, _collider.offset.y) + (new Vector2 (-(_collider.size.x - 10), -_collider.size.y) / 2.0f), Vector2.down, 1, layerMask) ||
                    Physics2D.Raycast (worldPosition + new Vector2 (_collider.offset.x * _transform.localScale.x, _collider.offset.y) + (new Vector2 ((_collider.size.x - 10), -_collider.size.y) / 2.0f), Vector2.down, 1, layerMask);
            }
        }

        public bool IsLanded
        {
            get
            {
                return _isLanded;
            }
            set
            {
                _isLanded = value;
            }
        }

        public Vector2 worldPosition
        {
            get
            {
                return _transform.position;
            }
            set
            {
                _transform.position = value;
            }
        }

        public MoveDirection Direction
        {
            get
            {
                return _direction;
            }
        }

        public BoxCollider2D Collider
        {
            get
            {
                return _collider;
            }
        }

        public Rigidbody2D Rigidbody
        {
            get
            {
                return _rigidbody;
            }
        }

        public enum MoveDirection
        {
            None,
            Right,
            Left,
        }

        private float _moveSpeed = 0;
        private float _jumpHeight = 0;
        private int _aerialJumpCount = 0;
        private int _aerialJumpCounter = 0;
        private bool _isLanded = false;
        private uint _connectionId = 0;
        private Rigidbody2D _rigidbody = null;
        private BoxCollider2D _collider = null;
        private Transform _transform = null;
        private MoveDirection _direction = MoveDirection.None;

        public override void Initialize (uint id)
        {
            base.Initialize (id);

            _rigidbody = GetComponent<Rigidbody2D> ();
            _collider = GetComponent<BoxCollider2D> ();
            _transform = transform;

            var data = MasterDataManager.Instance.Get<CharacterDAO> ().Get (id);
            _moveSpeed = data.moveSpeed;
            _jumpHeight = data.jumpHeight;
            _aerialJumpCount = data.aerialJumpCount;
        }

        public void SetIdentity (uint connectionId)
        {
            _connectionId = connectionId;
        }

        public void SetMoveDirection (MoveDirection direction)
        {
            if (direction != MoveDirection.None)
            {
                if (direction == MoveDirection.Right)
                {
                    _transform.localScale = Vector2.one;
                }
                else
                {
                    _transform.localScale = new Vector2 (-1, 1);
                }
            }

            var prevDirection = _direction;
            _direction = direction;

            if (prevDirection != direction)
            {
                onChangedMoveDirection.SafeInvoke (direction);
            }
        }

        public void CountAerialJump ()
        {
            _aerialJumpCounter++;
        }

        public void ResetAerialJumpCounter ()
        {
            _aerialJumpCounter = 0;
        }

        void OnTriggerEnter2D (Collider2D collision)
        {
            if (collision.gameObject.tag == IngameSettings.Tags.GoalPoint)
            {
                StateMachine.SetNextState (CharacterStateMachine.State.Appeal);
            }
        }
    }
}