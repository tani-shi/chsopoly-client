using Chsopoly.BaseSystem.UserData;

namespace Chsopoly.UserData.Entity
{
    public class Account : IUserDataEntity
    {
        public int Id { get; set; }
        public string Gs2AccountId { get; set; }
    }
}