// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using Chsopoly.Popup;

namespace Chsopoly.BaseSystem.Popup
{
    public static class PopupTypeHelper
    {
        public static string GetAssetPath (PopupType type)
        {
            switch (type)
            {
                case PopupType.GimmickList:
                    return "Assets/App/AddressableAssets/Prefabs/Popup/GimmickListPopup.prefab";
                case PopupType.Message:
                    return "Assets/App/AddressableAssets/Prefabs/Popup/MessagePopup.prefab";
                default:
                    throw new ArgumentOutOfRangeException ("Undefined PopupType was specified. " + type.ToString ());
            }
        }
    }
}