using System.Collections;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Ingame.Components
{
    public class CharacterMonitorIcon : MonoBehaviour
    {
        [SerializeField]
        private Image _iconImage = default;
        [SerializeField]
        private Image _bgImage = default;
        [SerializeField]
        private CanvasGroup _canvasGroup = default;
        [SerializeField]
        private float _offsetCameraBounds = 10f;
        [SerializeField]
        private IngameCamera _stageCamera = default;
        [SerializeField]
        private Camera _uiCamera = default;

        private CharacterObject _targetCharacter = null;
        private RectTransform _transform = null;

        public IEnumerator Load (uint characterId)
        {
            var data = MasterDataManager.Instance.Get<CharacterDAO> ().Get (characterId);
            var handle = Addressables.LoadAssetAsync<Sprite> (IngameSettings.Paths.CharacterIcon (data.assetName));
            yield return handle;
            _iconImage.sprite = handle.Result;
            _iconImage.SetNativeSize ();
        }

        public void StartMonitoring (CharacterObject character)
        {
            _targetCharacter = character;
            _transform = transform as RectTransform;
            _targetCharacter.StateMachine.onStateChanged += OnChangedCharacterState;
        }

        void Update ()
        {
            if (_targetCharacter != null && _targetCharacter.StateMachine.CurrentState != CharacterStateMachine.State.Dead)
            {
                var stageCameraRect = _stageCamera.ViewRect;
                var characterRect = _targetCharacter.ColliderRect;

                if (_transform != null)
                {
                    var offset = new Vector2 (_offsetCameraBounds + _transform.rect.size.x / 2, _offsetCameraBounds + _transform.rect.size.y / 2);
                    var vBounds = new Vector2 (stageCameraRect.xMin + offset.x, stageCameraRect.xMax - offset.x);
                    var hBounds = new Vector2 (stageCameraRect.yMin + offset.y, stageCameraRect.yMax - offset.y);
                    var position = characterRect.center;
                    position.x = Mathf.Clamp (position.x, vBounds.x, vBounds.y);
                    position.y = Mathf.Clamp (position.y, hBounds.x, hBounds.y);
                    var diff = characterRect.center - position;
                    var degree = (Mathf.Atan2 (diff.x, diff.y) * 180.0f / Mathf.PI) + 90.0f;
                    position = _stageCamera.MainCamera.WorldToScreenPoint (position);
                    position = _uiCamera.ScreenToWorldPoint (position) * _uiCamera.fieldOfView;
                    _transform.anchoredPosition = position;
                    _bgImage.transform.rotation = Quaternion.Euler (0, 0, degree);
                }

                _canvasGroup.alpha = stageCameraRect.Overlaps (characterRect) ? 0 : 1;
            }
            else
            {
                _canvasGroup.alpha = 0;
            }
        }

        private void OnChangedCharacterState (CharacterStateMachine.State before, CharacterStateMachine.State after)
        {
            if (after == CharacterStateMachine.State.Damage ||
                after == CharacterStateMachine.State.Dying ||
                after == CharacterStateMachine.State.Dead)
            {
                _iconImage.color = Color.gray;
            }
            else
            {
                _iconImage.color = Color.white;
            }
        }
    }
}