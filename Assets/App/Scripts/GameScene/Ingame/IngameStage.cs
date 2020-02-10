using System.Collections;
using System.Collections.Generic;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Event;
using Chsopoly.GameScene.Ingame.Factory;
using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.Libs.Extensions;
using Chsopoly.MasterData.DAO.Ingame;
using Chsopoly.MasterData.VO.Ingame;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame
{
    public class IngameStage : MonoBehaviour
    {
        public GameObject Field
        {
            get
            {
                return _field;
            }
        }

        public CharacterObject PlayerCharacter
        {
            get
            {
                return _playerCharacter;
            }
        }

        private List<GameObject> _stageObjects = new List<GameObject> ();
        private List<CharacterObject> _characterObjects = new List<CharacterObject> ();
        private GameObject _field = null;
        private CharacterObject _playerCharacter = null;
        private StageVO _stageData = new StageVO ();

        public IEnumerator Load (uint stageId, uint[] characterIds)
        {
            _stageData = MasterDataManager.Instance.Get<StageDAO> ().Get (stageId);

            yield return new FieldFactory ().CreateField (_stageData.fieldName, transform, OnCreateField);
            for (int i = 0; i < characterIds.Length; i++)
            {
                yield return new CharacterObjectFactory ().CreateCharacter (characterIds[i], transform, OnCreateObject);
            }

            Physics2D.gravity = new Vector2 (0, IngameSettings.Field.Gravity (_stageData.fieldGravity));

            OnLoadComplete ();
        }

        private void OnLoadComplete ()
        {
            foreach (var obj in _stageObjects)
            {
                (obj.GetComponent<IIngameLoadCompleteEvent> ())?.OnIngameLoadComplete ();
            }
        }

        private void OnCreateField (GameObject field)
        {
            if (field == null)
            {
                return;
            }

            if (_field == null)
            {
                _field = field;
            }
        }

        private void OnCreateObject (GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            _stageObjects.Add (obj);
            _characterObjects.AddIfNotNull (obj.GetComponent<CharacterObject> ());

            // The character object that is created first will be a player character.
            if (_playerCharacter == null && obj.HasComponent<CharacterObject> ())
            {
                _playerCharacter = obj.GetComponent<CharacterObject> ();
            }
        }
    }
}