using Chsopoly.BaseSystem.Effect;
using Chsopoly.Effect;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Gimmick.State
{
    public class GimmickStateDamage : IObjectState<GimmickStateMachine.State, GimmickObjectModel.Animation, GimmickObject>
    {
        void IObjectState<GimmickStateMachine.State, GimmickObjectModel.Animation, GimmickObject>.OnEnter (GimmickObject owner)
        {
            owner.StateMachine.SetStateTimer (Application.targetFrameRate);

            EffectManager.Instance.Play (
                Eff.TapGimmick,
                parent : owner.transform);
        }

        void IObjectState<GimmickStateMachine.State, GimmickObjectModel.Animation, GimmickObject>.OnUpdate (GimmickObject owner)
        {

        }

        void IObjectState<GimmickStateMachine.State, GimmickObjectModel.Animation, GimmickObject>.OnComplete (GimmickObject owner)
        {
            owner.StateMachine.SetNextState (GimmickStateMachine.State.Idle);
        }

        void IObjectState<GimmickStateMachine.State, GimmickObjectModel.Animation, GimmickObject>.OnExit (GimmickObject owner)
        {

        }
    }
}