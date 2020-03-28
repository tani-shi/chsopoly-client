// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using Chsopoly.BaseSystem.UserData;

namespace Chsopoly.UserData.Entity
{
    [Serializable]
    public partial class Character : IUserDataEntity
    {
        public int Id { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDelete { get; set; }
        public bool IsNew { get; set; }

        public Character ()
        {
            IsNew = true;
        }

        public Character (bool isNew)
        {
            IsNew = isNew;
        }
    }
}