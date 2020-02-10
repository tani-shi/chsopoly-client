using System.Collections;
using Chsopoly.BaseSystem.GameScene;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame
{
    public class IngameScene : BaseGameScene<IngameScene.Param>
    {
        [SerializeField]
        private IngameStage _stage = default;
        [SerializeField]
        private IngameController _controller = default;

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

            _controller.SetPlayer (_stage.PlayerCharacter);
        }
    }
}