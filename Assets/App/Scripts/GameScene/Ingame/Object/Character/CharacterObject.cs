using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Event;
using Chsopoly.GameScene.Ingame.Object.State.Character;
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

        private float _moveSpeed;
        private float _moveVelocity;
        private float _jumpHeight;
        private int _aerialJumpCount;
        private Rigidbody2D _rigidbody;
        private int _aerialJumpCounter;
        private bool _isLanding;

        public override void Initialize (uint id)
        {
            base.Initialize (id);

            _rigidbody = GetComponent<Rigidbody2D> ();

            var data = MasterDataManager.Instance.Get<CharacterDAO> ().Get (id);
            _moveSpeed = data.moveSpeed;
            _jumpHeight = data.jumpHeight;
            _aerialJumpCount = data.aerialJumpCount;
        }

        public void SetMoveVelocity (bool direction)
        {
            _moveVelocity = _moveSpeed * (direction ? 1.0f : -1.0f);

            if (direction)
            {
                transform.localScale = Vector2.one;
            }
            else
            {
                transform.localScale = new Vector2 (-1, 1);
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

        public void Jump ()
        {
            _rigidbody.velocity = new Vector2 (0, Mathf.Sqrt (-2.0f * Physics2D.gravity.y * _jumpHeight));

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

            _rigidbody.velocity = new Vector2 (_moveVelocity, _rigidbody.velocity.y);
            _moveVelocity = 0f;
        }

        void OnCollisionEnter2D (Collision2D collision)
        {
            _isLanding = Physics2D.Linecast (transform.position, transform.position + Vector3.down);
        }

        void OnCollisionExit2D (Collision2D collision)
        {
            if (_isLanding && !Physics2D.Linecast (transform.position + Vector3.down, transform.position + Vector3.down * 2.0f))
            {
                _isLanding = false;
            }
        }

        void IIngameLoadCompleteEvent.OnIngameLoadComplete ()
        {
            var obj = GameObject.FindWithTag (IngameSettings.Tags.StartPoint);
            if (obj != null)
            {
                transform.position = obj.transform.position;
            }
        }
    }
}