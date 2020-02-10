using System.Collections;
using System.Collections.Generic;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Factory;
using Chsopoly.MasterData.DAO.Ingame;
using Chsopoly.MasterData.VO.Ingame;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame
{
    public class IngameStage : MonoBehaviour
    {
        private FieldFactory _fieldFactory = null;
        private List<CharacterObjectFactory> _characterFactory = null;
        private StageVO _stageData = new StageVO ();

        public IngameStage ()
        {
            _fieldFactory = new FieldFactory ();
            _characterFactory = new List<CharacterObjectFactory> ();
            for (int i = 0; i < IngameSettings.MaxPlayerCount; i++)
            {
                _characterFactory.Add (new CharacterObjectFactory ());
            }
        }

        public IEnumerator Load (uint stageId, uint[] characterIds)
        {
            _stageData = MasterDataManager.Instance.Get<StageDAO> ().Get (stageId);

            yield return _fieldFactory.CreateField (_stageData.fieldName, transform);
            for (int i = 0; i < characterIds.Length; i++)
            {
                yield return _characterFactory[i].CreateCharacter (characterIds[i], transform);
            }
        }
    }
}