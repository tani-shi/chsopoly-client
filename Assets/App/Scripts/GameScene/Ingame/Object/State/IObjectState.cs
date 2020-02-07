using System;

namespace Chsopoly.GameScene.Ingame.Object.State
{
    public interface IObjectState<STATE, ANIM, OBJ> where STATE : struct, IConvertible where ANIM : struct, IConvertible where OBJ : BaseObject<STATE, ANIM, OBJ>
    {
        void OnEnter (OBJ owner);
        void OnExecute (OBJ owner);
        void OnComplete (OBJ owner);
        void OnExit (OBJ owner);
    }
}