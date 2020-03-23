using System;
using System.Collections;
using System.Collections.Generic;
using Chsopoly.Libs;
using Chsopoly.Libs.Extensions;
using Chsopoly.Popup;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Chsopoly.BaseSystem.Popup
{
    public class PopupManager : SingletonMonoBehaviour<PopupManager>
    {
        [SerializeField]
        private Canvas _popupCanvas = default;
        [SerializeField]
        private Canvas _errorPopupCanvas = default;

        private List<IPopup> _popupStack = new List<IPopup> ();
        private List<IPopup> _errorPopupStack = new List<IPopup> ();

        void Start ()
        {
            _popupCanvas.gameObject.SetActive (false);
            _errorPopupCanvas.gameObject.SetActive (false);
        }

        public void OpenPopup (PopupType type, IPopupParam param)
        {
            StartCoroutine (LoadPopup (type, param, (popup) =>
            {
                popup.Open ();
                _popupStack.Add (popup);
                popup.onClosed += () => OnClosedPopup (popup);
            }));

            _popupCanvas.gameObject.SetActive (true);
        }

        public void OpenErrorPopup (PopupType type, IPopupParam param)
        {
            StartCoroutine (LoadPopup (type, param, (popup) =>
            {
                popup.Open ();
                _errorPopupStack.Add (popup);
                popup.onClosed += () => OnClosedErrorPopup (popup);
            }));

            _errorPopupCanvas.gameObject.SetActive (true);
        }

        public void DestroyAllPopup ()
        {
            foreach (var popup in _popupStack)
            {
                Destroy (popup.gameObject);
            }
            foreach (var popup in _errorPopupStack)
            {
                Destroy (popup.gameObject);
            }
            _popupStack.Clear ();
            _errorPopupStack.Clear ();
        }

        private IEnumerator LoadPopup (PopupType type, IPopupParam param, Action<IPopup> callback)
        {
            var handle = Addressables.LoadAssetAsync<GameObject> (PopupTypeHelper.GetAssetPath (type));
            yield return handle;
            if (handle.Result != null)
            {
                if (handle.Result.GetComponent<IPopup> () is IPopup popup)
                {
                    yield return popup.Load (param);
                    callback.SafeInvoke (popup);
                }
                else
                {
                    Debug.LogError ("Failed to load a popup because it has no popup components. " + PopupTypeHelper.GetAssetPath (type));
                }
            }
        }

        private void OnClosedPopup (IPopup popup)
        {
            if (_popupStack.Contains (popup))
            {
                _popupStack.Remove (popup);
            }

            Destroy (popup.gameObject);

            if (_popupStack.Count == 0)
            {
                _popupCanvas.gameObject.SetActive (false);
            }
        }

        private void OnClosedErrorPopup (IPopup popup)
        {
            if (_errorPopupStack.Contains (popup))
            {
                _errorPopupStack.Remove (popup);
            }

            Destroy (popup.gameObject);

            if (_errorPopupStack.Count == 0)
            {
                _errorPopupCanvas.gameObject.SetActive (false);
            }
        }
    }
}