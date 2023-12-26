using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public abstract class ListTabDrawer<T> where T : ICloneable
    {
        private static readonly List<int> pageElements = new List<int> { 5, 10, 20, 30, 50 };

        public float TabHeight { get; set; } = 32;
        public float TabWidth { get; set; } = 100;
        public float ContentPadding { get; private set; } = 6;
        public int JumpStep { get; set; } = 10;
        public bool DrawControl { get; set; } = true;
        public int MaxVisibleHeader { get; set; } = 10;
        public bool DrawSeparator { get; set; } = false;
        public bool DrawAddButton { get; set; } = true;
        public float DrawSeparatorLength { get; set; } = 320f;
        public HeaderType HeaderType { get; set; } = HeaderType.Horizontal;
        public int SelectedIndex { get; private set; }
        public T SelectedElement { get; private set; }
        public static bool BoxStyle { get; private set; } = true;
        public bool HideOnSwap { get; set; } = true;
        public bool KeepCurrentTabIndex { get; set; } = false;
        public IReadOnlyList<T> List => fullList.AsReadOnly();

        public bool Paging { get; set; } = false;
        public List<T> PagingList { get; private set; }
        public int Pages { get; private set; }
        private int pageIdx = 0;
        private int pageElement = 10;

        public delegate void CloneEvent(T newElement, T origin);
        public CloneEvent onElementCloned = delegate { };

        public Action<T> onElementAdded = delegate { };
        public Action<T> onElementRemoved = delegate { };
        public Action<T, T> onSwapPerformed = delegate { };
        public Func<T, string> askBeforeDelete;

        private readonly BaseSwapWindow<T> swapLevelWindow;
        private readonly ScopeConfig tableConfig;
        private List<T> fullList;
        private Vector2 scrollPos;
        private bool showControl = false;


        public ListTabDrawer()
        {
            swapLevelWindow = new BaseSwapWindow<T>();
            swapLevelWindow.onSwapClicked = OnSwapPerformed;
            swapLevelWindow.onGetName = ele => GetTabLabel(ele);
        }

        public ListTabDrawer(string name) : this()
        {
            tableConfig = EditorConfigAsset.Get(name);
            HeaderType = (HeaderType)tableConfig.Get("header-type", (int)HeaderType.Horizontal);
            Paging = tableConfig.Get("Paging", 0) == 1;
            pageElement = tableConfig.Get("Page", 10);
        }

        protected abstract void DoDrawContent(T element);
        protected abstract string GetTabLabel(T element);
        protected abstract T CreateNewElement();
        protected virtual int GetLabelFontSize(T element) => 12;
        protected virtual void OnTabChanged(T element, int index) { }

        public virtual void DoDraw(params T[] array) => DoDraw(array.ToList());

        public virtual void DoDraw(List<T> list)
        {
            fullList = list;

            if (Paging && PagingList == null)
            {
                int fromIdx = pageIdx * pageElement;
                PagingList = fullList.GetRange(fromIdx, Math.Min(fullList.Count - fromIdx, pageElement));
            }

            if (!list.Contains(SelectedElement))
            {
                if (KeepCurrentTabIndex)
                    SelectedElement = list.Count > SelectedIndex ? list[SelectedIndex] : default;
                else if (list.Count > 0)
                {
                    SelectedIndex = 0;
                    SelectedElement = list[SelectedIndex];
                }
                else
                    SelectedElement = default;

                OnTabChanged(SelectedElement, SelectedIndex);
            }

            if (HeaderType == HeaderType.Vertical) Draw.BeginHorizontal(Draw.SubContentStyle);
            else Draw.BeginVertical(Draw.SubContentStyle);

            DrawHeader();
            Draw.BeginVertical();
            Draw.BeginVertical(Draw.BoxStyle);
            DrawContent();
            Draw.EndVertical();
            DrawFooter();
            Draw.EndVertical();

            if (HeaderType == HeaderType.Vertical) Draw.EndHorizontal();
            else Draw.EndVertical();
        }

        private void DrawHeader()
        {
            T lastSelectedElement = SelectedElement;

            if (Paging)
            {
                if (HeaderType == HeaderType.Horizontal)
                {
                    DrawSplitPages();
                    DrawHorizontalPaging();
                }
            }
            else
            {
                if (HeaderType == HeaderType.Horizontal) DrawHorizontalHeader();
                else DrawVerticalHeader();
            }

            if (!Equals(lastSelectedElement, SelectedElement))
            {
                GUIUtility.keyboardControl = 0; // Remove Input Focus
            }
        }

        private void DrawSplitPages()
        {
            Pages = (fullList.Count / pageElement) + (fullList.Count % pageElement > 0 ? 1 : 0);

            Draw.BeginHorizontal();
            Draw.BeginChangeCheck();
            Draw.LabelBold("Show", 35);
            pageElement = Draw.IntPopup(pageElement, pageElements, 60);
            if (Draw.EndChangeCheck())
            {
                pageIdx = 0;
                int fromIdx = pageIdx * pageElement;
                PagingList = fullList.GetRange(fromIdx, Math.Min(fullList.Count - fromIdx, pageElement));
                tableConfig?.Set("Page", pageElement);
            }

            if (pageElement < fullList.Count)
            {
                Draw.Space(16);
                Draw.LabelBold("Pages", 40);
                for (int i = 0; i < Pages; i++)
                {
                    if (pageIdx == i) GUI.backgroundColor = Color.green;
                    if (Draw.FitButton((i + 1).ToString(), 14))
                    {
                        pageIdx = i;
                        int fromIdx = pageIdx * pageElement;
                        PagingList = fullList.GetRange(fromIdx, Math.Min(fullList.Count - fromIdx, pageElement));
                    }
                    GUI.backgroundColor = Color.white;
                }
                Draw.FlexibleSpace();
            }
            Draw.EndHorizontal();
        }

        private void DrawHorizontalPaging()
        {
            NewMethod(PagingList.ToList());
        }

        private void DrawHorizontalHeader()
        {
            NewMethod(fullList.ToList());
        }

        void NewMethod(List<T> _list)
        {
            do
            {
                float totalW = 0;
                float w;
                Draw.BeginHorizontal();
                {
                    while (_list.Count > 0)
                    {
                        T ele = _list.Shift();
                        string label = GetTabLabel(ele);
                        w = Math.Max(TabWidth, Draw.GetWidth(label.ToUpper()) + 6);
                        if (totalW + w <= Screen.width)
                        {
                            totalW += w;
                            if (Draw.ToggleButton(ele.Equals(SelectedElement), label.ToUpper(), w, TabHeight, GetLabelFontSize(ele)))
                            {
                                if (!SelectedElement.Equals(ele))
                                {
                                    SelectedElement = ele;
                                    SelectedIndex = fullList.IndexOf(SelectedElement);
                                    OnTabChanged(SelectedElement, SelectedIndex);
                                }
                            }
                        }
                        else
                        {
                            _list.Unshift(ele);
                            break;
                        }
                    }

                    if (_list.Count == 0 && DrawAddButton && Draw.ToolbarButton("+", Color.yellow, Color.black, TabHeight, TabHeight))
                    {
                        OnAddClicked();
                    }
                }
                Draw.EndHorizontal();
            }
            while (_list.Count > 0);
        }

        private void DrawVerticalHeader()
        {
            Draw.BeginVertical(TabWidth);
            scrollPos = Draw.BeginScrollView(scrollPos);
            {
                fullList.ForEach(ele =>
                {
                    if (Draw.ToggleButton(ele.Equals(SelectedElement), GetTabLabel(ele).ToUpper(), TabWidth, TabHeight, GetLabelFontSize(ele)))
                    {
                        SelectedElement = ele;
                        SelectedIndex = fullList.IndexOf(SelectedElement);
                        OnTabChanged(SelectedElement, SelectedIndex);
                    }
                });

                if (DrawAddButton && Draw.ToolbarButton("+", Color.yellow, Color.black, TabWidth, 24))
                {
                    OnAddClicked();
                }
            }
            Draw.EndScrollView();
            Draw.EndVertical();
        }

        private void DrawContent()
        {
            if (SelectedElement != null) DoDrawContent(SelectedElement);
        }

        private void DrawFooter()
        {
            if (!DrawControl) return;

            Draw.BeginVertical(Draw.DefaultStyle);
            showControl = Draw.FitToggleGroup(showControl, "Controls");
            if (showControl)
            {
                Draw.BeginVertical();
                {
                    Draw.Space(6);
                    Draw.BeginHorizontal();
                    {
                        if (Draw.FitToolbarButton("Delete", Color.red, Color.white)) OnDeleteClicked(SelectedElement);

                        Draw.Space(6);
                        if (Draw.FitToolbarButton("Clone", Color.cyan, Color.white)) OnCloneClicked(SelectedElement);

                        Draw.Space(6);
                        if (Draw.FitToolbarButton("Swap", Color.blue, Color.white))
                        {
                            swapLevelWindow.SetUp(fullList);
                            swapLevelWindow.Show();
                        }
                    }
                    Draw.EndHorizontal();

                    Draw.BeginChangeCheck();
                    Paging = Draw.ToggleField(Paging, "Paging");
                    if (Draw.EndChangeCheck())
                    {
                        int fromIdx = pageIdx * pageElement;
                        PagingList = fullList.GetRange(fromIdx, Math.Min(fullList.Count - fromIdx, pageElement));
                        tableConfig?.Set("Paging", Paging ? 1 : 0);
                    }
                }
                Draw.EndVertical();
            }
            Draw.EndVertical();
        }

        public virtual bool DoDrawWindow() => swapLevelWindow.DoDraw();

        private void OnAddClicked()
        {
            T newElement = CreateNewElement();
            fullList.Add(newElement);
            onElementAdded(newElement);
            SelectedElement = newElement;
            OnTabChanged(SelectedElement, SelectedIndex);
        }

        private void OnCloneClicked(T element)
        {
            T newElement = (T)element.Clone();
            fullList.Add(newElement);
            onElementCloned(newElement, element);
            onElementAdded(newElement);
            SelectedElement = newElement;
            OnTabChanged(SelectedElement, SelectedIndex);
        }

        private void OnDeleteClicked(T element)
        {
            if (askBeforeDelete != null)
            {
                if (EditorUtility.DisplayDialog("Delete Element", askBeforeDelete?.Invoke(element), "Delete", "Cancel"))
                {
                    DoDeleteElement(element);
                }
            }
            else
            {
                DoDeleteElement(element);
            }
        }

        private void DoDeleteElement(T element)
        {
            int index = fullList.IndexOf(element);
            fullList.Remove(element);
            SelectedElement = fullList.Count > 0 ? fullList[Math.Max(0, --index)] : default;
            onElementRemoved(element);
            OnTabChanged(SelectedElement, SelectedIndex);
        }

        private void OnSwapPerformed(T a, T b)
        {
            if (HideOnSwap)
            {
                swapLevelWindow.Hide();
            }

            int indexA = fullList.IndexOf(a);
            int indexB = fullList.IndexOf(b);
            fullList[indexA] = b;
            fullList[indexB] = a;

            onSwapPerformed(a, b);
        }
    }
}
