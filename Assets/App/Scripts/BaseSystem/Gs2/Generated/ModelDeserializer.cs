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
                case 4001559680:
                    return new Chsopoly.Gs2.Models.CharacterObjectJump ().Deserialize (data);
                case 3384183465:
                    return new Chsopoly.Gs2.Models.CharacterObjectLand ().Deserialize (data);
                case 502460492:
                    return new Chsopoly.Gs2.Models.CharacterObjectMove ().Deserialize (data);
                case 2559275900:
                    return new Chsopoly.Gs2.Models.Profile ().Deserialize (data);
                default:
                    return null;
            }
        }
    }
}