using System.Collections;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.GameScene.Ingame;
using UnityEngine;

namespace Chsopoly.GameScene
{
    public class IngameScene : BaseGameScene<IngameScene.Param>
    {
        [SerializeField]
        private IngameStage _stage = default;

        public class Param : IGameSceneParam
        {
            public uint stageId;
        }

        protected override IEnumerator LoadProc (Param param)
        {
            yield return _stage.Load (param.stageId, new uint[]
            {
                1,
            });
        }
    }
}