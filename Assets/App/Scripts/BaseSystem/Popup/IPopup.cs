using System;
using System.Collections;
using UnityEngine;

namespace Chsopoly.BaseSystem.Popup
{
    public interface IPopup
    {
        event Action onOpened;
        event Action onClosed;

        bool IsReady { get; }
        bool IsAnimationPlaying { get; }
        bool IsOpen { get; }
        GameObject gameObject { get; }

        IEnumerator Load (IPopupParam param);
        void Open ();
        void Close ();
    }
}