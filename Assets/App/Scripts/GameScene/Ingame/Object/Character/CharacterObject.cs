using Chsopoly.BaseSystem.Gs2;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Event;
using Chsopoly.Gs2.Models;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Character
{
    public class CharacterObject : BaseObject<CharacterStateMachine.State, CharacterObjectModel.Animation, CharacterObject>, IIngameLoadCompleteEvent
    {
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
                return _moveVelocity;
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
                if (IsLanding || (StateMachine.CurrentState != CharacterStateMachine.State.Jump && _aerialJumpCounter < _aerialJumpCount))
                {
                    return true;
                }
                return _aerialJumpCounter < _aerialJumpCount && StateMachine.StateElapsedTime > IngameSettings.Character.JumpIntervalTime;
            }
        }

        public bool IsLanding
        {
            get
            {
                return _isLanding;
            }
        }

        public Vector2 worldPosition
        {
            get
            {
                return _transform.position;
            }
        }

        private float _moveSpeed = 0;
        private float _moveVelocity = 0;
        private float _jumpHeight = 0;
        private int _aerialJumpCount = 0;
        private int _aerialJumpCounter = 0;
        private bool _isLanding = false;
        private Rigidbody2D _rigidbody = null;
        private Transform _transform = null;
        private bool _isPlayer = false;
        private bool _direction = true;

        public override void Initialize (uint id)
        {
            base.Initialize (id);

            _rigidbody = GetComponent<Rigidbody2D> ();
            _transform = transform;

            var data = MasterDataManager.Instance.Get<CharacterDAO> ().Get (id);
            _moveSpeed = data.moveSpeed;
            _jumpHeight = data.jumpHeight;
            _aerialJumpCount = data.aerialJumpCount;
        }

        public void SetPlayerCharacter (bool isPlayer)
        {
            _isPlayer = isPlayer;
        }

        public void SetPositionToStartPoint ()
        {
            var obj = GameObject.FindWithTag (IngameSettings.Tags.StartPoint);
            if (obj != null)
            {
                _transform.position = obj.transform.position;
                _rigidbody.velocity = Vector2.zero;
            }
        }

        public bool Idle ()
        {
            return StateMachine.SetNextState (CharacterStateMachine.State.Idle, this);
        }

        public bool Move (bool direction)
        {
            if (SetMoveVelocity (direction))
            {
                StateMachine.SetNextState (CharacterStateMachine.State.Run, this);
                return true;
            }
            return false;
        }

        public bool Appeal ()
        {
            return StateMachine.SetNextState (CharacterStateMachine.State.Appeal, this);
        }

        public bool Jump (Vector2 position, bool force = false)
        {
            if (StateMachine.SetNextState (CharacterStateMachine.State.Jump, this, force))
            {
                _transform.position = position;
                _rigidbody.velocity = new Vector2 ((_moveVelocity * Time.deltaTime) + _rigidbody.velocity.x, Mathf.Sqrt (-2.0f * Physics2D.gravity.y * _jumpHeight));

                if (!_isLanding)
                {
                    _aerialJumpCounter++;
                }

                _isLanding = false;
                return true;
            }
            return false;
        }

        public bool Land (Vector2 position)
        {
            Idle ();

            _isLanding = true;
            _rigidbody.velocity = new Vector2 (_rigidbody.velocity.x, 0);
            _transform.position = position;
            _aerialJumpCounter = 0;

            return true;
        }

        protected override void FixedUpdate ()
        {
            base.FixedUpdate ();

            if (_isLanding)
            {
                _rigidbody.MovePosition (_rigidbody.position + new Vector2 (_moveVelocity * Time.deltaTime, 0));
            }
            else
            {
                _rigidbody.velocity = new Vector2 (_moveVelocity, _rigidbody.velocity.y);
            }

            _moveVelocity = 0f;
        }

        private bool SetMoveVelocity (bool direction)
        {
            if (StateMachine.CurrentState == CharacterStateMachine.State.Dead ||
                StateMachine.CurrentState == CharacterStateMachine.State.Squat ||
                StateMachine.CurrentState == CharacterStateMachine.State.Appeal)
            {
                return false;
            }

            _moveVelocity = _moveSpeed * (direction ? 1.0f : -1.0f);
            _direction = direction;

            if (direction)
            {
                _transform.localScale = Vector2.one;
            }
            else
            {
                _transform.localScale = new Vector2 (-1, 1);
            }

            return true;
        }

        void OnTriggerEnter2D (Collider2D collision)
        {
            if (collision.gameObject.tag == IngameSettings.Tags.GoalPoint)
            {
                Appeal ();
            }
            else if (!_isLanding && _isPlayer)
            {
                if (Land (worldPosition))
                {
                    var packet = new CharacterObjectLand ()
                    {
                        x = worldPosition.x,
                        y = worldPosition.y,
                    };
                    Gs2Manager.Instance.SendRelayMessage (packet);
                }
            }
        }

        void OnTriggerExit2D (Collider2D collision)
        {
            _isLanding = false;
        }

        void IIngameLoadCompleteEvent.OnIngameLoadComplete ()
        {
            SetPositionToStartPoint ();
        }
    }
}