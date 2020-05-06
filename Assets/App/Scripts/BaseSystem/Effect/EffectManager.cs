using System;
using System.Collections;
using System.Collections.Generic;
using Chsopoly.Effect;
using Chsopoly.Libs;
using Chsopoly.Libs.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Chsopoly.BaseSystem.Effect
{
    public class EffectManager : SingletonMonoBehaviour<EffectManager>
    {
        private Dictionary<Eff, GameObject> _loadedEffects = new Dictionary<Eff, GameObject> ();

        public IEnumerator LoadAsync ()
        {
            yield return Addressables.LoadAssetsAsync<GameObject> (EffectSettings.EffectLabelName, result =>
            {
                var eff = ToEff (result.name);
                if (eff != Eff.None)
                {
                    _loadedEffects.Add (eff, result);
                }
            });
        }

        public GameObject Play (
            Eff eff,
            Transform parent = null,
            Vector3 position = default,
            Vector3 localPosition = default,
            Vector3 eulerAngles = default,
            int sortingLayerID = default)
        {
            if (!CheckLoaded (eff))
            {
                return null;
            }

            var go = _loadedEffects[eff].CreateInstance (parent);
            go.transform.eulerAngles = eulerAngles;

            if (position != default)
            {
                go.transform.position = position;
            }
            else if (localPosition != default)
            {
                go.transform.localPosition = localPosition;
            }

            var ps = go.GetComponent<ParticleSystemRenderer> ();
            if (ps != null)
            {
                ps.sortingLayerName = EffectSettings.EffectLayerName;
            }

            return go;
        }

        public GameObject PlayOneShot (
            Eff eff,
            Transform parent = null,
            Vector3 position = default,
            Vector3 localPosition = default,
            Vector3 eulerAngles = default,
            int sortingLayerID = default)
        {
            var go = Play (eff, parent, position, localPosition, eulerAngles, sortingLayerID);
            if (go != null)
            {
                var ps = go.GetComponent<ParticleSystem> ();
                if (ps != null)
                {
                    ps.Stop ();

                    var main = ps.main;
                    main.stopAction = ParticleSystemStopAction.Destroy;

                    ps.Play ();
                }
            }
            return go;
        }

        private Eff ToEff (string name)
        {
            if (name.StartsWith (EffectSettings.EffectPrefix))
            {
                var eff = Enum.Parse (typeof (Eff), name.Replace (EffectSettings.EffectPrefix, "").Snake2Pascal ());
                if (eff != null)
                {
                    return (Eff) eff;
                }
            }
            return Eff.None;
        }

        private bool CheckLoaded (Eff eff)
        {
            if (!_loadedEffects.ContainsKey (eff))
            {
                Debug.LogError ("A non-loaded effect was specified. " + eff.ToString ());
                return false;
            }
            return true;
        }
    }
}