using Google.Protobuf;

namespace Chsopoly.BaseSystem.Gs2
{
    public abstract class Gs2PacketModel
    {
        public virtual ByteString Serialize ()
        {
            return null;
        }
    }
}