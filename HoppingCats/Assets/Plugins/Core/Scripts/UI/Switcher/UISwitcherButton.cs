using Doozy.Engine.UI;
using UnityEngine;

namespace moonNest
{
    [RequireComponent(typeof(UIButton))]
    public class UISwitcherButton : UISwitcher
    {
        private UIButton _button;
        public UIButton UIButton { get { if(!_button) _button = GetComponent<UIButton>(); return _button; } }

        private void Start()
        {
            UIButton.OnClick.OnTrigger.Event.AddListener(Toggle);
        }
    }
}