using System;
using System.Collections.Generic;
using System.Linq;
using Chsopoly.GameScene.Ingame.Object.Gimmick;
using Chsopoly.Libs.Extensions;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Pool
{
    public class GimmickObjectPool : MonoBehaviour
    {
        public event Action<GimmickObject[]> onSetGimmicks;
        public event Action<GimmickObject> onEnqueueGimmick;
        public event Action<int, GimmickObject, GimmickObject> onDequeueGimmick;

        public IEnumerable<GimmickObject> Gimmicks
        {
            get
            {
                return _gimmicks;
            }
        }

        public GimmickObject PooledGimmick
        {
            get
            {
                if (_pooledGimmickQueue.Count > 0)
                {
                    return _pooledGimmickQueue.Peek ();
                }
                return null;
            }
        }

        public bool HasSet
        {
            get
            {
                return _hasSet;
            }
        }

        private Queue<GimmickObject> _pooledGimmickQueue = new Queue<GimmickObject> ();
        private GimmickObject[] _gimmicks = new GimmickObject[IngameSettings.Rules.MaxGimmickBoxCount];
        private bool _hasSet = false;

        public void Enqueue (GimmickObject gimmick)
        {
            gimmick.transform.SetParent (transform);
            gimmick.transform.localPosition = Vector3.zero;
            gimmick.gameObject.SetActive (false);

            var emptyIndex = _gimmicks.ToList ().FindIndex (o => o == null);
            if (emptyIndex >= 0)
            {
                _gimmicks[emptyIndex] = gimmick;

                if (!_gimmicks.Any (o => o == null))
                {
                    _hasSet = true;
                    onSetGimmicks.SafeInvoke (_gimmicks);
                }
            }
            else
            {
                _pooledGimmickQueue.Enqueue (gimmick);
                onEnqueueGimmick.SafeInvoke (gimmick);
            }
        }

        public GimmickObject Dequeue (int index, Transform parent)
        {
            if (index >= IngameSettings.Rules.MaxGimmickBoxCount)
            {
                Debug.LogWarning ("The index to dequeue a gimmick must be smaller than " + IngameSettings.Rules.MaxGimmickBoxCount + ".");
                return null;
            }
            if (index >= _gimmicks.Length)
            {
                return null;
            }

            var gimmick = _gimmicks[index];
            _gimmicks[index] = _pooledGimmickQueue.Dequeue ();
            gimmick.gameObject.SetActive (true);
            gimmick.transform.SetParent (parent);

            onDequeueGimmick.SafeInvoke (index, gimmick, _gimmicks[index]);

            return gimmick;
        }
    }
}