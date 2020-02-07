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
                case GameSceneType.Ingame:
                    return "Assets/App/AddressableAssets/Prefabs/GameScene/IngameScene.prefab";
                case GameSceneType.Title:
                    return "Assets/App/AddressableAssets/Prefabs/GameScene/TitleScene.prefab";
            }
            throw new ArgumentOutOfRangeException ("Undefined GameSceneType was found. " + type.ToString ());
        }
    }
}