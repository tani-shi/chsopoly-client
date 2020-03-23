using Chsopoly.BaseSystem.UserData;

namespace Chsopoly.UserData.Entity
{
    public class Character : IUserDataEntity
    {
        public int Id { get; set; }
        public uint CharacterId { get; set; }
    }
}