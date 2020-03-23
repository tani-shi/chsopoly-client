using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.Components.Button;
using Chsopoly.GameScene.Matching;
using Chsopoly.GameScene.Mypage.Components;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;

namespace Chsopoly.GameScene.Mypage
{
    public class MypageScene : BaseGameScene
    {
        [SerializeField]
        private CharacterSelection _characterSelection = default;

        protected override IEnumerator LoadProc (Param param)
        {
            var ids = MasterDataManager.Instance.Get<CharacterDAO> ().ToList ().ConvertAll (o => o.id);
            yield return _characterSelection.Initialize (ids);
        }

        public void OnClickMatching ()
        {
            var param = new MatchingScene.Param ()
            {
                capacity = 2,
            };
            GameSceneManager.Instance.ChangeScene (GameSceneType.Matching, param);
        }

        public void OnClickSingle ()
        {
            var param = new Chsopoly.GameScene.Ingame.IngameScene.Param ()
            {
                stageId = 1,
                otherPlayers = new System.Collections.Generic.Dictionary<uint, Gs2.Models.Profile> (),
                characterId = 1,
            };
            GameSceneManager.Instance.ChangeScene (GameSceneType.Ingame, param);
        }

        public void OnClickRanking ()
        {

        }

        public void OnClickShop ()
        {

        }

        public void OnClickGimmick ()
        {

        }

        public void OnClickSettings ()
        {

        }

        public void OnClickHelp ()
        {

        }
    }
}