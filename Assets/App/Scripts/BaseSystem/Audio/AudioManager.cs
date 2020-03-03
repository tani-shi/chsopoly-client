using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chsopoly.Audio;
using Chsopoly.GameScene;
using Chsopoly.Libs;
using Chsopoly.Libs.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Chsopoly.BaseSystem.Audio
{
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        [SerializeField]
        private float _bgmVolume = 1.0f;
        [SerializeField]
        private float _efxVolume = 1.0f;

        private Dictionary<Bgm, AudioClip> _loadedBgmClips = new Dictionary<Bgm, AudioClip> ();
        private Dictionary<Efx, AudioClip> _loadedEfxClips = new Dictionary<Efx, AudioClip> ();
        private AudioSource _bgmSource = null;
        private Bgm _playingBgm = Bgm.None;
        private List<AudioSource> _efxSources = new List<AudioSource> ();

        public IEnumerator LoadAsync ()
        {
            yield return LoadBgm ();
            yield return LoadEfx ();
        }

        public void PlayBgm (Bgm bgm)
        {
            if (!CheckLoaded (bgm) || _playingBgm == bgm)
            {
                return;
            }

            StopBgm ();

            _bgmSource = new GameObject (bgm.ToString ()).AddComponent<AudioSource> ();
            _bgmSource.clip = _loadedBgmClips[bgm];
            _bgmSource.loop = true;
            _bgmSource.volume = _bgmVolume;
            _bgmSource.transform.SetParent (transform);
            _bgmSource.Play ();

            _playingBgm = bgm;
        }

        public void PlayEfx (Efx efx, Transform parent = null)
        {
            if (!CheckLoaded (efx))
            {
                return;
            }

            var efxSource = new GameObject (efx.ToString ()).AddComponent<AudioSource> ();
            efxSource.clip = _loadedEfxClips[efx];
            efxSource.loop = false;
            efxSource.volume = _efxVolume;
            efxSource.Play ();

            if (parent != null)
            {
                efxSource.transform.SetParent (parent);
            }
            else
            {
                efxSource.transform.SetParent (transform);
            }

            _efxSources.Add (efxSource);
        }

        public void PauseBgm ()
        {
            if (_bgmSource != null)
            {
                _bgmSource.Pause ();
            }
        }

        public void ResumeBgm ()
        {
            if (_bgmSource != null)
            {
                _bgmSource.UnPause ();
            }
        }

        public void StopBgm ()
        {
            if (_bgmSource != null)
            {
                Destroy (_bgmSource);
                _bgmSource = null;
                _playingBgm = Bgm.None;
            }
        }

        void Update ()
        {
            var removed = new List<AudioSource> ();
            foreach (var efx in _efxSources)
            {
                if (!efx.isPlaying)
                {
                    Destroy (efx.gameObject);
                    removed.Add (efx);
                }
            }
            foreach (var efx in removed)
            {
                _efxSources.Remove (efx);
            }
        }

        private IEnumerator LoadBgm ()
        {
            yield return Addressables.LoadAssetsAsync<AudioClip> (nameof (Bgm), result =>
            {
                var bgm = ToBgm (result.name);
                if (bgm != Bgm.None)
                {
                    _loadedBgmClips.Add (bgm, result);
                }
            });
        }

        private IEnumerator LoadEfx ()
        {
            yield return Addressables.LoadAssetsAsync<AudioClip> (nameof (Efx), result =>
            {
                var efx = ToEfx (result.name);
                if (efx != Efx.None)
                {
                    _loadedEfxClips.Add (efx, result);
                }
            });
        }

        private Bgm ToBgm (string name)
        {
            var bgm = Enum.Parse (typeof (Bgm), name.Replace (AudioSettings.BgmPrefix, "").Snake2Pascal ());
            if (bgm != null)
            {
                return (Bgm) bgm;
            }
            return Bgm.None;
        }

        private Efx ToEfx (string name)
        {
            var efx = Enum.Parse (typeof (Efx), name.Replace (AudioSettings.EfxPrefix, "").Snake2Pascal ());
            if (efx != null)
            {
                return (Efx) efx;
            }
            return Efx.None;
        }

        private bool CheckLoaded (Bgm bgm)
        {
            if (!_loadedBgmClips.ContainsKey (bgm))
            {
                Debug.LogError ("A non-loaded bgm clip was specified. " + bgm.ToString ());
                return false;
            }
            return true;
        }

        private bool CheckLoaded (Efx efx)
        {
            if (!_loadedEfxClips.ContainsKey (efx))
            {
                Debug.LogError ("A non-loaded efx clip was specified. " + efx.ToString ());
                return false;
            }
            return true;
        }
    }
}