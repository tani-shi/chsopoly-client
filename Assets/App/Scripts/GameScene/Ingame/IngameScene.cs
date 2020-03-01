using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.Gs2;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.GameScene.Ingame.UI;
using Chsopoly.Gs2.Models;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame
{
    public class IngameScene : BaseGameScene<IngameScene.Param>
    {
        public class Param : IGameSceneParam
        {
            public string gatheringId;
            public uint stageId;
            public uint characterId;
            public Dictionary<uint, Profile> otherPlayers;
        }

        [SerializeField]
        private IngameStage _stage = default;
        [SerializeField]
        private PlayerController _controller = default;
        [SerializeField]
        private IngameCamera _camera = default;
        [SerializeField]
        private GimmickBox _gimmickBox = default;

        private Dictionary<uint, Profile> _otherPlayers = new Dictionary<uint, Profile> ();
        private string _gatheringId = string.Empty;

        void Start ()
        {
            Gs2Manager.Instance.onRelayRealtimeMessage += OnRelayMessage;
            Gs2Manager.Instance.onCloseRealtime += OnCloseConnection;
        }

        void Destroy ()
        {
            Gs2Manager.Instance.onRelayRealtimeMessage -= OnRelayMessage;
            Gs2Manager.Instance.onCloseRealtime -= OnCloseConnection;
        }

        protected override IEnumerator LoadProc (Param param)
        {
            _gatheringId = param.gatheringId;
            _otherPlayers = param.otherPlayers;

            var gimmickIds = MasterDataManager.Instance.Get<GimmickDAO> ().FindAll (o => o.id > 0).ConvertAll (o => o.id).ToArray ();

            yield return _stage.Load (param.stageId, param.characterId, param.otherPlayers, gimmickIds);
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

        private void OnRelayMessage (uint connectionId, Gs2PacketModel model)
        {
            _stage.ApplyRelayMessage (connectionId, model);
        }

        private void OnCloseConnection (uint connectionId, string reason, bool wasClean)
        {
            _stage.DestroyOtherPlayerCharacter (connectionId);
        }
    }
}