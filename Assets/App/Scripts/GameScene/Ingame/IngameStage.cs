using System.Collections;
using System.Collections.Generic;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Event;
using Chsopoly.GameScene.Ingame.Factory;
using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.GameScene.Ingame.Object.Gimmick;
using Chsopoly.GameScene.Ingame.Pool;
using Chsopoly.Libs.Extensions;
using Chsopoly.MasterData.DAO.Ingame;
using Chsopoly.MasterData.VO.Ingame;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame
{
    public class IngameStage : MonoBehaviour
    {
        [SerializeField]
        private GimmickObjectPool _gimmickPool = default;

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

        public GimmickObjectPool GimmickPool
        {
            get
            {
                return _gimmickPool;
            }
        }

        public IEnumerable<GameObject> StageObjects
        {
            get
            {
                return _stageObjects;
            }
        }

        public IEnumerable<GimmickObject> GimmickObjects
        {
            get
            {
                return _gimmickObjects;
            }
        }

        private List<GameObject> _stageObjects = new List<GameObject> ();
        private List<CharacterObject> _characterObjects = new List<CharacterObject> ();
        private List<GimmickObject> _gimmickObjects = new List<GimmickObject> ();
        private GameObject _field = null;
        private CharacterObject _playerCharacter = null;
        private StageVO _stageData = null;
        private uint[] _gimmickLotteryTable = null;

        public IEnumerator Load (uint stageId, uint[] characterIds, uint[] gimmickIds)
        {
            if (_stageData != null)
            {
                Debug.LogWarning ("The stage was already loaded.");
                yield break;
            }

            _stageData = MasterDataManager.Instance.Get<StageDAO> ().Get (stageId);

            var lotteryTable = new List<uint> ();
            foreach (var id in gimmickIds)
            {
                var data = MasterDataManager.Instance.Get<GimmickDAO> ().Get (id);
                for (int i = 0; i < data.lotteryWeight; i++)
                {
                    lotteryTable.Add (id);
                }
            }
            _gimmickLotteryTable = lotteryTable.ToArray ();

            yield return new FieldFactory ().CreateField (_stageData.fieldName, transform, OnCreateField);
            for (int i = 0; i < characterIds.Length; i++)
            {
                yield return new CharacterObjectFactory ().CreateCharacter (characterIds[i], transform, OnCreateObject);
            }
            for (int i = 0; i < IngameSettings.Rules.MaxGimmickQueueCount; i++)
            {
                yield return new GimmickObjectFactory ().CreateGimmick (DrawGimmickId (), transform, OnCreateObject);
            }

            Physics2D.gravity = new Vector2 (0, IngameSettings.Field.Gravity (_stageData.fieldGravity));

            OnLoadComplete ();
        }

        public void PutGimmick (int index, Vector3 position)
        {
            var gimmick = _gimmickPool.Dequeue (index, transform);
            if (gimmick != null)
            {
                gimmick.transform.position = position;
            }

            StartCoroutine (new GimmickObjectFactory ().CreateGimmick (DrawGimmickId (), transform, OnCreateObject));
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

            SetPhysicsMaterialsRecursively (field);
        }

        private void OnCreateObject (GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            _stageObjects.Add (obj);
            _characterObjects.AddIfNotNull (obj.GetComponent<CharacterObject> ());
            _gimmickObjects.AddIfNotNull (obj.GetComponent<GimmickObject> ());

            // The character object that is created first will be a player character.
            if (_playerCharacter == null && obj.HasComponent<CharacterObject> ())
            {
                _playerCharacter = obj.GetComponent<CharacterObject> ();
            }

            if (obj.HasComponent<GimmickObject> ())
            {
                _gimmickPool.Enqueue (obj.GetComponent<GimmickObject> ());
            }

            SetPhysicsMaterialsRecursively (obj);
        }

        private uint DrawGimmickId ()
        {
            return _gimmickLotteryTable[Random.Range (0, _gimmickLotteryTable.Length)];
        }

        private void SetPhysicsMaterialsRecursively (GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            foreach (var collider in obj.GetComponentsInChildren<Collider2D> ())
            {
                collider.sharedMaterial = IngameSettings.Physics.DefaultMaterial;
            }
        }
    }
}