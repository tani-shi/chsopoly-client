// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using Google.Protobuf;

namespace Chsopoly.BaseSystem.Gs2
{
    public static class ModelDeserializer
    {
        public static Gs2PacketModel Deserialize (ByteString data)
        {
            switch (BitConverter.ToUInt32 (data.ToByteArray (), 0))
            {
                case 2415485155:
                    return new Chsopoly.Gs2.Models.CharacterObjectSync ().Deserialize (data);
                case 3047453915:
                    return new Chsopoly.Gs2.Models.GimmickObjectPut ().Deserialize (data);
                case 2559275900:
                    return new Chsopoly.Gs2.Models.Profile ().Deserialize (data);
                default:
                    return null;
            }
        }
    }
}