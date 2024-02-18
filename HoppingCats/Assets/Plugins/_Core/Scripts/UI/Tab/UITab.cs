using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace moonNest
{
    public class UITab : MonoBehaviour
    {
        public int defaultIndex;
        public RectTransform labelContainer;
        public RectTransform contentContainer;
        public RectTransform cursor;
        public float scrollDuration = 0.5f;

        private RectTransform cursorTarget;
        private float cursorAnimDt;
        private bool cursorTargetUpdate;
        private Vector2 cursorSize;
        private Vector3 cursorPosition;

        public Action<UITabItem> onTabSelected = delegate { };

        public UITabContent CurrTabContent { get; protected set; }
        public UITabItem CurrTabItem { get; protected set; }
        public int CurrTabIndex { get; protected set; }

        private List<UITabItem> _tabItems;
        public List<UITabItem> TabItems
        {
            get
            {
                if (_tabItems == null || _tabItems.Count == 0)
                    _tabItems = labelContainer.GetComponentsInChildren<UITabItem>().ToList();

                return _tabItems;
            }
        }

        protected virtual void Reset()
        {
            if (!labelContainer) labelContainer = transform.Find("Labels") as RectTransform;
            if (!contentContainer) contentContainer = transform.Find("Contents") as RectTransform;
        }

        protected virtual void Awake()
        {
            TabItems.ForEach(tabItem =>
            {
                tabItem.Selected = false;
                tabItem.onClick += i => FocusTab(i);
                if (tabItem.tabContent)
                {
                    tabItem.Selected = tabItem.tabContent == CurrTabContent;
                    if (!tabItem.Selected) tabItem.tabContent.Show(false);
                }
            });
        }

        protected virtual void Start()
        {
            if (!CurrTabContent)
            {
                SetFocusTab(TabItems[defaultIndex]);
            }
        }

        protected virtual void Update()
        {
            if (cursor)
            {
                if (cursorTargetUpdate)
                {
                    cursorAnimDt += Time.deltaTime;
                    cursorTargetUpdate = !(cursorAnimDt >= scrollDuration);
                    float t = Mathf.Min(1f, cursorAnimDt / scrollDuration);
                    cursor.sizeDelta = Vector2.Lerp(cursorSize, cursorTarget.sizeDelta, t);
                    cursor.position = Vector3.Lerp(cursorPosition, cursorTarget.position, t);
                }
                else
                {
                    cursor.sizeDelta = cursorTarget.sizeDelta;
                    cursor.position = cursorTarget.position;
                }
            }
        }

        public virtual void AddTab(UITabItem tabItem, UITabContent tabContent)
        {
            tabItem.tabContent = tabContent;
            tabItem.Selected = false;
            tabItem.onClick += item => FocusTab(item);
            if (tabContent)
            {
                tabItem.Selected = false;
                tabContent.Show(false);
            }
            _tabItems.Add(tabItem);
        }

        public virtual void UpdateListTab()
        {
            _tabItems?.Clear();
            TabItems.ForEach(tabItem =>
            {
                tabItem.Selected = false;
                tabItem.onClick += i => FocusTab(i);
                if (tabItem.tabContent)
                {
                    tabItem.Selected = tabItem.tabContent == CurrTabContent;
                    if (!tabItem.Selected) tabItem.tabContent.Show(false);
                }
            });
        }

        public UITabItem TabItem(string name) => TabItems.Find(_ => _.Name == name);

        public UITabItem TabItem(int index) => TabItems[index];

        public void FocusTab(int index)
        {
            if (index >= 0 && index < TabItems.Count)
                FocusTab(TabItems[index]);
        }

        public void FocusTab(string name)
        {
            FocusTab(TabItem(name));
        }

        protected virtual void FocusTab(UITabItem tabItem)
        {
            SetFocusTab(tabItem);
        }

        protected virtual void SetFocusTab(UITabItem tabItem)
        {
            if (!tabItem) throw new NullReferenceException("TabItem is null");
            if (CurrTabItem == tabItem) return;
            if (CurrTabItem) CurrTabItem.Selected = false;
            if (CurrTabContent) CurrTabContent.Show(false);

            CurrTabIndex = TabItems.IndexOf(tabItem);
            CurrTabItem = tabItem;
            CurrTabItem.Selected = true;
            CurrTabContent = tabItem.tabContent;
            if (CurrTabContent) CurrTabContent.Show(true);
            onTabSelected(CurrTabItem);

            PlayCursorAnim(CurrTabItem.RectTransform);
        }

        protected void PlayCursorAnim(RectTransform target)
        {
            if (!cursor) return;

            cursorTarget = target;
            cursorAnimDt = 0;
            cursorTargetUpdate = true;
            cursorSize = cursor.sizeDelta;
            cursorPosition = cursor.position;
        }
    }
}