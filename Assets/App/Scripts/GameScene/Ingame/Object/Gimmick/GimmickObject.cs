using System;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Gimmick
{
    public class GimmickObject : BaseObject<GimmickStateMachine.State, GimmickObjectModel.Animation, GimmickObject>
    {
        public bool IsPlayer
        {
            get
            {
                return _connectionId == IngameSettings.Gs2.PlayerConnectionId;
            }
        }

        public int UniqueId
        {
            get
            {
                return _uniqueId;
            }
        }

        public uint ConnectionId
        {
            get
            {
                return _connectionId;
            }
        }

        public int MaxHitPoint
        {
            get
            {
                return _maxHitPoint;
            }
        }

        public int HitPoint
        {
            get
            {
                return _hitPoint;
            }
            set
            {
                _hitPoint = value;
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

        private Transform _transform = null;
        private int _uniqueId = 0;
        private uint _connectionId = 0;
        private int _maxHitPoint = 0;
        private int _hitPoint = 0;

        public override void Initialize (uint id)
        {
            base.Initialize (id);

            _transform = transform;

            var data = MasterDataManager.Instance.Get<GimmickDAO> ().Get (id);
            _maxHitPoint = data.hitPoint;
            _hitPoint = data.hitPoint;
        }

        public void SetIdentity (uint connectionId, int uniqueId)
        {
            _uniqueId = uniqueId;
            _connectionId = connectionId;
        }

        public void Damage (int value)
        {
            _hitPoint = Mathf.Max (0, _hitPoint - value);
            if (_hitPoint <= 0)
            {
                StateMachine.SetNextState (GimmickStateMachine.State.Dying);
            }
            else
            {
                StateMachine.SetNextState (GimmickStateMachine.State.Damage);
            }
        }
    }
}