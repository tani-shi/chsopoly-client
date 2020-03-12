using System;
using Chsopoly.Libs.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Ingame.Components
{
    public class LimitTimeCounter : MonoBehaviour
    {
        private const int Min = 60;

        public event Action onTimeout;

        [SerializeField]
        private Text _limitTimeText = default;

        public enum State
        {
            None,
            Ready,
            Counting,
            Paused,
        }

        public int RemainingTime
        {
            get
            {
                return Mathf.CeilToInt ((float) _frameTimer / (float) Application.targetFrameRate);
            }
        }

        public int RemainingFrames
        {
            get
            {
                return _frameTimer;
            }
        }

        private int _frameTimer = 0;
        private State _state = State.None;

        public void SetFrames (int frames)
        {
            _frameTimer = frames;
            _state = State.Ready;
        }

        public void StartCount ()
        {
            if (_state == State.Ready || _state == State.Paused)
            {
                _state = State.Counting;
            }
            else
            {
                Debug.LogWarning (string.Format ("[{0}] Call SetFrames before Start.", typeof (LimitTimeCounter).Name));
            }
        }

        public void PauseCount ()
        {
            if (_state == State.Counting)
            {
                _state = State.Paused;
            }
        }

        void Update ()
        {
            var sec = RemainingTime;
            _limitTimeText.text = string.Format ("{0:D2}:{1:D2}", sec / Min, sec % Min);
        }

        void FixedUpdate ()
        {
            if (_state != State.Counting)
            {
                return;
            }

            if (_frameTimer > 0)
            {
                _frameTimer--;

                if (_frameTimer <= 0)
                {
                    _state = State.None;
                    onTimeout.SafeInvoke ();
                }
            }
        }
    }
}