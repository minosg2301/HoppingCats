using Doozy.Engine.Progress;
using Doozy.Engine.UI;
using System;
using UnityEngine;

namespace moonNest
{
    public class UITabItem : MonoBehaviour
    {
        public UIButton button;
        public UITabContent tabContent;
        public Progressor progressor;
        public UISwitcher switcher;

        private RectTransform _rect;
        public RectTransform RectTransform { get { if(!_rect) _rect = GetComponent<RectTransform>(); return _rect; } }

        public Action<UITabItem> onClick;

        public string Name => tabContent ? tabContent.name : "";

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                button.Interactable = !value;
                if (switcher) switcher.On = value;
                if(progressor) progressor.SetValue(selected ? 1 : 0);

                OnSelectionChanged(selected);
            }
        }

        protected virtual void Reset()
        {
            if(!button) button = GetComponentInChildren<UIButton>();
            if(!progressor) progressor = GetComponentInChildren<Progressor>();
            if(!switcher) switcher = GetComponentInChildren<UISwitcher>();
        }

        protected virtual void Start()
        {
            button.OnClick.OnTrigger.Event.AddListener(OnClick);
        }

        protected virtual void OnClick()
        {
            onClick?.Invoke(this);
        }

        // allow called outside class
        public void InvokeClick() { OnClick(); }

        protected virtual void OnSelectionChanged(bool selected) { }
    }
}