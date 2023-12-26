using Doozy.Engine.UI;
using UnityEngine;

namespace moonNest
{
    [RequireComponent(typeof(UIButton))]
    public class UIIAPRestore : MonoBehaviour
    {
        private UIButton _button;
        public UIButton Button { get { if (!_button) _button = GetComponent<UIButton>(); return _button; } }

        void Awake()
        {
            Button.OnClick.OnTrigger.Event.AddListener(() => IAPManager.Ins.RestoreTransaction());
        }
    }
}