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
        public IEnumerator CreateGimmick (uint gimmickId, uint connectionId, Transform parent, Action<GameObject> callback = null, int uniqueId = 0, Vector2 position = new Vector2 ())
        {
            var data = MasterDataManager.Instance.Get<GimmickDAO> ().Get (gimmickId);
            var handle = LoadGimmick (data.assetName);
            yield return handle;

            var obj = handle.Result.CreateInstance (parent);
            var gimmick = obj.SafeAddComponent<GimmickObject> ();
            obj.SafeAddComponent<GimmickObjectModel> ();
            obj.SafeAddComponent<GimmickStateMachine> ().Initialize (gimmick);
            (obj.transform as RectTransform).pivot = new Vector2 (0.5f, 0.5f);
            obj.transform.position = position;
            obj.SetLayerRecursively (IngameSettings.Layers.Gimmick);

            gimmick.Initialize (gimmickId);
            gimmick.SetIdentity (connectionId, uniqueId != 0 ? uniqueId : gimmick.GetHashCode ());

            callback.SafeInvoke (obj);
        }

        private AsyncOperationHandle<GameObject> LoadGimmick (string assetName)
        {
            return Addressables.LoadAssetAsync<GameObject> (IngameSettings.Paths.GimmickPrefab (assetName));
        }
    }
}