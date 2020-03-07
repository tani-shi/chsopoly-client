using Chsopoly.MasterData.Type;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame
{
    public static class IngameSettings
    {
        public static class Gs2
        {
            public const uint PlayerConnectionId = 0;
        }

        public static class Rules
        {
            public const int MaxPlayerCount = 4;
            public const int MaxGimmickQueueCount = 4;
            public const int MaxGimmickBoxCount = 3;
            public const int GameSetPoint = 2;
        }

        public static class Layers
        {
            public const int UI = 5;
            public const int Field = 8;
            public const int Gimmick = 9;
            public const int Character = 10;
        }

        public static class Tags
        {
            public const string StartPoint = "StartPoint";
            public const string GoalPoint = "GoalPoint";
        }

        public static class Paths
        {
            public static string CharacterPrefab (string assetName)
            {
                return string.Format ("Assets/App/AddressableAssets/Prefabs/Character/{0}.prefab", assetName);
            }

            public static string FieldPrefab (string assetName)
            {
                return string.Format ("Assets/App/AddressableAssets/Prefabs/Field/{0}.prefab", assetName);
            }

            public static string GimmickPrefab (string assetName)
            {
                return string.Format ("Assets/App/AddressableAssets/Prefabs/Gimmick/{0}.prefab", assetName);
            }

            public static string GimmickIcon (string assetName)
            {
                return string.Format ("Assets/App/AddressableAssets/Textures/Capture/Gimmick/{0}.png", assetName);
            }
        }

        public static class Physics
        {
            public static PhysicsMaterial2D DefaultMaterial
            {
                get
                {
                    var material = new PhysicsMaterial2D ();
                    material.bounciness = 0;
                    material.friction = 0;
                    return material;
                }
            }
        }

        public static class Character
        {
            public const float JumpIntervalTime = 0.5f;
        }

        public static class Field
        {
            public const float WallSize = 50f;

            public static float Gravity (FieldGravity gravity)
            {
                var g = Physics2D.gravity.y;
                switch (gravity)
                {
                    case FieldGravity.Normal:
                        g = -9.81f;
                        break;
                }
                return g * 100.0f; // Pixels per unit.
            }
        }

        public static class Gimmick
        {
            public static readonly Color DraggingColor = new Color (1, 1, 1, 0.5f);
        }
    }
}