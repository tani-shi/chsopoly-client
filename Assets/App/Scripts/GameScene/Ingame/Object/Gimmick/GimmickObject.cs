using System;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Gimmick
{
    public class GimmickObject : BaseObject<GimmickStateMachine.State, GimmickObjectModel.Animation, GimmickObject>
    {
        private Transform _transform = null;

        public override void Initialize (uint id)
        {
            base.Initialize (id);

            _transform = transform;
        }
    }
}