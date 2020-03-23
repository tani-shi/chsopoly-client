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
                case PopupType.MessagePopup:
                    return "Assets/App/AddressableAssets/Prefabs/Popup/MessagePopup.prefab";
                default:
                    throw new ArgumentOutOfRangeException ("Undefined PopupType was specified. " + type.ToString ());
            }
        }
    }
}