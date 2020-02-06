using System.Collections;
using Chsopoly.BaseSystem.GameScene;

namespace Chsopoly.GameScene
{
    public class IngameScene : BaseGameScene<IngameScene.Param>
    {
        public class Param : IGameSceneParam
        {
            public int stageId;
        }

        protected override IEnumerator LoadProc ()
        {
            yield break;
        }
    }
}