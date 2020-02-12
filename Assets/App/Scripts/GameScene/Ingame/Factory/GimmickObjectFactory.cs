using System;
using System.Collections;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Object.Gimmick;
using Chsopoly.Libs.Extensions;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Chsopoly.GameScene.Ingame.Factory
{
    public class GimmickObjectFactory
    {
        public IEnumerator CreateGimmick (uint gimmickId, Transform parent, Action<GameObject> callback = null)
        {
            var data = MasterDataManager.Instance.Get<GimmickDAO> ().Get (gimmickId);
            var handle = LoadGimmick (data.assetName);
            yield return handle;

            var obj = handle.Result.CreateInstance (parent);
            var gimmick = obj.SafeAddComponent<GimmickObject> ();
            obj.SafeAddComponent<GimmickObjectModel> ();
            obj.SafeAddComponent<GimmickStateMachine> ();
            (obj.transform as RectTransform).pivot = new Vector2 (0.5f, 0.5f);

            foreach (var c in obj.GetComponentsInChildren<Collider2D> (true))
            {
                c.sharedMaterial = IngameSettings.Gimmick.DefaultPhysicsMaterial;
            }

            gimmick.Initialize (gimmickId);

            callback.SafeInvoke (obj);
        }

        private AsyncOperationHandle<GameObject> LoadGimmick (string assetName)
        {
            return Addressables.LoadAssetAsync<GameObject> (IngameSettings.Paths.GimmickPrefab (assetName));
        }
    }
}