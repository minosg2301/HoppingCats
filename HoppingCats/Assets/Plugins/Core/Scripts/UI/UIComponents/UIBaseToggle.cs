using Doozy.Engine.Progress;
using Doozy.Engine.UI;
using UnityEngine;

namespace moonNest
{
    [RequireComponent(typeof(UIButton))]
    public abstract class UIBaseToggle : MonoBehaviour
    {
        public Progressor progressor;
        public UISwitcher switcher;

        private UIButton _button;
        public UIButton Button { get { if(!_button) _button = GetComponent<UIButton>(); return _button; } }

        protected abstract bool Value { get; set; }
        protected abstract void OnStateChanged(bool value);

        protected virtual void Awake()
        {
            Button.OnClick.OnTrigger.Event.AddListener(OnClicked);
        }

        protected virtual void Start()
        {
            UpdateUI();
        }

        protected virtual void OnClicked()
        {
            Value = !Value;
            OnStateChanged(Value);
            UpdateUI();
        }

        protected virtual void UpdateUI()
        {
            bool isOn = Value;
            if(progressor) progressor.SetValue(isOn ? 1 : 0);
            if(switcher) switcher.On = isOn;
        }
    }
}