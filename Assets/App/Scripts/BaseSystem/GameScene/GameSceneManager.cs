using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Chsopoly.BaseSystem.GameScene.Event;
using Chsopoly.GameScene;
using Chsopoly.Libs;
using Chsopoly.Libs.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Chsopoly.BaseSystem.GameScene
{
    public class GameSceneManager : SingletonMonoBehaviour<GameSceneManager>
    {
        private const float FadeInTime = 0.2f;
        private const float FadeOutTime = 0.2f;

        [SerializeField]
        private CanvasGroup _fadeCanvasGroup = default;
        [SerializeField]
        private Image _fadeImage = default;

        public static IGameScene CurrentScene
        {
            get
            {
                return Instance._gameScene;
            }
        }

        private enum State
        {
            None,
            Idle,
            Opening,
            Closing,
            Loading,
        }

        private struct GameSceneInfo
        {
            public GameSceneType type;
            public IGameSceneParam param;

            public GameSceneInfo (GameSceneType type, IGameSceneParam param = null)
            {
                this.type = type;
                this.param = param;
            }
        }

        private Stack<GameSceneInfo> _sceneInfoHistory = new Stack<GameSceneInfo> ();
        private GameSceneInfo _nextSceneInfo;
        private State _state = State.None;
        private float _fadeRate;
        private IGameScene _gameScene;
        private AsyncOperationHandle<GameObject> _loader;
        private GameSceneType _defaultSceneType = GameSceneType.None;
        private GameObject _loaderObject = null;
        private Dictionary<GameSceneType, GameObject> _loaderAssetMap = new Dictionary<GameSceneType, GameObject> ();
        private bool _initialized = false;

        public IEnumerator Initialize ()
        {
            foreach (GameSceneType type in Enum.GetValues (typeof (GameSceneType)))
            {
                if (GameSceneTypeHelper.HasLoader (type))
                {
                    var handle = Addressables.LoadAssetAsync<GameObject> (GameSceneTypeHelper.GetLoaderAssetPath (type));
                    yield return handle;
                    _loaderAssetMap.Add (type, handle.Result);
                }
            }

            _initialized = true;
        }

        public void ChangeScene (GameSceneType type, IGameSceneParam param = null)
        {
            ValidateInitialized ();

            if (_defaultSceneType == GameSceneType.None)
            {
                _defaultSceneType = type;
            }

            _nextSceneInfo = new GameSceneInfo (type, param);

            Debug.Log ("Change scene: " + type.ToString ());
        }

        public void PushScene (GameSceneType type, IGameSceneParam param = null)
        {
            ValidateInitialized ();

            _nextSceneInfo = new GameSceneInfo (type, param);
            _sceneInfoHistory.Push (new GameSceneInfo (_gameScene.sceneType, param));

            Debug.Log ("Push scene: " + type.ToString ());
        }

        public void PopScene ()
        {
            ValidateInitialized ();

            if (_sceneInfoHistory.Count > 0)
            {
                _nextSceneInfo = _sceneInfoHistory.Pop ();

                Debug.Log ("Back scene: " + _nextSceneInfo.type.ToString ());
            }
            else if (_defaultSceneType != GameSceneType.None)
            {
                _nextSceneInfo = new GameSceneInfo (_defaultSceneType);

                Debug.Log ("Back to default scene: " + _nextSceneInfo.type.ToString ());
            }
            else
            {
                Debug.LogError ("Cannot back to null scene.");
            }
        }

        public void ClearHistory ()
        {
            _sceneInfoHistory.Clear ();
        }

        void Update ()
        {
            switch (_state)
            {
                case State.None:
                case State.Idle:
                    if (HasNextSceneInfo ())
                    {
                        _fadeRate = 0;
                        _state = State.Closing;
                        _fadeImage.raycastTarget = true;

                        if (GameSceneTypeHelper.HasLoader (_nextSceneInfo.type))
                        {
                            var _loaderObject = _loaderAssetMap[_nextSceneInfo.type].CreateInstance ();
                            _loaderObject.transform.SetParent (_fadeImage.transform);
                        }

                        if (_gameScene != null)
                        {
                            (_gameScene as IGameSceneCloseStartEvent)?.OnCloseStart ();
                        }
                    }
                    break;

                case State.Opening:
                    if (_fadeRate < 1)
                    {
                        _fadeRate = FadeInTime > 0 ? _fadeRate + (Time.deltaTime / FadeInTime) : 1;
                        _fadeCanvasGroup.alpha = Mathf.Lerp (1, 0, _fadeRate);
                    }
                    else
                    {
                        _state = State.Idle;
                        _fadeImage.raycastTarget = false;

                        if (_loaderObject != null)
                        {
                            Destroy (_loaderObject);
                            _loaderObject = null;
                        }

                        if (_gameScene != null)
                        {
                            (_gameScene as IGameSceneOpenCompleteEvent)?.OnOpenComplete ();
                        }
                    }
                    break;

                case State.Closing:
                    if (_fadeRate < 1)
                    {
                        _fadeRate = FadeOutTime > 0 ? _fadeRate + (Time.deltaTime / FadeOutTime) : 1;
                        _fadeCanvasGroup.alpha = Mathf.Lerp (0, 1, _fadeRate);
                    }
                    else
                    {
                        _loader = LoadGameScene (_nextSceneInfo);
                        _state = State.Loading;

                        if (_gameScene != null)
                        {
                            (_gameScene as IGameSceneCloseCompleteEvent)?.OnCloseComplete ();
                        }
                    }
                    break;

                case State.Loading:
                    if (HasNextSceneInfo ())
                    {
                        if (_loader.IsDone)
                        {
                            if (_loader.Status == AsyncOperationStatus.Succeeded && _loader.Result != null)
                            {
                                if (_gameScene != null)
                                {
                                    _gameScene.RequestDestroy ();
                                }
                                _gameScene = CreateGameScene (_loader.Result, _nextSceneInfo);
                            }
                            else
                            {
                                Debug.LogError ("Failed to load a scene object. " + _loader.OperationException.ToString ());
                            }

                            ClearNextSceneInfo ();
                            ReleaseLoader ();
                        }
                    }
                    else if (_gameScene != null)
                    {
                        if (_gameScene.IsReady)
                        {
                            _fadeRate = 0;
                            _state = State.Opening;
                            (_gameScene as IGameSceneOpenStartEvent)?.OnOpenStart ();
                        }
                    }
                    break;
            }
        }

        private void ValidateInitialized ()
        {
            if (!_initialized)
            {
                throw new Exception ("[GameSceneManager] Do initialize first before do anything.");
            }
        }

        private AsyncOperationHandle<GameObject> LoadGameScene (GameSceneInfo info)
        {
            return Addressables.LoadAssetAsync<GameObject> (GameSceneTypeHelper.GetAssetPath (info.type));
        }

        private IGameScene CreateGameScene (GameObject obj, GameSceneInfo info)
        {
            var sceneObject = obj.CreateInstance ();
            if (sceneObject != null)
            {
                var scene = sceneObject.GetComponent<IGameScene> ();
                if (scene != null)
                {
                    scene.Initialize (info.type, info.param);
                    return scene;
                }
            }
            Debug.LogError ("Failed to create a instance of GameScene. " + info.type.ToString ());
            return null;
        }

        private bool HasNextSceneInfo ()
        {
            return _nextSceneInfo.type != GameSceneType.None;
        }

        private void ClearNextSceneInfo ()
        {
            _nextSceneInfo.type = GameSceneType.None;
            _nextSceneInfo.param = null;
        }

        private void ReleaseLoader ()
        {
            Addressables.Release (_loader);
        }
    }
}