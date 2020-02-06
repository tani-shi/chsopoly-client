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

        public GameSceneType SceneType
        {
            get
            {
                var sceneName = GetType ().Name.Replace ("Scene", "");
                if (Enum.IsDefined (typeof (GameSceneType), sceneName))
                {
                    return (GameSceneType) Enum.Parse (typeof (GameSceneType), sceneName);
                }
                return GameSceneType.None;
            }
        }

        public IGameSceneParam Param
        {
            get
            {
                return _param;
            }
        }

        private T _param = new T ();
        private bool _ready = false;

        public virtual void RequestDestroy ()
        {
            Destroy (gameObject);
        }

        protected virtual IEnumerator LoadProc ()
        {
            yield break;
        }

        public void Initialize (IGameSceneParam p)
        {
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

    public abstract class BaseGameScene : BaseGameScene<BaseGameScene.EmptyParam>
    {
        public class EmptyParam : IGameSceneParam { }
    }
}