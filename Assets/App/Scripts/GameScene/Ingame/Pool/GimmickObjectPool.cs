using System;
using System.Collections.Generic;
using Chsopoly.GameScene.Ingame.Object.Gimmick;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Pool
{
    public class GimmickObjectPool : MonoBehaviour
    {
        public List<GimmickObject> Gimmicks
        {
            get
            {
                return _gimmicks;
            }
        }

        private List<GimmickObject> _gimmicks = new List<GimmickObject> ();

        public void Enqueue (GimmickObject gimmick)
        {
            gimmick.transform.SetParent (transform);
            gimmick.transform.localPosition = Vector3.zero;
            gimmick.gameObject.SetActive (false);
            _gimmicks.Add (gimmick);
        }

        public GimmickObject Dequeue (int index, Transform parent)
        {
            if (index >= IngameSettings.Rules.MaxGimmickBoxCount)
            {
                Debug.LogWarning ("The index to dequeue a gimmick must be smaller than " + IngameSettings.Rules.MaxGimmickBoxCount + ".");
                return null;
            }
            if (index >= _gimmicks.Count)
            {
                return null;
            }

            var gimmick = _gimmicks[index];
            _gimmicks.RemoveAt (index);
            gimmick.gameObject.SetActive (true);
            gimmick.transform.SetParent (parent);

            return gimmick;
        }
    }
}