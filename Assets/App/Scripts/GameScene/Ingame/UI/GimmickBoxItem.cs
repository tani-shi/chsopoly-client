using System;
using Chsopoly.Libs.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Ingame.UI
{
    public class GimmickBoxItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<int, Vector2> onPutGimmick;

        [SerializeField]
        private int _index = 0;
        [SerializeField]
        private Image _image = default;
        [SerializeField]
        private float _dragToPickThreshold = 10f;
        [SerializeField]
        private Canvas _stageUICanvas = default;
        [SerializeField]
        private Camera _stageCamera = default;

        private Vector2 _dragStartPos = Vector2.zero;
        private Image _draggingImage = null;
        private Sprite _texture = null;
        private uint _gimmickId = 0;

        public void Initialize (uint id, Sprite texture)
        {
            if (id == _gimmickId)
            {
                return;
            }

            _gimmickId = id;
            _texture = texture;

            if (_image.sprite != null)
            {
                Destroy (_image.sprite);
                _image.sprite = null;
                _image.color = Color.clear;
            }
            if (texture != null)
            {
                _image.sprite = Instantiate (_texture);
                _image.color = Color.white;
            }
        }

        void IDragHandler.OnDrag (PointerEventData eventData)
        {
            if (_gimmickId == 0 || _texture == null)
            {
                return;
            }

            if (_draggingImage != null)
            {
                _draggingImage.transform.position = ScreenToWorldPoint (eventData.position);
            }
            else if ((_dragStartPos - eventData.position).sqrMagnitude > _dragToPickThreshold * _dragToPickThreshold)
            {
                _draggingImage = CreateDraggingImageObject ();
                _draggingImage.transform.position = ScreenToWorldPoint (eventData.position);
            }
        }

        void IPointerDownHandler.OnPointerDown (PointerEventData eventData)
        {
            if (_gimmickId == 0 || _texture == null)
            {
                return;
            }

            _dragStartPos = eventData.position;
        }

        void IPointerUpHandler.OnPointerUp (PointerEventData eventData)
        {
            if (_gimmickId == 0 || _texture == null)
            {
                return;
            }

            _dragStartPos = Vector2.zero;

            if (_draggingImage != null)
            {
                Destroy (_draggingImage.gameObject);
                onPutGimmick.SafeInvoke (_index, eventData.position);
            }
        }

        private Image CreateDraggingImageObject ()
        {
            var obj = new GameObject ("GimmickDraggingImage");
            obj.layer = IngameSettings.Layers.Gimmick;
            obj.transform.SetParent (_stageUICanvas.transform);
            obj.transform.position = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            var image = obj.AddComponent<Image> ();
            image.sprite = Instantiate (_texture);
            image.color = IngameSettings.Gimmick.DraggingColor;
            image.SetNativeSize ();

            return image;
        }

        private Vector3 ScreenToWorldPoint (Vector2 point)
        {
            var worldPoint = _stageCamera.ScreenToWorldPoint (point);
            worldPoint.z = 0;
            return worldPoint;
        }
    }
}