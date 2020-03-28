using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.BaseSystem.Popup;
using Chsopoly.BaseSystem.UserData;
using Chsopoly.Components.Button;
using Chsopoly.GameScene.Ingame;
using Chsopoly.MasterData.DAO.Ingame;
using Chsopoly.MasterData.VO.Ingame;
using Chsopoly.UserData.Entity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Chsopoly.Popup.Generated
{
    public class GimmickListPopup : BasePopup
    {
        private const int MaxGimmickGridCount = 10;

        [SerializeField]
        private List<GimmickListPopupItem> _itemList = default;
        [SerializeField]
        private SimpleButton _nextButton = default;
        [SerializeField]
        private SimpleButton _previousButton = default;
        [SerializeField]
        private Text _descriptionText = default;

        private int _pageIndex = 0;
        private GimmickVO[] _gimmickDataArray = null;
        private Dictionary<uint, bool> _changedStateMap = new Dictionary<uint, bool> ();
        private Dictionary<uint, Sprite> _loadedGimmickSpriteMap = new Dictionary<uint, Sprite> ();

        protected override IEnumerator LoadProc ()
        {
            foreach (var data in MasterDataManager.Instance.Get<GimmickDAO> ())
            {
                var handle = Addressables.LoadAssetAsync<Sprite> (IngameSettings.Paths.GimmickCapture (data.assetName));
                yield return handle;
                _loadedGimmickSpriteMap.Add (data.id, handle.Result);
            }
        }

        protected override void Initialize ()
        {
            _gimmickDataArray = MasterDataManager.Instance.Get<GimmickDAO> ().ToArray ();

            ChangePage (0);

            _nextButton.gameObject.SetActive (_gimmickDataArray.Length > MaxGimmickGridCount);
            _previousButton.gameObject.SetActive (_gimmickDataArray.Length > MaxGimmickGridCount);

            for (int i = 0; i < _itemList.Count; i++)
            {
                var index = i;
                _itemList[i].GetComponent<SimpleButton> ().onClick.AddListener (_ => OnClickItem (index));
            }
        }

        public void OnClickNext ()
        {
            var index = _pageIndex + 1;
            if (index >= Mathf.CeilToInt ((float) _gimmickDataArray.Length / (float) MaxGimmickGridCount))
            {
                index = 0;
            }
            ChangePage (index);
        }

        public void OnClickPrevious ()
        {
            var index = _pageIndex - 1;
            if (index < 0)
            {
                index = Mathf.CeilToInt ((float) _gimmickDataArray.Length / (float) MaxGimmickGridCount) - 1;
            }
            ChangePage (index);
        }

        public void OnClickItem (int index)
        {
            if (_itemList[index].CurrentState != GimmickListPopupItem.State.Unknown)
            {
                var gimmickId = _gimmickDataArray[index + (_pageIndex * MaxGimmickGridCount)].id;
                var isActive = _itemList[index].CurrentState == GimmickListPopupItem.State.Active;

                if (_changedStateMap.ContainsKey (gimmickId))
                {
                    _changedStateMap[gimmickId] = !isActive;
                }
                else
                {
                    _changedStateMap.Add (gimmickId, !isActive);
                }

                _itemList[index].SetState (isActive ? GimmickListPopupItem.State.Inactive : GimmickListPopupItem.State.Active);
            }
        }

        public void OnClickOk ()
        {
            foreach (var kv in _changedStateMap)
            {
                var data = UserDataManager.Instance.Gimmick.First (o => o.gimmickId == kv.Key);
                data.isActive = kv.Value;
                data.IsDirty = true;
            }

            UserDataManager.Instance.Save ();

            Close ();
        }

        private void ChangePage (int page)
        {
            _pageIndex = page;

            for (int i = 0; i < MaxGimmickGridCount; i++)
            {
                var itemIndex = (MaxGimmickGridCount * page) + i;

                if (_gimmickDataArray.Length > (MaxGimmickGridCount * page) + i)
                {
                    var gimmickId = _gimmickDataArray[itemIndex].id;
                    var userData = UserDataManager.Instance.Gimmick.FirstOrDefault (o => o.gimmickId == gimmickId);

                    _itemList[i].gameObject.SetActive (true);

                    if (userData == null)
                    {
                        _itemList[i].SetState (GimmickListPopupItem.State.Unknown);
                    }
                    else
                    {
                        _itemList[i].SetState (userData.isActive ? GimmickListPopupItem.State.Active : GimmickListPopupItem.State.Inactive);
                    }

                    _itemList[i].SetImage (_loadedGimmickSpriteMap[gimmickId]);
                }
                else
                {
                    _itemList[i].gameObject.SetActive (false);
                }
            }
        }
    }
}