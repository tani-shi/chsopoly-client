using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityMasterData;
using UnityMasterData.Interfaces;

namespace Chsopoly.BaseSystem.MasterData
{
    public class MasterDataManager : MasterDataManagerBase<MasterDataManager>
    {
        protected override IEnumerator LoadAsyncProc (IMasterDataAccessorObject dao)
        {
            Debug.Log ("LOAD: " + dao.GetName ());
            var handle = Addressables.LoadAssetAsync<ScriptableObject> (dao.GetAssetPath ());
            yield return handle;
            if (handle.Result != null)
            {
                dao.SetData (Instantiate (handle.Result));
                Debug.Log (dao.ToJson ());
            }
            else
            {
                Debug.LogError ("Failed to load a master data asset. " + dao.GetAssetPath ());
            }
        }

        protected override void LoadProc (IMasterDataAccessorObject dao)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var data = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject> (dao.GetAssetPath ());
                if (data != null)
                {
                    dao.SetData (data);
                }
                else
                {
                    Debug.LogError ("Failed to load a master data asset. " + dao.GetAssetPath ());
                }
            }
            else
#endif
            {
                Debug.LogWarning ("It is not supported to load on a frame for except Editor Mode.");
            }
        }
    }
}