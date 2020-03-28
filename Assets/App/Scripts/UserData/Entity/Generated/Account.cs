// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using Chsopoly.BaseSystem.UserData;

namespace Chsopoly.UserData.Entity
{
    [Serializable]
    public partial class Account : IUserDataEntity
    {
        public int Id
        {
            get
            {
                return 1;
            }
            set { }
        }

        public bool IsDirty { get; set; }
        public bool IsDelete { get; set; }
        public bool IsNew { get; set; }

        public Account ()
        {
            IsNew = true;
        }

        public Account (bool isNew)
        {
            IsNew = isNew;
        }
    }
}