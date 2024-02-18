using Doozy.Engine.UI;
using UnityEngine;

namespace moonNest
{
    [RequireComponent(typeof(UIButton))]
    public class UIOpenLinkButton : MonoBehaviour
    {
        public LinkUrl link;

        private UIButton _button;
        public UIButton UIButton { get { if (!_button) _button = GetComponent<UIButton>(); return _button; } }

        void Start()
        {
            UIButton.OnClick.OnTrigger.Event.AddListener(() =>
            {
                link.Open();
            });
        }
    }
}