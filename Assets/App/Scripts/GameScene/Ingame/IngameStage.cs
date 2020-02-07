using System.Collections;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Factory;
using Chsopoly.MasterData.DAO.Ingame;
using Chsopoly.MasterData.VO.Ingame;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame
{
    public class IngameStage : MonoBehaviour
    {
        private FieldFactory _fieldFactory = new FieldFactory ();
        private StageVO _stageData = new StageVO ();

        public IEnumerator Load (uint stageId)
        {
            var dao = MasterDataManager.Instance.Get<StageDAO> ();
            _stageData = MasterDataManager.Instance.Get<StageDAO> ().Get (stageId);
            yield return _fieldFactory.CreateField (_stageData.fieldName, transform);
        }
    }
}