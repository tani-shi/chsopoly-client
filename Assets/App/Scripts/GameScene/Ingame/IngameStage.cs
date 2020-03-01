using System.Collections;
using System.Collections.Generic;
using Chsopoly.BaseSystem.Gs2;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Event;
using Chsopoly.GameScene.Ingame.Factory;
using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.GameScene.Ingame.Object.Gimmick;
using Chsopoly.GameScene.Ingame.Pool;
using Chsopoly.Gs2.Models;
using Chsopoly.Libs.Extensions;
using Chsopoly.MasterData.DAO.Ingame;
using Chsopoly.MasterData.VO.Ingame;
using UnityEngine;
using CharacterState = Chsopoly.GameScene.Ingame.Object.Character.CharacterStateMachine.State;

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

        public IEnumerable<CharacterObject> CharacterObjects
        {
            get
            {
                return _characterObjects;
            }
        }

        public IEnumerable<CharacterObject> OtherCharacterObjects
        {
            get
            {
                return _otherCharacterObjects;
            }
        }

        private List<GameObject> _stageObjects = new List<GameObject> ();
        private List<CharacterObject> _otherCharacterObjects = new List<CharacterObject> ();
        private List<CharacterObject> _characterObjects = new List<CharacterObject> ();
        private List<GimmickObject> _gimmickObjects = new List<GimmickObject> ();
        private GameObject _field = null;
        private CharacterObject _playerCharacter = null;
        private StageVO _stageData = null;
        private uint[] _gimmickLotteryTable = null;

        public IEnumerator Load (uint stageId, uint playerCharacterId, uint[] otherCharacterIds, uint[] gimmickIds)
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
            yield return new CharacterObjectFactory ().CreateCharacter (playerCharacterId, transform, OnCreateObject);
            for (int i = 0; i < otherCharacterIds.Length; i++)
            {
                yield return new CharacterObjectFactory ().CreateCharacter (otherCharacterIds[i], transform, OnCreateObject);
            }
            for (int i = 0; i < IngameSettings.Rules.MaxGimmickQueueCount; i++)
            {
                yield return new GimmickObjectFactory ().CreateGimmick (DrawGimmickId (), transform, OnCreateObject);
            }

            Physics2D.gravity = new Vector2 (0, IngameSettings.Field.Gravity (_stageData.fieldGravity));
            Physics2D.IgnoreLayerCollision (IngameSettings.Layers.Character, IngameSettings.Layers.Character);

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

        public void SendRelayMessage (Gs2PacketModel model)
        {
            if (_otherCharacterObjects.Count == 0 || _otherCharacterObjects.Exists (o => o == null))
            {
                return;
            }

            Gs2Manager.Instance.SendRelayMessage (model);
        }

        public void SendPlayerSyncMessage ()
        {
            var model = new CharacterObjectSync ()
            {
                x = _playerCharacter.worldPosition.x,
                y = _playerCharacter.worldPosition.y,
                state = (int) _playerCharacter.StateMachine.CurrentState,
                direction = (int) _playerCharacter.Direction,
            };

            SendRelayMessage (model);
        }

        public void ApplyRelayMessage (int playerIndex, Gs2PacketModel model)
        {
            if (playerIndex < 0 || playerIndex >= _otherCharacterObjects.Count || _otherCharacterObjects[playerIndex] == null)
            {
                return;
            }

            if (model is CharacterObjectSync sync)
            {
                _otherCharacterObjects[playerIndex].worldPosition = new Vector2 (sync.x, sync.y);
                _otherCharacterObjects[playerIndex].SetMoveDirection ((CharacterObject.MoveDirection) sync.direction);
                _otherCharacterObjects[playerIndex].StateMachine.SetNextState (
                    (CharacterState) sync.state,
                    _otherCharacterObjects[playerIndex].StateMachine.CurrentState != (CharacterState) sync.state);
            }
        }

        public void DestroyOtherPlayerCharacter (int playerIndex)
        {
            Destroy (_otherCharacterObjects[playerIndex]);
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

            if (obj.HasComponent<CharacterObject> ())
            {
                var character = obj.GetComponent<CharacterObject> ();

                // The character object that is created first will be a player character.
                if (_playerCharacter == null)
                {
                    _playerCharacter = character;
                    _playerCharacter.SetPlayerCharacter (true);
                    _playerCharacter.StateMachine.onStateChanged += (_, __) => SendPlayerSyncMessage ();
                    _playerCharacter.onChangedMoveDirection += (_) => SendPlayerSyncMessage ();
                }
                else
                {
                    _otherCharacterObjects.Add (character);
                    character.SetPlayerCharacter (false);
                }

                var startPoint = GameObject.FindWithTag (IngameSettings.Tags.StartPoint);
                if (startPoint != null)
                {
                    character.worldPosition = startPoint.transform.position;
                    character.Rigidbody.velocity = Vector2.zero;
                }
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