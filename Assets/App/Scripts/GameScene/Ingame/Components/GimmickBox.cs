using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Object.Gimmick;
using Chsopoly.GameScene.Ingame.Pool;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Chsopoly.GameScene.Ingame.Components
{
    public class GimmickBox : MonoBehaviour
    {
        [SerializeField]
        private List<GimmickBoxItem> _items = default;

        private GimmickObjectPool _gimmickPool = null;
        private Dictionary<uint, Sprite> _gimmickIconTextureMap = new Dictionary<uint, Sprite> ();

        public IEnumerator LoadTextures (uint[] gimmickIds)
        {
            foreach (var id in gimmickIds)
            {
                if (!_gimmickIconTextureMap.ContainsKey (id))
                {
                    var data = MasterDataManager.Instance.Get<GimmickDAO> ().Get (id);
                    var handle = Addressables.LoadAssetAsync<Sprite> (IngameSettings.Paths.GimmickCapture (data.assetName));
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
            _gimmickPool.onSetGimmicks += OnSetGimmicks;
            _gimmickPool.onEnqueueGimmick += OnEnqueueGimmick;
            _gimmickPool.onDequeueGimmick += OnDequeueGimmick;

            if (_gimmickPool.HasSet)
            {
                OnSetGimmicks (_gimmickPool.Gimmicks.ToArray ());
            }
            if (_gimmickPool.PooledGimmick != null)
            {
                OnEnqueueGimmick (_gimmickPool.PooledGimmick);
            }
        }

        public void SetPutGimmickCallback (Action<int, Vector2> callback)
        {
            foreach (var item in _items)
            {
                item.onPutGimmick -= callback;
                item.onPutGimmick += callback;
            }
        }

        void OnDestroy ()
        {
            if (_gimmickPool != null)
            {
                _gimmickPool.onSetGimmicks -= OnSetGimmicks;
                _gimmickPool.onEnqueueGimmick -= OnEnqueueGimmick;
                _gimmickPool.onDequeueGimmick -= OnDequeueGimmick;
            }
        }

        private void OnSetGimmicks (GimmickObject[] gimmicks)
        {
            for (int i = 0; i < gimmicks.Length; i++)
            {
                _items[i].Initialize (gimmicks[i].Id, _gimmickIconTextureMap[gimmicks[i].Id]);
            }
        }

        private void OnEnqueueGimmick (GimmickObject gimmick)
        {
            _items[IngameSettings.Rules.MaxGimmickBoxCount].Initialize (gimmick.Id, _gimmickIconTextureMap[gimmick.Id]);
        }

        private void OnDequeueGimmick (int index, GimmickObject before, GimmickObject after)
        {
            _items[index].Initialize (after.Id, _gimmickIconTextureMap[after.Id], true);
            _items[IngameSettings.Rules.MaxGimmickBoxCount].Initialize (0, null);
        }
    }
}