// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using UnityEngine;
using UnityMasterData.Interfaces;
using Chsopoly.MasterData.Type;

namespace Chsopoly.MasterData.VO.Ingame
{
    [SerializableAttribute]
    public partial class StageVO : IValueObject<uint>
    {
        public uint id;
        public string fieldName;
        public FieldGravity fieldGravity;
        public int limitTime;

        public uint GetKey ()
        {
            return id;
        }
    }
}
