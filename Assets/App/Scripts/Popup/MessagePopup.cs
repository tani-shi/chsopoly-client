using System.Collections;
using Chsopoly.BaseSystem.Popup;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.Popup.Generated
{
    public class MessagePopup : BasePopup<MessagePopup.Param>
    {
        public class Param : IPopupParam
        {
            public string message;
        }

        [SerializeField]
        private Text _messageText = default;

        protected override void Initialize ()
        {
            _messageText.text = param.message;
        }
    }
}