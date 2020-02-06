using Chsopoly.GameScene;
using UnityEngine;

namespace Chsopoly.BaseSystem.GameScene
{
    public interface IGameScene
    {
        bool IsReady { get; }
        GameSceneType SceneType { get; }
        IGameSceneParam Param { get; }

        void Initialize (IGameSceneParam p);
        void RequestDestroy ();
    }
}