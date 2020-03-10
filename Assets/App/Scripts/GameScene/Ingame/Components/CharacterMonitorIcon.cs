using Chsopoly.GameScene.Ingame.Object.Character;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Ingame.Components
{
    public class CharacterMonitorIcon : MonoBehaviour
    {
        [SerializeField]
        private RawImage _renderRawImage = default;
        [SerializeField]
        private CanvasGroup _canvasGroup = default;
        [SerializeField]
        private float _offsetCameraBounds = 10f;

        private CharacterObject _targetCharacter = null;
        private Transform _cameraTransform = null;
        private Camera _monitoringCamera = default;
        private RectTransform _transform = null;
        private IngameCamera _stageCamera = null;

        public void Initialize (CharacterObject character)
        {
            _targetCharacter = character;
            _transform = transform as RectTransform;

            var size = (int) character.Collider.size.y;
            var renderTexture = new RenderTexture (size, size, 16, RenderTextureFormat.ARGB64);
            renderTexture.Create ();
            _renderRawImage.texture = renderTexture;

            var camera = new GameObject ("CharacterCamera").AddComponent<Camera> ();
            camera.transform.SetParent (transform);
            camera.orthographic = true;
            camera.depth = -1;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.clear;
            camera.orthographicSize = size / 2;
            camera.targetTexture = renderTexture;
            camera.cullingMask = LayerMask.GetMask (LayerMask.LayerToName (IngameSettings.Layers.Character));

            _monitoringCamera = camera;
            _cameraTransform = camera.transform;
            _stageCamera = GameObject.FindObjectOfType (typeof (IngameCamera)) as IngameCamera;
        }

        void Update ()
        {
            if (_targetCharacter != null)
            {
                var stageCameraRect = _stageCamera.ViewRect;
                var characterRect = _targetCharacter.ColliderRect;

                if (_cameraTransform != null)
                {
                    _cameraTransform.position = new Vector3 (characterRect.center.x, characterRect.center.y, -10);
                }
                if (_transform != null)
                {
                    var offset = new Vector2 (_offsetCameraBounds + _transform.rect.size.x / 2, _offsetCameraBounds + _transform.rect.size.y / 2);
                    var vBounds = new Vector2 (stageCameraRect.xMin + offset.x, stageCameraRect.xMax - offset.x);
                    var hBounds = new Vector2 (stageCameraRect.yMin + offset.y, stageCameraRect.yMax - offset.y);
                    var position = characterRect.center;
                    position.x = Mathf.Clamp (position.x, vBounds.x, vBounds.y);
                    position.y = Mathf.Clamp (position.y, hBounds.x, hBounds.y);
                    _transform.position = position;
                }

                _canvasGroup.alpha = stageCameraRect.Overlaps (characterRect) ? 0 : 1;
            }
        }
    }
}