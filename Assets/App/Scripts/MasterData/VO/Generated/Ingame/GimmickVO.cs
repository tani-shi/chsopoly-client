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
        public int hitPoint;
        public float coolTime;
        public int damage;

        public uint GetKey ()
        {
            return id;
        }
    }
}
