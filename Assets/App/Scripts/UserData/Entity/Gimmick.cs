using Chsopoly.BaseSystem.UserData;

namespace Chsopoly.UserData.Entity
{
    public class Gimmick : IUserDataEntity
    {
        public int Id { get; set; }
        public uint GimmickId { get; set; }
        public bool IsActive { get; set; }
    }
}