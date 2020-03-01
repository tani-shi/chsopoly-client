using System;
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

        private Transform _transform = null;
        private int _uniqueId = 0;
        private uint _connectionId = 0;

        public override void Initialize (uint id)
        {
            base.Initialize (id);

            _transform = transform;
        }

        public void SetIdentity (uint connectionId, int uniqueId)
        {
            _uniqueId = uniqueId;
            _connectionId = connectionId;
        }
    }
}