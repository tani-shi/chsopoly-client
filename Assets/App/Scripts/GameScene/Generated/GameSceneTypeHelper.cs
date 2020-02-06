// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;

namespace Chsopoly.GameScene
{
    public static class GameSceneTypeHelper
    {
        public static string GetAssetPath (GameSceneType type)
        {
            switch (type)
            {
                case GameSceneType.Title:
                    return "Assets/App/AddressableAssets/GameScene/TitleScene.prefab";
            }
            throw new ArgumentOutOfRangeException ("Undefined GameSceneType was found. " + type.ToString ());
        }
    }
}