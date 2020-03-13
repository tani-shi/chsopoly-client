using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chsopoly.Audio;
using Chsopoly.BaseSystem.Audio;
using Chsopoly.BaseSystem.GameScene;
using Chsopoly.BaseSystem.GameScene.Event;
using Chsopoly.BaseSystem.Gs2;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Components;
using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.Gs2.Models;
using Chsopoly.Libs.Extensions;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Ingame
{
    public class IngameScene : BaseGameScene<IngameScene.Param>, IGameSceneOpenCompleteEvent
    {
        public class Param : IGameSceneParam
        {
            public string gatheringId;
            public uint stageId;
            public uint characterId;
            public Dictionary<uint, Profile> otherPlayers;
        }

        public enum AnimationState
        {
            None,
            Ready,
            Go,
            Dead,
            Goal,
            Lose,
            Win,
            Draw,
        }

        private enum CharacterState
        {
            None,
            Dead,
            Goal,
        }

        private enum Result
        {
            None,
            Draw,
            Lose,
            Win,
        }

        [SerializeField]
        private IngameStage _stage = default;
        [SerializeField]
        private PlayerController _controller = default;
        [SerializeField]
        private IngameCamera _camera = default;
        [SerializeField]
        private GimmickBox _gimmickBox = default;
        [SerializeField]
        private LimitTimeCounter _timeCounter = default;
        [SerializeField]
        private PlayerLifeGauge _playerLifeGauge = default;
        [SerializeField]
        private Animator _animator = default;

        public AnimationState CurrentState
        {
            get
            {
                return _state;
            }
        }

        private Dictionary<uint, Profile> _otherPlayers = new Dictionary<uint, Profile> ();
        private string _gatheringId = string.Empty;
        private AnimationState _state = AnimationState.None;
        private Dictionary<uint, CharacterState> _characterStateMap = new Dictionary<uint, CharacterState> ();

        public void OnFinishedAnimation (AnimationState state)
        {
            switch (state)
            {
                case AnimationState.Ready:
                    SetAnimationState (AnimationState.Go);
                    break;

                case AnimationState.Go:
                    _timeCounter.StartCount ();
                    _controller.SetMode (PlayerController.Mode.ControlPlayer);
                    break;

                case AnimationState.Dead:
                    if (!EvaluateResult (IngameSettings.Gs2.PlayerConnectionId, CharacterState.Dead))
                    {
                        _controller.SetMode (PlayerController.Mode.ControlCamera);
                    }
                    break;

                case AnimationState.Goal:
                    if (!EvaluateResult (IngameSettings.Gs2.PlayerConnectionId, CharacterState.Goal))
                    {
                        _controller.SetMode (PlayerController.Mode.ControlCamera);
                    }
                    break;

                case AnimationState.Win:
                case AnimationState.Draw:
                case AnimationState.Lose:
                    Gs2Manager.Instance.StartCloseRoomConnection (() =>
                    {
                        if (!string.IsNullOrEmpty (_gatheringId))
                        {
                            Gs2Manager.Instance.StartCancelGathering (_gatheringId, result =>
                            {
                                GameSceneManager.Instance.ChangeScene (GameSceneType.Title);
                            });
                        }
                        else
                        {
                            GameSceneManager.Instance.ChangeScene (GameSceneType.Title);
                        }
                    });
                    break;
            }
        }

        protected override IEnumerator LoadProc (Param param)
        {
            _gatheringId = param.gatheringId;
            _otherPlayers = param.otherPlayers;

            _characterStateMap.Add (IngameSettings.Gs2.PlayerConnectionId, CharacterState.None);
            foreach (var kv in param.otherPlayers)
            {
                _characterStateMap.Add (kv.Key, CharacterState.None);
            }

            var gimmickIds = MasterDataManager.Instance.Get<GimmickDAO> ().FindAll (o => o.id > 0).ConvertAll (o => o.id).ToArray ();

            yield return _stage.Load (param.stageId, param.characterId, param.otherPlayers, gimmickIds);
            yield return _gimmickBox.LoadTextures (gimmickIds);

            _stage.onGoalPlayer += OnGoalPlayer;
            _stage.onDeadPlayer += OnDeadPlayer;
            _stage.onGoalOtherPlayer += OnGoalOtherPlayer;
            _stage.onDeadOtherPlayer += OnDeadOtherPlayer;
            _controller.SetPlayer (_stage.PlayerCharacter);
            _gimmickBox.SetPool (_stage.GimmickPool);
            _gimmickBox.SetPutGimmickCallback (OnPutGimmick);
            _camera.SetTarget (_stage.PlayerCharacter.transform);
            _camera.SetBounds ((_stage.Field.transform as RectTransform).sizeDelta);
            _timeCounter.SetFrames (_stage.StageData.limitTime * Application.targetFrameRate);
            _playerLifeGauge.SetPlayer (_stage.PlayerCharacter);

            AudioManager.Instance.PlayBgm (Bgm.Ingame);
        }

        void Start ()
        {
            Gs2Manager.Instance.onRelayRealtimeMessage += OnRelayMessage;
            Gs2Manager.Instance.onLeaveRealtimePlayer += OnLeavePlayer;
        }

        void Destroy ()
        {
            Gs2Manager.Instance.onRelayRealtimeMessage -= OnRelayMessage;
            Gs2Manager.Instance.onLeaveRealtimePlayer -= OnLeavePlayer;
        }

        void IGameSceneOpenCompleteEvent.OnOpenComplete ()
        {
            SetAnimationState (AnimationState.Ready);
        }

        private void SetAnimationState (AnimationState state)
        {
            if ((int) _state < (int) state)
            {
                _state = state;
                _animator.Play (state.ToString ());
            }
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

        private void OnLeavePlayer (uint connectionId, Gs2PacketModel model)
        {
            EvaluateResult (connectionId, CharacterState.Dead);
        }

        private void OnGoalPlayer ()
        {
            SetAnimationState (AnimationState.Goal);
        }

        private void OnDeadPlayer ()
        {
            SetAnimationState (AnimationState.Dead);
        }

        private void OnGoalOtherPlayer (uint connectionId)
        {
            EvaluateResult (connectionId, CharacterState.Goal);
        }

        private void OnDeadOtherPlayer (uint connectionId)
        {
            EvaluateResult (connectionId, CharacterState.Dead);
        }

        private void OnChangedPlayerState (CharacterStateMachine.State before, CharacterStateMachine.State after)
        {
            switch (after)
            {
                case CharacterStateMachine.State.Dead:
                    _timeCounter.PauseCount ();
                    SetAnimationState (AnimationState.Dead);
                    break;
                case CharacterStateMachine.State.Appeal:
                    _timeCounter.PauseCount ();
                    break;
            }
        }

        private bool EvaluateResult (uint connectionId, CharacterState state)
        {
            _characterStateMap[connectionId] = state;

            if (_characterStateMap.Any (kv => kv.Value == CharacterState.None))
            {
                // Has not ended playing the player or other players.
                return false;
            }

            if (_characterStateMap.Count > 1)
            {
                if (!_characterStateMap.Any (kv => kv.Value == CharacterState.Dead) ||
                    !_characterStateMap.Any (kv => kv.Value == CharacterState.Goal))
                {
                    SetAnimationState (AnimationState.Draw);
                }
                else if (_characterStateMap[IngameSettings.Gs2.PlayerConnectionId] == CharacterState.Dead)
                {
                    SetAnimationState (AnimationState.Lose);
                }
                else if (_characterStateMap.Count (kv => kv.Value == CharacterState.Goal) == 1)
                {
                    SetAnimationState (AnimationState.Win);
                }
                else
                {
                    SetAnimationState (AnimationState.Draw);
                }
            }
            else
            {
                if (_characterStateMap[IngameSettings.Gs2.PlayerConnectionId] == CharacterState.Dead)
                {
                    SetAnimationState (AnimationState.Lose);
                }
                else
                {
                    SetAnimationState (AnimationState.Win);
                }
            }

            return true;
        }
    }
}