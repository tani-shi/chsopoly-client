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

        private float _moveSpeed;
        private float _moveVelocity;
        private Rigidbody2D _rigidbody;

        public override void Initialize (uint id)
        {
            base.Initialize (id);

            _rigidbody = GetComponent<Rigidbody2D> ();

            var data = MasterDataManager.Instance.Get<CharacterDAO> ().Get (id);
            _moveSpeed = data.moveSpeed;
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
            StateMachine.SetNextState (CharacterStateMachine.State.Idle);
        }

        public void SetStateRun ()
        {
            StateMachine.SetNextState (CharacterStateMachine.State.Run);
        }

        public void MovePosition ()
        {
            _rigidbody.position += new Vector2 (_moveVelocity, 0);
        }

        protected override void Update ()
        {
            base.Update ();
            _moveVelocity = 0f;
        }

        void IIngameLoadCompleteEvent.OnIngameLoadComplete ()
        {
            var obj = GameObject.FindWithTag (IngameSettings.StartPointTag);
            if (obj != null)
            {
                transform.position = obj.transform.position;
            }
        }
    }
}