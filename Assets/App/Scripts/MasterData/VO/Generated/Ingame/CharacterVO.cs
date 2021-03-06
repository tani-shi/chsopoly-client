// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using UnityEngine;
using UnityMasterData.Interfaces;
using Chsopoly.MasterData.Type;

namespace Chsopoly.MasterData.VO.Ingame
{
    [SerializableAttribute]
    public partial class CharacterVO : IValueObject<uint>
    {
        public uint id;
        public string assetName;
        public float moveSpeed;
        public float weight;
        public float jumpHeight;
        public int aerialJumpCount;
        public int power;
        public int hp;

        public uint GetKey ()
        {
            return id;
        }
    }
}
