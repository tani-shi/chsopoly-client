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
        private float _seVolume = 1.0f;

        private Dictionary<Bgm, AudioClip> _loadedBgmClips = new Dictionary<Bgm, AudioClip> ();
        private Dictionary<Se, AudioClip> _loadedSeClips = new Dictionary<Se, AudioClip> ();
        private AudioSource _bgmSource = null;
        private Bgm _playingBgm = Bgm.None;
        private List<AudioSource> _seSources = new List<AudioSource> ();

        public IEnumerator LoadAsync ()
        {
            yield return Addressables.LoadAssetsAsync<AudioClip> (AudioSettings.AudioLabelName, result =>
            {
                var bgm = ToBgm (result.name);
                if (bgm != Bgm.None)
                {
                    _loadedBgmClips.Add (bgm, result);
                }

                var se = ToSe (result.name);
                if (se != Se.None)
                {
                    _loadedSeClips.Add (se, result);
                }
            });
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

        public void PlaySe (Se se, Transform parent = null)
        {
            if (!CheckLoaded (se))
            {
                return;
            }

            var seSource = new GameObject (se.ToString ()).AddComponent<AudioSource> ();
            seSource.clip = _loadedSeClips[se];
            seSource.loop = false;
            seSource.volume = _seVolume;
            seSource.Play ();

            if (parent != null)
            {
                seSource.transform.SetParent (parent);
            }
            else
            {
                seSource.transform.SetParent (transform);
            }

            _seSources.Add (seSource);
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
            foreach (var se in _seSources)
            {
                if (!se.isPlaying)
                {
                    Destroy (se.gameObject);
                    removed.Add (se);
                }
            }
            foreach (var se in removed)
            {
                _seSources.Remove (se);
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

        private IEnumerator LoadSe ()
        {
            yield return Addressables.LoadAssetsAsync<AudioClip> (nameof (Se), result =>
            {
                var se = ToSe (result.name);
                if (se != Se.None)
                {
                    _loadedSeClips.Add (se, result);
                }
            });
        }

        private Bgm ToBgm (string name)
        {
            if (name.StartsWith (AudioSettings.BgmPrefix))
            {
                var bgm = Enum.Parse (typeof (Bgm), name.Replace (AudioSettings.BgmPrefix, "").Snake2Pascal ());
                if (bgm != null)
                {
                    return (Bgm) bgm;
                }
            }
            return Bgm.None;
        }

        private Se ToSe (string name)
        {
            if (name.StartsWith (AudioSettings.SePrefix))
            {
                var se = Enum.Parse (typeof (Se), name.Replace (AudioSettings.SePrefix, "").Snake2Pascal ());
                if (se != null)
                {
                    return (Se) se;
                }
            }
            return Se.None;
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

        private bool CheckLoaded (Se se)
        {
            if (!_loadedSeClips.ContainsKey (se))
            {
                Debug.LogError ("A non-loaded se clip was specified. " + se.ToString ());
                return false;
            }
            return true;
        }
    }
}