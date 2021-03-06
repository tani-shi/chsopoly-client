using Chsopoly.GameScene;
using UnityEngine;

namespace Chsopoly.BaseSystem.GameScene
{
    public interface IGameScene
    {
        bool IsReady { get; }
        GameSceneType sceneType { get; }

        void Initialize (GameSceneType type, IGameSceneParam p);
        void RequestDestroy ();
    }
}