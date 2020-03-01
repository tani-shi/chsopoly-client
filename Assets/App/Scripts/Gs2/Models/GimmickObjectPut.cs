using Chsopoly.BaseSystem.Gs2;

namespace Chsopoly.Gs2.Models
{
    public partial class GimmickObjectPut : Gs2PacketModel
    {
        public int uniqueId;
        public uint gimmickId;
        public float x;
        public float y;
    }
}