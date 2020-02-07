using Chsopoly.BaseSystem.GameScene;
using UnityEngine;

namespace Chsopoly.GameScene
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