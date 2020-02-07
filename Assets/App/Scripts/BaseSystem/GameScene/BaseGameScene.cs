using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Chsopoly.GameScene;
using UnityEngine;

namespace Chsopoly.BaseSystem.GameScene
{
    public abstract class BaseGameScene<T> : MonoBehaviour, IGameScene where T : class, IGameSceneParam, new ()
    {
        public bool IsReady
        {
            get
            {
                return _ready;
            }
        }

        public GameSceneType sceneType
        {
            get
            {
                return _sceneType;
            }
        }

        private bool _ready = false;
        private GameSceneType _sceneType = GameSceneType.None;

        public virtual void RequestDestroy ()
        {
            Destroy (gameObject);
        }

        protected virtual IEnumerator LoadProc (T param)
        {
            yield break;
        }

        public void Initialize (GameSceneType type, IGameSceneParam param)
        {
            _sceneType = type;
            if (param is T)
            {
                StartCoroutine (DoLoad (param as T));
            }
            else
            {
                StartCoroutine (DoLoad (new T ()));
            }
        }

        private IEnumerator DoLoad (T param)
        {
            _ready = false;
            yield return LoadProc (param);
            _ready = true;
        }
    }

    public abstract class BaseGameScene : BaseGameScene<BaseGameScene.Param>
    {
        public class Param : IGameSceneParam { }
    }
}