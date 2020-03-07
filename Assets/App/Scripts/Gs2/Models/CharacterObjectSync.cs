using Chsopoly.BaseSystem.Gs2;

namespace Chsopoly.Gs2.Models
{
    public partial class CharacterObjectSync : Gs2PacketModel
    {
        public int direction;
        public int state;
        public float x;
        public float y;
        public uint targetGimmickConnectionId;
        public int targetGimmickUniqueId;
    }
}