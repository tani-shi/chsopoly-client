using System;

namespace Chsopoly.GameScene.Ingame.Object
{
    public interface IObjectState<STATE, ANIM, OBJ> where STATE : struct, IConvertible where ANIM : struct, IConvertible where OBJ : BaseObject<STATE, ANIM, OBJ>
    {
        void OnEnter (OBJ owner);
        void OnUpdate (OBJ owner);
        void OnComplete (OBJ owner);
        void OnExit (OBJ owner);
    }
}