using System;
using System.Collections;
using Chsopoly.Libs.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Chsopoly.GameScene.Ingame.Factory
{
    public class FieldFactory
    {
        public IEnumerator CreateField (string fieldName, Transform parent, Action<GameObject> callback = null)
        {
            var handle = LoadField (fieldName);
            yield return handle;

            var obj = handle.Result.CreateInstance (parent);
            CreateSideWall (false, obj.transform as RectTransform);
            CreateSideWall (true, obj.transform as RectTransform);
            CreateCeilWall (obj.transform as RectTransform);

            foreach (var collider in obj.GetComponentsInChildren<Collider2D> ())
            {
                collider.sharedMaterial = IngameSettings.Field.PhysicsMaterial;
            }

            callback.SafeInvoke (obj);
        }

        private AsyncOperationHandle<GameObject> LoadField (string fieldName)
        {
            return Addressables.LoadAssetAsync<GameObject> (IngameSettings.Paths.FieldPrefab (fieldName));
        }

        private GameObject CreateSideWall (bool dir, RectTransform parent)
        {
            var obj = new GameObject ("Wall");
            obj.transform.parent = parent;
            obj.SafeAddComponent<RectTransform> ().localPosition = Vector2.zero;

            var collider = obj.AddComponent<BoxCollider2D> ();
            collider.size = new Vector2 (IngameSettings.Field.WallSize, parent.sizeDelta.y * 2f);
            collider.offset = new Vector2 ((dir ? 1 : -1) * (parent.sizeDelta.x.Half () + IngameSettings.Field.WallSize.Half ()), 0);

            return obj;
        }

        private GameObject CreateCeilWall (RectTransform parent)
        {
            var obj = new GameObject ("Wall");
            obj.transform.parent = parent;
            obj.SafeAddComponent<RectTransform> ().localPosition = Vector2.zero;

            var collider = obj.AddComponent<BoxCollider2D> ();
            collider.size = new Vector2 (parent.sizeDelta.x, IngameSettings.Field.WallSize);
            collider.offset = new Vector2 (0, parent.sizeDelta.y.Half () + IngameSettings.Field.WallSize.Half ());

            return obj;
        }
    }
}