using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.Popup
{
    public class GimmickListPopupItem : MonoBehaviour
    {
        [SerializeField]
        private Image _gimmickImage = default;
        [SerializeField]
        private List<Image> _stateFrames = default;

        public enum State
        {
            Unknown,
            Active,
            Inactive,
        }

        public State CurrentState
        {
            get
            {
                for (int i = 0; i < _stateFrames.Count; i++)
                {
                    if (_stateFrames[i].gameObject.activeSelf)
                    {
                        return (State) i;
                    }
                }
                return State.Unknown;
            }
        }

        public void SetState (State state)
        {
            for (int i = 0; i < _stateFrames.Count; i++)
            {
                _stateFrames[i].gameObject.SetActive (i == (int) state);
                _gimmickImage.gameObject.SetActive (state != State.Unknown);
            }
        }

        public void SetImage (Sprite sprite)
        {
            _gimmickImage.sprite = sprite;
            _gimmickImage.SetNativeSize ();
        }
    }
}