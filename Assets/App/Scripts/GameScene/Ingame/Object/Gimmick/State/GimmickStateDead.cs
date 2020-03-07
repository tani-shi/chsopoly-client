namespace Chsopoly.GameScene.Ingame.Object.Gimmick.State
{
    public class GimmickStateDead : IObjectState<GimmickStateMachine.State, GimmickObjectModel.Animation, GimmickObject>
    {
        void IObjectState<GimmickStateMachine.State, GimmickObjectModel.Animation, GimmickObject>.OnEnter (GimmickObject owner)
        {
            owner.StateMachine.SetStateTimerInfinite ();
        }

        void IObjectState<GimmickStateMachine.State, GimmickObjectModel.Animation, GimmickObject>.OnUpdate (GimmickObject owner)
        {

        }

        void IObjectState<GimmickStateMachine.State, GimmickObjectModel.Animation, GimmickObject>.OnComplete (GimmickObject owner)
        {

        }

        void IObjectState<GimmickStateMachine.State, GimmickObjectModel.Animation, GimmickObject>.OnExit (GimmickObject owner)
        {

        }
    }
}