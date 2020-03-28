namespace Chsopoly.BaseSystem.UserData
{
    public interface IUserDataEntity
    {
        int Id { get; set; }
        bool IsDirty { get; set; }
        bool IsDelete { get; set; }
        bool IsNew { get; set; }
    }
}