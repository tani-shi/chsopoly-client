using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<GameObject> StageObjects
        {
            get
            {
                return _stageObjects;
            }
        }

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

        private List<GameObject> _stageObjects = new List<GameObject> ();
        private List<CharacterObject> _characterObjects = new List<CharacterObject> ();
        private List<GimmickObject> _gimmickObjects = new List<GimmickObject> ();
        private CharacterObject _playerCharacter = null;
        private Dictionary<int, GimmickObject> _playerGimmickObjectMap = new Dictionary<int, GimmickObject> ();
        private Dictionary<uint, CharacterObject> _otherCharacterObjectMap = new Dictionary<uint, CharacterObject> ();
        private Dictionary<uint, Dictionary<int, GimmickObject>> _otherGimmickObjectMaps = new Dictionary<uint, Dictionary<int, GimmickObject>> ();
        private GameObject _field = null;
        private StageVO _stageData = null;
        private uint[] _gimmickLotteryTable = null;

        public IEnumerator Load (uint stageId, uint playerCharacterId, Dictionary<uint, Profile> otherPlayers, uint[] gimmickIds)
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
            yield return new CharacterObjectFactory ().CreateCharacter (playerCharacterId, IngameSettings.Gs2.PlayerConnectionId, transform, OnCreateObject);
            foreach (var kv in otherPlayers)
            {
                _otherGimmickObjectMaps.Add (kv.Key, new Dictionary<int, GimmickObject> ());

                yield return new CharacterObjectFactory ().CreateCharacter (kv.Value.characterId, kv.Key, transform, OnCreateObject);
            }
            for (int i = 0; i < IngameSettings.Rules.MaxGimmickQueueCount; i++)
            {
                yield return new GimmickObjectFactory ().CreateGimmick (DrawGimmickId (), IngameSettings.Gs2.PlayerConnectionId, transform, OnCreateObject);
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

            StartCoroutine (new GimmickObjectFactory ().CreateGimmick (DrawGimmickId (), IngameSettings.Gs2.PlayerConnectionId, transform, OnCreateObject));

            var packet = new GimmickObjectPut ()
            {
                uniqueId = gimmick.UniqueId,
                gimmickId = gimmick.Id,
                x = position.x,
                y = position.y,
            };
            SendRelayMessage (packet);
        }

        public void SendRelayMessage (Gs2PacketModel model)
        {
            if (_otherCharacterObjectMap.Count == 0 || _otherCharacterObjectMap.Values.Any (o => o == null))
            {
                return;
            }

            Gs2Manager.Instance.StartSendRelayMessage (model);
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

        public void ApplyRelayMessage (uint connectionId, Gs2PacketModel model)
        {
            if (!_otherCharacterObjectMap.ContainsKey (connectionId) || _otherCharacterObjectMap[connectionId] == null)
            {
                return;
            }

            if (model is CharacterObjectSync characterObjectSync)
            {
                _otherCharacterObjectMap[connectionId].worldPosition = new Vector2 (characterObjectSync.x, characterObjectSync.y);
                _otherCharacterObjectMap[connectionId].SetMoveDirection ((CharacterObject.MoveDirection) characterObjectSync.direction);
                _otherCharacterObjectMap[connectionId].StateMachine.SetNextState (
                    (CharacterState) characterObjectSync.state,
                    _otherCharacterObjectMap[connectionId].StateMachine.CurrentState != (CharacterState) characterObjectSync.state);
            }
            else if (model is GimmickObjectPut gimmickObjectPut)
            {
                StartCoroutine (new GimmickObjectFactory ().CreateGimmick (
                    gimmickObjectPut.gimmickId, connectionId, transform, OnCreateObject,
                    gimmickObjectPut.uniqueId, new Vector2 (gimmickObjectPut.x, gimmickObjectPut.y)));
            }
        }

        public void DestroyOtherPlayerCharacter (uint connectionId)
        {
            Destroy (_otherCharacterObjectMap[connectionId]);
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

            if (obj.HasComponent<CharacterObject> ())
            {
                var character = obj.GetComponent<CharacterObject> ();
                _characterObjects.Add (character);

                if (character.IsPlayer)
                {
                    _playerCharacter = character;
                    _playerCharacter.StateMachine.onStateChanged += OnChangedPlayerState;
                    _playerCharacter.onChangedMoveDirection += OnChangedPlayerDirection;
                }
                else
                {
                    _otherCharacterObjectMap.Add (character.ConnectionId, character);
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
                var gimmick = obj.GetComponent<GimmickObject> ();
                _gimmickObjects.Add (gimmick);

                if (gimmick.IsPlayer)
                {
                    _playerGimmickObjectMap.Add (gimmick.GetHashCode (), gimmick);
                    _gimmickPool.Enqueue (gimmick);
                }
                else
                {
                    _otherGimmickObjectMaps[gimmick.ConnectionId].Add (gimmick.UniqueId, gimmick);
                }
            }

            SetPhysicsMaterialsRecursively (obj);
        }

        private void OnChangedPlayerState (CharacterState before, CharacterState after)
        {
            SendPlayerSyncMessage ();
        }

        private void OnChangedPlayerDirection (CharacterObject.MoveDirection direction)
        {
            SendPlayerSyncMessage ();
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