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
            callback.SafeInvoke (handle.Result.CreateInstance (parent));
        }

        private AsyncOperationHandle<GameObject> LoadField (string fieldName)
        {
            return Addressables.LoadAssetAsync<GameObject> (IngameSettings.Paths.FieldPrefab (fieldName));
        }
    }
}