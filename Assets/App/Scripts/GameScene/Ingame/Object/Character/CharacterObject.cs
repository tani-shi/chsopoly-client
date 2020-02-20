using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Event;
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

        private float _moveSpeed = 0;
        private float _moveVelocity = 0;
        private float _jumpHeight = 0;
        private int _aerialJumpCount = 0;
        private int _aerialJumpCounter = 0;
        private bool _isLanding = false;
        private Rigidbody2D _rigidbody = null;
        private Transform _transform = null;

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

        public void SetMoveVelocity (bool direction)
        {
            if (StateMachine.CurrentState == CharacterStateMachine.State.Dead ||
                StateMachine.CurrentState == CharacterStateMachine.State.Squat ||
                StateMachine.CurrentState == CharacterStateMachine.State.Appeal)
            {
                return;
            }

            _moveVelocity = _moveSpeed * (direction ? 1.0f : -1.0f);

            if (direction)
            {
                _transform.localScale = Vector2.one;
            }
            else
            {
                _transform.localScale = new Vector2 (-1, 1);
            }
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

        public void SetStateIdle ()
        {
            StateMachine.SetNextState (CharacterStateMachine.State.Idle, this);
        }

        public void SetStateRun ()
        {
            StateMachine.SetNextState (CharacterStateMachine.State.Run, this);
        }

        public void SetStateJump ()
        {
            StateMachine.SetNextState (CharacterStateMachine.State.Jump, this);
        }

        public void SetStateAppeal ()
        {
            StateMachine.SetNextState (CharacterStateMachine.State.Appeal, this);
        }

        public void Jump ()
        {
            _rigidbody.velocity = new Vector2 ((_moveVelocity * Time.deltaTime) + _rigidbody.velocity.x, Mathf.Sqrt (-2.0f * Physics2D.gravity.y * _jumpHeight));

            if (!_isLanding)
            {
                _aerialJumpCounter++;
            }

            _isLanding = false;
        }

        public void ResetAerialJumpCounter ()
        {
            _aerialJumpCounter = 0;
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

        void OnTriggerEnter2D (Collider2D collision)
        {
            if (collision.gameObject.tag == IngameSettings.Tags.GoalPoint)
            {
                SetStateAppeal ();
            }
            else if (!_isLanding)
            {
                _isLanding = true;
                _rigidbody.velocity = new Vector2 (_rigidbody.velocity.x, 0);
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