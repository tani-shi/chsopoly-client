// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using Chsopoly.BaseSystem.UserData;

namespace Chsopoly.UserData.Entity
{
    [Serializable]
    public partial class Gimmick : IUserDataEntity
    {
        public int Id { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDelete { get; set; }
        public bool IsNew { get; set; }

        public Gimmick ()
        {
            IsNew = true;
        }

        public Gimmick (bool isNew)
        {
            IsNew = isNew;
        }
    }
}