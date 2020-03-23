// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using Chsopoly.GameScene;

namespace Chsopoly.BaseSystem.GameScene
{
    public static class GameSceneTypeHelper
    {
        public static string GetAssetPath (GameSceneType type)
        {
            switch (type)
            {
                case GameSceneType.Ingame:
                    return "Assets/App/AddressableAssets/Prefabs/GameScene/IngameScene.prefab";
                case GameSceneType.Matching:
                    return "Assets/App/AddressableAssets/Prefabs/GameScene/MatchingScene.prefab";
                case GameSceneType.Mypage:
                    return "Assets/App/AddressableAssets/Prefabs/GameScene/MypageScene.prefab";
                case GameSceneType.Title:
                    return "Assets/App/AddressableAssets/Prefabs/GameScene/TitleScene.prefab";
                default:
                    throw new ArgumentOutOfRangeException ("Undefined GameSceneType was specified. " + type.ToString ());
            }
        }

        public static string GetLoaderAssetPath (GameSceneType type)
        {
            switch (type)
            {

                default:
                    throw new ArgumentOutOfRangeException ("Undefined GameSceneType was specified. " + type.ToString ());
            }
        }

        public static bool HasLoader (GameSceneType type)
        {
            switch (type)
            {

                default: return false;
            }
        }
    }
}