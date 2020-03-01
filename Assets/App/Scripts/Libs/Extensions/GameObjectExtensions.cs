using UnityEngine;

namespace Chsopoly.Libs.Extensions
{
    public static class GameObjectExtensions
    {
        public static bool HasComponent<T> (this GameObject self) where T : Component
        {
            return self.GetComponent<T> () != null;
        }

        public static T SafeAddComponent<T> (this GameObject self) where T : Component
        {
            var component = self.GetComponent<T> ();
            if (component != null)
            {
                return component;
            }
            return self.AddComponent<T> ();
        }

        public static void SetLayerRecursively (this GameObject self, int layer)
        {
            foreach (var component in self.GetComponentsInChildren<Transform> ())
            {
                component.gameObject.layer = layer;
            }
        }
    }
}