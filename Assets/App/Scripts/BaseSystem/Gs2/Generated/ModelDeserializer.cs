// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;

namespace Chsopoly.BaseSystem.Gs2
{
    public static class ModelDeserializer
    {
        public static IGs2PacketModel Deserialize (byte[] data)
        {
            switch (BitConverter.ToInt32 (data, 0))
            {
                case -1735691396:
                    return new Chsopoly.Gs2.Models.Profile ().Deserialize (data);
                default:
                    return null;
            }
        }
    }
}