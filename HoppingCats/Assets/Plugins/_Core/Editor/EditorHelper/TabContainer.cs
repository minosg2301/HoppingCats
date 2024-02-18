using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest
{
    public class TabContainer
    {
        public List<TabItem> TabItems => items.ToList();

        public int TabHeight { get; set; } = 32;
        public int TabWidth { get; set; } = 100;
        public int JumpStep { get; set; } = 10;
        public int FirstIndexFocused { get; set; } = 0;
        public bool DrawShortHeader { get; set; } = true;
        public float DrawSeparatorLength { get; set; } = 320f;
        public bool DrawContentOnly { get; set; } = false;
        public HeaderType HeaderType { get; set; } = HeaderType.Horizontal;
        public TabItem SelectedItem { get; private set; }

        public Action OnButtonAdd;

        int fromShownIdx = 0;

        readonly List<TabItem> items = new List<TabItem>();

        public void InsertTab(TabItem item, int index) => items.Insert(index, item);

        public void AddTab(TabItem item) => items.Add(item);

        public void AddTab(string label, TabContent tab) => items.Add(new TabItem(label, tab));

        public TabItem DrawHeader()
        {
            TabItem lastSelectedItem = SelectedItem;

            if(SelectedItem == null && items.Count > 0) SelectedItem = items[FirstIndexFocused];

            if(HeaderType == HeaderType.Horizontal)
            {
                if(DrawShortHeader) DrawHorizontalHeaderShort();
                else DrawHorizontalHeader();
            }
            else if(HeaderType == HeaderType.Vertical)
            {
                if(DrawShortHeader) DrawVerticalHeaderShort();
                else DrawVerticalHeader();
            }

            if(lastSelectedItem != SelectedItem)
            {
                SelectedItem.Content.OnFocused();
                GUIUtility.keyboardControl = 0; // Remove Input Focus
            }

            return SelectedItem;
        }

        public void DoDraw()
        {
            if(DrawContentOnly && items.Count > 0)
            {
                items[0].DoDrawContent();
                return;
            }

            if(HeaderType == HeaderType.Vertical) Draw.BeginHorizontal(Draw.SubContentStyle);
            else Draw.BeginVertical(Draw.SubContentStyle);

            var tabItem = DrawHeader();
            if(tabItem != null)
            {
                Draw.BeginVertical(Draw.BoxStyle);
                tabItem.DoDrawContent();
                Draw.Space(6);
                OnPostDrawContent(tabItem);
                Draw.EndVertical();
            }

            if(HeaderType == HeaderType.Vertical) Draw.EndHorizontal();
            else Draw.EndVertical();
        }

        public TabItem GetTab(string name) => items.Find(_ => _.label == name);

        public void DoDrawAsToolbar()
        {
            TabItem lastSelectedItem = SelectedItem;

            if(SelectedItem == null && items.Count > 0) SelectedItem = items[0];

            Draw.BeginHorizontal();

            items.ForEach(ele =>
            {
                float w = Math.Max(TabWidth, Draw.GetWidth(ele.label.ToUpper()) + 12);
                if(Draw.ToggleButton(ele == SelectedItem, ele.label.ToUpper(), w, TabHeight))
                    SelectedItem = ele;
            });

            Draw.EndHorizontal();

            if(lastSelectedItem != SelectedItem)
            {
                SelectedItem.Content.OnFocused();
                GUIUtility.keyboardControl = 0; // Remove Input Focus
            }
        }

        private void DrawHorizontalHeader()
        {
            List<TabItem> list = items.ToList();

            do
            {
                float totalW = 0;
                float w = 0;
                Draw.BeginHorizontal();
                while(list.Count > 0)
                {
                    TabItem ele = list.Shift();

                    if(!ele.show) continue;

                    w = Math.Max(TabWidth, Draw.GetWidth(ele.label) + 12);
                    if(totalW + w <= Screen.width)
                    {
                        Color color = GUI.backgroundColor;
                        GUI.backgroundColor = ele.HeaderBackgroundColor;
                        if (Draw.ToggleButton(ele == SelectedItem, ele.label.ToUpper(), w, TabHeight))
                            SelectedItem = ele;
                        GUI.backgroundColor = color;
                        totalW += w;
                    }
                    else
                    {
                        list.Unshift(ele);
                        break;
                    }
                }

                if(list.Count == 0 && OnButtonAdd != null)
                {
                    GUI.backgroundColor = Color.yellow;
                    if(Draw.ToolbarButton("+", TabHeight, TabHeight))
                        OnButtonAdd?.Invoke();
                    GUI.backgroundColor = Color.white;
                }
                Draw.EndHorizontal();
            }
            while(list.Count > 0);
        }

        private void DrawHorizontalHeaderShort()
        {
            List<TabItem> list = items.SubList(fromShownIdx);

            float totalW = 0;
            float w = 0;
            float jumpBtnW = Draw.GetWidth("<<");
            Draw.BeginHorizontal();
            if(fromShownIdx > 0)
            {
                if(Draw.FitToolbarButton("<<", Color.cyan, Color.white, TabHeight))
                {
                    fromShownIdx = Math.Max(0, fromShownIdx - JumpStep);
                    SelectedItem = items[fromShownIdx];
                    return;
                }
                totalW += jumpBtnW;
            }

            int drawCount = 0;
            while(list.Count > 0)
            {
                TabItem ele = list.Shift();

                if(!ele.show) continue;

                w = Math.Max(TabWidth, Draw.GetWidth(ele.label.ToUpper()) + 12);
                if(totalW + w + jumpBtnW <= Screen.width)
                {
                    Color color = GUI.backgroundColor;
                    GUI.backgroundColor = ele.HeaderBackgroundColor;
                    if (Draw.ToggleButton(ele == SelectedItem, ele.label.ToUpper(), w, TabHeight))
                    {
                        SelectedItem = ele;
                    }
                    GUI.backgroundColor = color;

                    totalW += w;
                    drawCount++;
                }
                else
                {
                    list.Unshift(ele);
                    break;
                }
            }

            if(list.Count > 0)
            {
                if(Draw.FitToolbarButton(">>", Color.cyan, Color.white, TabHeight))
                {
                    fromShownIdx = Mathf.Min(items.Count - drawCount, fromShownIdx + drawCount);
                    SelectedItem = items[fromShownIdx];
                }
            }

            if(OnButtonAdd != null)
            {
                GUI.backgroundColor = Color.yellow;
                if(Draw.ToolbarButton("+", TabHeight, TabHeight))
                {
                    OnButtonAdd?.Invoke();
                }
                GUI.backgroundColor = Color.white;
            }
            Draw.EndHorizontal();
        }

        private void DrawVerticalHeader()
        {
            Draw.BeginVertical(TabWidth + 6);
            items.ForEach(ele =>
            {
                if(!ele.show) return;

                Color color = GUI.backgroundColor;
                GUI.backgroundColor = ele.HeaderBackgroundColor;
                if(Draw.ToggleButton(ele == SelectedItem, ele.label.ToUpper(), TabWidth, TabHeight))
                    SelectedItem = ele;
                GUI.backgroundColor = color;
            });
            if(OnButtonAdd != null)
            {
                Color color = GUI.backgroundColor;
                GUI.backgroundColor = Color.yellow;
                if(Draw.ToolbarButton("+", TabWidth, 24))
                    OnButtonAdd?.Invoke();
                GUI.backgroundColor = color;
            }
            Draw.EndVertical();
        }

        private void DrawVerticalHeaderShort()
        {
            List<TabItem> list = items.SubList(fromShownIdx, JumpStep);

            Draw.BeginVertical(TabWidth + 6);
            if(fromShownIdx > 0)
            {
                if(Draw.ToolbarButton("<<", Color.cyan, Color.white, TabWidth, 24))
                {
                    fromShownIdx = Math.Max(0, fromShownIdx - JumpStep);
                    SelectedItem = items[fromShownIdx];
                    return;
                }
            }
            list.ForEach(ele =>
            {
                if(!ele.show) return;

                Color color = GUI.backgroundColor;
                GUI.backgroundColor = ele.HeaderBackgroundColor;
                if (Draw.ToggleButton(ele == SelectedItem, ele.label.ToUpper(), TabWidth, TabHeight))
                    SelectedItem = ele;
                GUI.backgroundColor = color;
            });
            if(fromShownIdx + JumpStep < items.Count)
            {
                if(Draw.ToolbarButton(">>", Color.cyan, Color.white, TabWidth, 24))
                {
                    fromShownIdx = Mathf.Min(items.Count - JumpStep, fromShownIdx + JumpStep);
                    SelectedItem = items[fromShownIdx];
                }
            }
            if(OnButtonAdd != null)
            {
                GUI.backgroundColor = Color.yellow;
                if(Draw.ToolbarButton("+", TabWidth, 24))
                    OnButtonAdd?.Invoke();
                GUI.backgroundColor = Color.white;
            }
            Draw.EndVertical();
        }

        public void ClearAll()
        {
            items.Clear();
        }

        public void SetSelectedTab(int index)
        {
            if(items.Count <= index) return;

            SelectedItem = items[index];
            fromShownIdx = Mathf.Clamp(index, 0, items.Count > JumpStep ? items.Count - JumpStep : 0);
        }

        public void SetSelectedTab(TabItem item)
        {
            SelectedItem = item;
            int selectedIndex = items.IndexOf(SelectedItem);
            fromShownIdx = Mathf.Clamp(selectedIndex, 0, items.Count > JumpStep ? items.Count - JumpStep : 0);
        }

        public void RemoveTab(string name)
        {
            TabItem tabItem = GetTab(name);
            if(tabItem != null) RemoveTab(tabItem.Content);
        }

        public void RemoveTab(TabContent tabContent)
        {
            TabItem tabItem = items.Find(_ => _.Content == tabContent);
            RemoveTab(tabItem);
        }

        public void RemoveTab(TabItem tabItem)
        {
            int index = items.IndexOf(tabItem);
            items.Remove(tabItem);
            SelectedItem = SelectedItem != tabItem
                ? SelectedItem : index < items.Count
                ? items[index] : items.Count > 0
                ? items[items.Count - 1] : null;
        }

        public virtual bool DoDrawWindow() => SelectedItem == null ? false : SelectedItem.DoDrawWindow();

        protected virtual void OnPostDrawContent(TabItem tabItem) { }
    }

    public class TabItem
    {
        public string label;
        public bool show = true;
        public bool overrideColor = false;

        public TabContent Content { get; private set; }

        public TabItem(string label, TabContent content)
        {
            this.label = label;
            Content = content;
            Content.TabItem = this;
        }

        public void DoDrawContent()
        {
            Content.DoDraw();
        }

        public Color HeaderBackgroundColor => Content.HeaderBackgroundColor;

        public bool DoDrawWindow() => Content.DoDrawWindow();
    }

    public abstract class TabContent
    {
        public TabItem TabItem { get; internal set; }
        public abstract void DoDraw();
        public virtual bool DoDrawWindow() => false;
        public virtual void OnFocused() { }
        public virtual Color HeaderBackgroundColor => Color.white;
    }

    public enum HeaderType
    {
        Horizontal,
        Vertical
    }
}