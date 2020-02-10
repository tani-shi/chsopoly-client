using Chsopoly.MasterData.Type;

namespace Chsopoly.GameScene.Ingame
{
    public static class IngameSettings
    {
        public static class Rules
        {
            public const int MaxPlayerCount = 4;
        }

        public static class Tags
        {
            public const string StartPoint = "StartPoint";
            public const string GoalPoint = "GoalPoint";
            public const string Ground = "Ground";
        }

        public static class Paths
        {
            public static string CharacterPrefab (string assetName)
            {
                return string.Format ("Assets/App/AddressableAssets/Prefabs/Character/{0}.prefab", assetName);
            }

            public static string CharacterAnimator (string assetName)
            {
                return string.Format ("Assets/App/AddressableAssets/Animations/Character/{0}.controller", assetName);
            }

            public static string FieldPrefab (string assetName)
            {
                return string.Format ("Assets/App/AddressableAssets/Prefabs/Field/{0}.prefab", assetName);
            }
        }

        public static class Character
        {
            public const float FootHeight = 1.0f;
            public const float JumpIntervalTime = 0.5f;
        }

        public static class Field
        {
            public static float Gravity (FieldGravity gravity)
            {
                switch (gravity)
                {
                    case FieldGravity.Normal:
                        return -9.81f;
                    default:
                        return 0f;
                }
            }
        }
    }
}