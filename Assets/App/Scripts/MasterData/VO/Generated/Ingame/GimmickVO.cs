// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using UnityEngine;
using UnityMasterData.Interfaces;
using Chsopoly.MasterData.Type;

namespace Chsopoly.MasterData.VO.Ingame
{
    [SerializableAttribute]
    public partial class GimmickVO : IValueObject<uint>
    {
        public uint id;
        public string assetName;
        public int lotteryWeight;

        public uint GetKey ()
        {
            return id;
        }
    }
}
