using System;
using System.Collections.Generic;
using System.Linq;
using Chsopoly.BaseSystem.Gs2;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Event;
using Chsopoly.GameScene.Ingame.Object.Gimmick;
using Chsopoly.Gs2.Models;
using Chsopoly.Libs.Extensions;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Character
{
    public class CharacterObject : BaseObject<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>
    {
        public event Action<MoveDirection> onChangedMoveDirection;
        public event Action<Status, int> onStatusBegan;
        public event Action<Status, int> onStatusUpdated;
        public event Action<Status> onStatusEnded;

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

        public int Power
        {
            get
            {
                return _power;
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
                var layerMask = LayerMask.GetMask (LayerMask.LayerToName (IngameSettings.Layers.Field), LayerMask.LayerToName (IngameSettings.Layers.Gimmick));
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

        public GimmickObject TargetGimmick
        {
            get
            {
                return _targetGimmick;
            }
            set
            {
                _targetGimmick = value;
            }
        }

        public Rect ColliderRect
        {
            get
            {
                return new Rect (
                    _transform.position.x + _collider.offset.x - _collider.size.x.Half (),
                    _transform.position.y + _collider.offset.y - _collider.size.y.Half (),
                    _collider.size.x,
                    _collider.size.y
                );
            }
        }

        public int Hp
        {
            get
            {
                return _hp;
            }
            set
            {
                _hp = value;
            }
        }

        public int MaxHp
        {
            get
            {
                return _maxHp;
            }
        }

        public enum MoveDirection
        {
            None,
            Right,
            Left,
        }

        public enum Status
        {
            None,
            Invincible,
        }

        private float _moveSpeed = 0;
        private float _jumpHeight = 0;
        private int _aerialJumpCount = 0;
        private int _aerialJumpCounter = 0;
        private int _power = 0;
        private bool _isLanded = true;
        private uint _connectionId = 0;
        private Rigidbody2D _rigidbody = null;
        private BoxCollider2D _collider = null;
        private Transform _transform = null;
        private MoveDirection _direction = MoveDirection.None;
        private GimmickObject _targetGimmick = null;
        private int _hp = 0;
        private int _maxHp = 0;
        private Dictionary<Status, int> _statusTimer = new Dictionary<Status, int> ();

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
            _power = data.power;
            _hp = data.hp;
            _maxHp = data.hp;
        }

        public void SetIdentity (uint connectionId)
        {
            _connectionId = connectionId;
        }

        public void SetMoveDirection (MoveDirection direction)
        {
            if (StateMachine.CurrentState != CharacterStateMachine.State.Idle &&
                StateMachine.CurrentState != CharacterStateMachine.State.Jump &&
                StateMachine.CurrentState != CharacterStateMachine.State.Fall &&
                StateMachine.CurrentState != CharacterStateMachine.State.Run &&
                StateMachine.CurrentState != CharacterStateMachine.State.Guard)
            {
                return;
            }

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

        public void Damage (int damage)
        {
            if (StateMachine.SetNextState (CharacterStateMachine.State.Damage))
            {
                _hp = Mathf.Max (0, _hp - damage);
            }
        }

        public bool HasStatus (Status status)
        {
            return _statusTimer.ContainsKey (status);
        }

        public void SetStatus (Status status, int timer)
        {
            if (!_statusTimer.ContainsKey (status))
            {
                _statusTimer.Add (status, timer);
                onStatusBegan.SafeInvoke (status, timer);
            }
            else if (_statusTimer[status] < timer)
            {
                _statusTimer[status] = timer;
                onStatusUpdated.SafeInvoke (status, timer);
            }
        }

        protected override void FixedUpdate ()
        {
            base.FixedUpdate ();

            UpdateStatusTimer ();
        }

        void OnTriggerEnter2D (Collider2D collision)
        {
            if (collision.gameObject.tag == IngameSettings.Tags.GoalPoint)
            {
                StateMachine.SetNextState (CharacterStateMachine.State.Appeal);
            }
        }

        private void UpdateStatusTimer ()
        {
            foreach (var status in _statusTimer.Keys.ToArray ())
            {
                _statusTimer[status]--;
                if (_statusTimer[status] <= 0)
                {
                    _statusTimer.Remove (status);
                    onStatusEnded.SafeInvoke (status);
                }
            }
        }
    }
}