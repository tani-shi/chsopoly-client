using Chsopoly.BaseSystem.GameScene;
using Chsopoly.GameScene.Ingame;
using UnityEngine;

namespace Chsopoly.GameScene.Title
{
    public class TitleScene : BaseGameScene
    {
        public void OnClickScreen ()
        {
            GameSceneManager.Instance.ChangeScene (GameSceneType.Ingame, new IngameScene.Param ()
            {
                stageId = 1,
            });
        }
    }
}