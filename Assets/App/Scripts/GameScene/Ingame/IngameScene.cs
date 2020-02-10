using System.Collections;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.GameScene.Ingame.UI;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame
{
    public class IngameScene : BaseGameScene<IngameScene.Param>
    {
        [SerializeField]
        private IngameStage _stage = default;
        [SerializeField]
        private PlayerController _controller = default;
        [SerializeField]
        private IngameCamera _camera = default;

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
            _camera.SetTarget (_stage.PlayerCharacter.transform);
            _camera.SetBounds ((_stage.Field.transform as RectTransform).sizeDelta);
        }
    }
}