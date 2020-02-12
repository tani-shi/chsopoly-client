using System.Collections;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.UI;
using Chsopoly.MasterData.DAO.Ingame;
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
        [SerializeField]
        private GimmickBox _gimmickBox = default;

        public class Param : IGameSceneParam
        {
            public uint stageId;
        }

        protected override IEnumerator LoadProc (Param param)
        {
            var characterIds = new uint[] { 1 };
            var gimmickIds = MasterDataManager.Instance.Get<GimmickDAO> ().FindAll (o => o.id > 0).ConvertAll (o => o.id).ToArray ();

            yield return _stage.Load (param.stageId, characterIds, gimmickIds);
            yield return _gimmickBox.LoadTextures (gimmickIds);

            _controller.SetPlayer (_stage.PlayerCharacter);
            _gimmickBox.SetPool (_stage.GimmickPool);
            _gimmickBox.SetPutGimmickCallback (OnPutGimmick);
            _camera.SetTarget (_stage.PlayerCharacter.transform);
            _camera.SetBounds ((_stage.Field.transform as RectTransform).sizeDelta);
        }

        private void OnPutGimmick (int index, Vector2 screenPos)
        {
            var position = _camera.MainCamera.ScreenToWorldPoint (screenPos);
            position.z = 0;

            _stage.PutGimmick (index, position);
        }
    }
}