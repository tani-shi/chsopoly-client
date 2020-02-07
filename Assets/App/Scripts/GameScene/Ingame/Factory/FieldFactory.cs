using System.Collections;
using Chsopoly.Libs.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Chsopoly.GameScene.Ingame.Factory
{
    public class FieldFactory
    {
        private const string PrefabPathFormat = "Assets/App/AddressableAssets/Prefabs/Field/{0}.prefab";

        private GameObject _fieldObject = null;

        public IEnumerator CreateField (string fieldName, Transform parent)
        {
            if (_fieldObject != null)
            {
                Debug.LogWarning ("The field was already loaded.");
                yield break;
            }
            var handle = LoadField (fieldName);
            yield return handle;
            _fieldObject = handle.Result.CreateInstance (parent);
        }

        public void DestroyField ()
        {
            if (_fieldObject != null)
            {
                GameObject.Destroy (_fieldObject);
            }
        }

        private AsyncOperationHandle<GameObject> LoadField (string fieldName)
        {
            return Addressables.LoadAssetAsync<GameObject> (string.Format (PrefabPathFormat, fieldName));
        }
    }
}