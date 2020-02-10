using System;

namespace Chsopoly.GameScene.Ingame.Object.Gimmick
{
    public abstract class BaseGimmickObject<STATE, ANIM, OBJ> : BaseObject<STATE, ANIM, OBJ>, IGimmickObject where STATE : struct, IConvertible where ANIM : struct, IConvertible where OBJ : BaseGimmickObject<STATE, ANIM, OBJ>
    {
        
    }
}