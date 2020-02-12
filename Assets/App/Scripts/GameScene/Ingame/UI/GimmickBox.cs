using System;
using System.Collections;
using System.Collections.Generic;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Pool;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Chsopoly.GameScene.Ingame.UI
{
    public class GimmickBox : MonoBehaviour
    {
        [SerializeField]
        private List<GimmickBoxItem> _items = new List<GimmickBoxItem> (IngameSettings.Rules.MaxGimmickQueueCount);

        private GimmickObjectPool _gimmickPool = null;
        private Dictionary<uint, Sprite> _gimmickIconTextureMap = new Dictionary<uint, Sprite> ();

        public IEnumerator LoadTextures (uint[] gimmickIds)
        {
            foreach (var id in gimmickIds)
            {
                if (!_gimmickIconTextureMap.ContainsKey (id))
                {
                    var data = MasterDataManager.Instance.Get<GimmickDAO> ().Get (id);
                    var handle = Addressables.LoadAssetAsync<Sprite> (IngameSettings.Paths.GimmickIcon (data.assetName));
                    yield return handle;
                    if (handle.Result == null)
                    {
                        Debug.LogError ("Failed to load a gimmick icon texture. " + data.assetName);
                        continue;
                    }
                    _gimmickIconTextureMap.Add (id, handle.Result);
                }
            }
        }

        public void SetPool (GimmickObjectPool pool)
        {
            _gimmickPool = pool;
        }

        public void SetPutGimmickCallback (Action<int, Vector2> callback)
        {
            foreach (var item in _items)
            {
                item.onPutGimmick -= callback;
                item.onPutGimmick += callback;
            }
        }

        public void Update ()
        {
            if (_gimmickPool == null)
            {
                return;
            }

            for (int i = 0; i < IngameSettings.Rules.MaxGimmickQueueCount; i++)
            {
                if (i < _gimmickPool.Gimmicks.Count)
                {
                    var gimmick = _gimmickPool.Gimmicks[i];
                    _items[i].Initialize (gimmick.Id, _gimmickIconTextureMap[gimmick.Id]);
                }
                else
                {
                    _items[i].Initialize (0, null);
                }
            }
        }
    }
}