using System;
using UnityEngine;
using UnityEngine.Events;

namespace Chsopoly.GameScene.Components.Button
{
    public class SwitchButton : StateButton<SwitchButton.Switch>
    {
        public enum Switch
        {
            On,
            Off,
        }

        [Serializable]
        public class SwitchEvent : UnityEvent<bool> { }

        public SwitchEvent onSwitch;

        [SerializeField]
        private bool _isOn = false;

        public bool IsOn
        {
            get
            {
                return _isOn;
            }
            set
            {
                _isOn = value;
                ApplyState (value);
            }
        }

        protected override void Start ()
        {
            base.Start ();

            ApplyState (_isOn);
        }

        protected override void OnChangedState (Switch state)
        {
            _isOn = state == Switch.On;
            onSwitch.Invoke (state == Switch.On);
        }

        private void ApplyState (bool isOn)
        {
            SetState (isOn ? Switch.On : Switch.Off);
        }
    }
}