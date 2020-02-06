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

        public IGameSceneParam param
        {
            get
            {
                return _param;
            }
        }

        private T _param = new T ();
        private bool _ready = false;
        private GameSceneType _sceneType = GameSceneType.None;

        public virtual void RequestDestroy ()
        {
            Destroy (gameObject);
        }

        protected virtual IEnumerator LoadProc ()
        {
            yield break;
        }

        public void Initialize (GameSceneType type, IGameSceneParam p)
        {
            _sceneType = type;
            _ready = false;
            if (p is T)
            {
                _param = p as T;
            }
            StartCoroutine (DoLoad ());
        }

        private IEnumerator DoLoad ()
        {
            yield return LoadProc ();
            _ready = true;
        }
    }

    public abstract class BaseGameScene : BaseGameScene<BaseGameScene.Param>
    {
        public class Param : IGameSceneParam { }
    }
}