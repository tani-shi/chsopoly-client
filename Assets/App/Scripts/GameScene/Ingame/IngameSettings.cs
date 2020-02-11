using Chsopoly.MasterData.Type;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame
{
    public static class IngameSettings
    {
        public static class Rules
        {
            public const int MaxPlayerCount = 4;
            public const int MaxGimmickCount = 5;
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
            public const float JumpIntervalTime = 0.5f;

            public static PhysicsMaterial2D PhysicsMaterial
            {
                get
                {
                    var material = new PhysicsMaterial2D ();
                    material.bounciness = 0.1f;
                    material.friction = 0;
                    return material;
                }
            }
        }

        public static class Field
        {
            public const float WallSize = 50f;

            public static PhysicsMaterial2D PhysicsMaterial
            {
                get
                {
                    var material = new PhysicsMaterial2D ();
                    material.bounciness = 0;
                    material.friction = 0;
                    return material;
                }
            }

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
    }
}