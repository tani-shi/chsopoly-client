using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chsopoly.BaseSystem.Gs2;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Components;
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
using GimmickState = Chsopoly.GameScene.Ingame.Object.Gimmick.GimmickStateMachine.State;

namespace Chsopoly.GameScene.Ingame
{
    public class IngameStage : MonoBehaviour
    {
        public event Action onGoalPlayer;
        public event Action onDeadPlayer;
        public event Action<uint> onGoalOtherPlayer;
        public event Action<uint> onDeadOtherPlayer;

        [SerializeField]
        private Transform _characterContainer = default;
        [SerializeField]
        private Transform _gimmickContainer = default;

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

        public IEnumerable<CharacterObject> OtherCharacters
        {
            get
            {
                return _otherCharacterObjectMap.Values;
            }
        }

        public GimmickObjectPool GimmickPool
        {
            get
            {
                return _gimmickPool;
            }
        }

        public StageVO StageData
        {
            get
            {
                return _stageData;
            }
        }

        private List<GameObject> _stageObjects = new List<GameObject> ();
        private List<CharacterObject> _characterObjects = new List<CharacterObject> ();
        private CharacterObject _playerCharacter = null;
        private List<GimmickObject> _gimmickObjects = new List<GimmickObject> ();
        private Dictionary<uint, CharacterObject> _otherCharacterObjectMap = new Dictionary<uint, CharacterObject> ();
        private GameObject _field = null;
        private StageVO _stageData = null;
        private uint[] _gimmickLotteryTable = null;
        private GimmickObjectPool _gimmickPool = null;

        public IEnumerator Load (uint stageId, uint playerCharacterId, Dictionary<uint, Profile> otherPlayers, uint[] gimmickIds)
        {
            if (_stageData != null)
            {
                Debug.LogWarning ("The stage was already loaded.");
                yield break;
            }

            _stageData = MasterDataManager.Instance.Get<StageDAO> ().Get (stageId);
            _gimmickPool = new GameObject (nameof (GimmickObjectPool)).AddComponent<GimmickObjectPool> ();
            _gimmickPool.transform.SetParent (transform);

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
            yield return new CharacterObjectFactory ().CreateCharacter (playerCharacterId, IngameSettings.Gs2.PlayerConnectionId, _characterContainer, OnCreateObject);
            foreach (var kv in otherPlayers)
            {
                yield return new CharacterObjectFactory ().CreateCharacter (kv.Value.characterId, kv.Key, _characterContainer, OnCreateObject);
            }
            for (int i = 0; i < IngameSettings.Rules.MaxGimmickBoxCount + 1; i++)
            {
                yield return new GimmickObjectFactory ().CreateGimmick (DrawGimmickId (), IngameSettings.Gs2.PlayerConnectionId, _gimmickPool.transform, OnCreateObject);
            }

            Physics2D.gravity = new Vector2 (0, IngameSettings.Field.Gravity (_stageData.fieldGravity));
            Physics2D.IgnoreLayerCollision (IngameSettings.Layers.Character, IngameSettings.Layers.Character);

            OnLoadComplete ();
        }

        public void PutGimmick (int index, Vector3 position)
        {
            var gimmick = _gimmickPool.Dequeue (index, _gimmickContainer);
            if (gimmick != null)
            {
                gimmick.transform.position = position;
            }

            StartCoroutine (new GimmickObjectFactory ().CreateGimmick (DrawGimmickId (), IngameSettings.Gs2.PlayerConnectionId, _gimmickPool.transform, OnCreateObject));

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
                targetGimmickConnectionId = _playerCharacter.TargetGimmick != null ? _playerCharacter.TargetGimmick.ConnectionId : 0,
                targetGimmickUniqueId = _playerCharacter.TargetGimmick != null ? _playerCharacter.TargetGimmick.UniqueId : 0,
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
                var targetGimmickConnectionId = characterObjectSync.targetGimmickConnectionId == 0 ?
                    connectionId : characterObjectSync.targetGimmickConnectionId;
                _otherCharacterObjectMap[connectionId].TargetGimmick = _gimmickObjects.FirstOrDefault (
                    o => o.ConnectionId == targetGimmickConnectionId && o.UniqueId == characterObjectSync.targetGimmickUniqueId);
                _otherCharacterObjectMap[connectionId].worldPosition = new Vector2 (characterObjectSync.x, characterObjectSync.y);
                _otherCharacterObjectMap[connectionId].SetMoveDirection ((CharacterObject.MoveDirection) characterObjectSync.direction);
                _otherCharacterObjectMap[connectionId].StateMachine.SetNextState ((CharacterState) characterObjectSync.state, true);

                switch ((CharacterState) characterObjectSync.state)
                {
                    case CharacterState.Appeal:
                        onGoalOtherPlayer.SafeInvoke (connectionId);
                        break;
                    case CharacterState.Dead:
                        onDeadOtherPlayer.SafeInvoke (connectionId);
                        break;
                }
            }
            else if (model is GimmickObjectPut gimmickObjectPut)
            {
                StartCoroutine (new GimmickObjectFactory ().CreateGimmick (
                    gimmickObjectPut.gimmickId, connectionId, transform, OnCreateObject,
                    gimmickObjectPut.uniqueId, new Vector2 (gimmickObjectPut.x, gimmickObjectPut.y)));
            }
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
                    _gimmickPool.Enqueue (gimmick);
                }

                gimmick.StateMachine.onStateChanged += (prev, after) =>
                {
                    if (after == GimmickState.Dead)
                    {
                        if (_gimmickObjects.Contains (gimmick))
                        {
                            _gimmickObjects.Remove (gimmick);
                        }

                        Destroy (gimmick.gameObject);
                    }
                };
            }

            SetPhysicsMaterialsRecursively (obj);
        }

        private void OnChangedPlayerState (CharacterState before, CharacterState after)
        {
            switch (after)
            {
                case CharacterState.Appeal:
                    onGoalPlayer.SafeInvoke ();
                    break;
                case CharacterState.Dead:
                    onDeadPlayer.SafeInvoke ();
                    break;
            }

            if (before == CharacterState.Jump && after == CharacterState.Fall)
            {
                return;
            }

            SendPlayerSyncMessage ();
        }

        private void OnChangedPlayerDirection (CharacterObject.MoveDirection direction)
        {
            if (_playerCharacter.StateMachine.CurrentState != CharacterState.Run &&
                _playerCharacter.StateMachine.CurrentState != CharacterState.Fall &&
                _playerCharacter.StateMachine.CurrentState != CharacterState.Jump)
            {
                return;
            }

            SendPlayerSyncMessage ();
        }

        private uint DrawGimmickId ()
        {
            return _gimmickLotteryTable[UnityEngine.Random.Range (0, _gimmickLotteryTable.Length)];
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