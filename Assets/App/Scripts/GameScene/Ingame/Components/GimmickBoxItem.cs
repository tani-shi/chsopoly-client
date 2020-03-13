using System;
using System.Collections.Generic;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.Libs.Extensions;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Ingame.Components
{
    public class GimmickBoxItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<int, Vector2> onPutGimmick;

        [SerializeField]
        private int _index = 0;
        [SerializeField]
        private Image _gimmickImage = default;
        [SerializeField]
        private float _dragToPickThreshold = 10f;
        [SerializeField]
        private Transform _draggingRoot = default;
        [SerializeField]
        private Camera _stageCamera = default;
        [SerializeField]
        private Image _coolTimeCountImage = default;
        [SerializeField]
        private List<Sprite> _coolTimeCountSprites = default;
        [SerializeField]
        private GameObject _touchBlocker = default;

        public uint GimmickId
        {
            get
            {
                return _gimmickId;
            }
        }

        private Vector2 _dragStartPos = Vector2.zero;
        private Image _draggingImage = null;
        private Sprite _texture = null;
        private uint _gimmickId = 0;
        private float _coolTimeCounter = 0f;
        private float _coolTime = 0f;

        public void Initialize (uint id, Sprite texture, bool resetCoolTime = false)
        {
            _gimmickId = id;
            _texture = texture;

            if (_gimmickImage.sprite != null)
            {
                Destroy (_gimmickImage.sprite);
                _gimmickImage.sprite = null;
                _gimmickImage.color = Color.clear;
            }
            if (texture != null)
            {
                _gimmickImage.sprite = Instantiate (_texture);
                _gimmickImage.color = Color.white;
            }

            if (_touchBlocker != null)
            {
                if (id != 0 && resetCoolTime)
                {
                    var data = MasterDataManager.Instance.Get<GimmickDAO> ().Get (id);
                    _coolTime = data.coolTime;
                    _coolTimeCounter = 0;
                    UpdateCoolTimeCount (Mathf.CeilToInt (data.coolTime));
                }
                else
                {
                    UpdateCoolTimeCount (-1);
                }
            }
        }

        void Update ()
        {
            if (_touchBlocker == null || !_touchBlocker.activeSelf)
            {
                return;
            }

            _coolTimeCounter = Mathf.Min (_coolTime, _coolTimeCounter + Time.deltaTime);
            UpdateCoolTimeCount (Mathf.CeilToInt (_coolTime - _coolTimeCounter));
        }

        void IDragHandler.OnDrag (PointerEventData eventData)
        {
            if (_gimmickId == 0 || _texture == null || _touchBlocker == null || _touchBlocker.activeSelf)
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
            if (_gimmickId == 0 || _texture == null || _touchBlocker == null || _touchBlocker.activeSelf)
            {
                return;
            }

            _dragStartPos = eventData.position;
        }

        void IPointerUpHandler.OnPointerUp (PointerEventData eventData)
        {
            if (_gimmickId == 0 || _texture == null || _touchBlocker == null || _touchBlocker.activeSelf)
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
            obj.transform.SetParent (_draggingRoot);
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

        private void UpdateCoolTimeCount (int time)
        {
            if (time > 0)
            {
                var sprite = _coolTimeCountSprites[Mathf.Min (IngameSettings.Rules.MaxGimmickCoolTime, time) - 1];
                if (_coolTimeCountImage.sprite != sprite)
                {
                    _coolTimeCountImage.sprite = sprite;
                    _coolTimeCountImage.SetNativeSize ();
                }
            }

            _touchBlocker.SetActive (time > 0);
        }
    }
}