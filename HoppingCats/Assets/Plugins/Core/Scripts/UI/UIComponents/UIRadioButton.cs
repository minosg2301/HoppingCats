using Doozy.Engine.UI;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace moonNest
{
    [RequireComponent(typeof(UIButton))]
    public class UIRadioButton : MonoBehaviour
    {
        public int defaultValue;
        public RadioNode[] nodes;
        public UnityEvent<int> onChanged;

        int index = 0;

        public int Value { get; private set; }
        public string DisplayValue { get; private set; }

        private UIButton _button;
        public UIButton UIButton { get { if (!_button) _button = GetComponent<UIButton>(); return _button; } }

        protected virtual void Start()
        {
            SetIndex(GetIndex(defaultValue));
            UIButton.OnClick.OnTrigger.Event.AddListener(OnClick);
        }

        void OnClick()
        {
            if (nodes.Length == 0) return;

            index = (index + 1) % nodes.Length;
            SetIndex(index);
            onChanged.Invoke(Value);
        }

        void SetIndex(int index)
        {
            Value = nodes[index].value;
            DisplayValue = nodes[index].name;
            UIButton.SetLabelText(DisplayValue);
        }

        int GetIndex(int value)
        {
            if (nodes.Length == 0) return -1;
            for (int i = 0; i < nodes.Length; i++)
                if (nodes[i].value == value) return i;
            return -1;
        }
    }

    [Serializable]
    public class RadioNode
    {
        public string name;
        public int value;
    }
}