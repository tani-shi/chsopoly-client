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

        public bool CanJumpAgain
        {
            get
            {
                return _jumpCounter < _jumpMaxCount && StateMachine.StateElapsedTime > IngameSettings.Character.JumpIntervalTime;
            }
        }

        private float _moveSpeed;
        private float _moveVelocity;
        private float _jumpHeight;
        private int _jumpMaxCount;
        private Rigidbody2D _rigidbody;
        private int _jumpCounter;

        public override void Initialize (uint id)
        {
            base.Initialize (id);

            _rigidbody = GetComponent<Rigidbody2D> ();

            var data = MasterDataManager.Instance.Get<CharacterDAO> ().Get (id);
            _moveSpeed = data.moveSpeed;
            _jumpHeight = data.jumpHeight;
            _jumpMaxCount = data.jumpMaxCount;
        }

        public void SetMoveVelocity (bool direction)
        {
            _moveVelocity = _moveSpeed * Time.deltaTime * (direction ? 1.0f : -1.0f);

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

        public void MovePosition ()
        {
            _rigidbody.position += new Vector2 (_moveVelocity, 0);
        }

        public void Jump ()
        {
            _rigidbody.velocity = new Vector2 (0, Mathf.Sqrt (-2.0f * Physics2D.gravity.y * _jumpHeight));
            _jumpCounter++;
        }

        public void ResetJumpCounter ()
        {
            _jumpCounter = 0;
        }

        protected override void FixedUpdate ()
        {
            base.FixedUpdate ();
            _moveVelocity = 0f;
        }

        void OnTriggerStay2D (Collider2D collision)
        {
            if (StateMachine.CurrentState == CharacterStateMachine.State.Jump)
            {
                ResetJumpCounter ();
                SetStateIdle ();
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