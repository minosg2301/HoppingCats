using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace moonNest
{
    public class TableDrawer<T>
    {
        private static readonly List<int> pageElements = new List<int> { 5, 10, 20, 30, 50 };
        private static readonly float groupW = 48;
        private static readonly float buttonW = 22;

        private readonly Dictionary<ColDrawer<T>, string> exCols = new Dictionary<ColDrawer<T>, string>();
        private readonly Dictionary<ColDrawer<T>, Func<T, string>> exLabelGetters = new Dictionary<ColDrawer<T>, Func<T, string>>();
        private readonly List<ColDrawer<T>> columnDrawers = new List<ColDrawer<T>>();

        List<ColDrawer<T>> enabledColumnDrawers;

        private bool _init = false;
        private float tableWidth;

        private int deletedIndex;
        private T deletedElement;
        private T editingElement = default;
        private ColDrawer<T> editingColDrawer;
        private int editingIndex;

        public Action<T> onElementAdded;
        public Action<T> onElementDeleted;
        public Action<T, T> onSwapPerformed, onMovePerformed;
        public Action<T, T> onOrderChanged;
        public Func<T> elementCreator;
        public Func<T, string> onGetName;
        public Func<T, string> askBeforeDelete;
        public Func<T, bool> checkDeletable;
        public float rowHeight = -1;
        public bool drawHeader = true;
        public bool expandRight = true;
        public bool drawIndex = true;
        public bool drawControl = true;
        public bool drawOrder = true;
        public bool drawDeleteButton = true;
        public bool drawFilter = false;
        public bool drawAddButton = true;
        public bool inlineAdd = false;
        public bool alwayHidePage = false;
        public bool pageEnabled = true;
        public int pageElement = 10;
        public int minElementCount = 0;
        public bool allowSwap = true;
        public bool drawingInlineAdd = false;
        public bool disableInlineAdd = false;
        public bool disableInlineAddButton = false;

        private int upIdx, downIdx;
        private int pageIdx;
        private bool _internalPageEnabled = false;
        private bool showControl = false;
        private BaseSwapWindow<T> swapElementWindow;
        private bool willAddButtonClick;
        private ScopeConfig tableConfig;
        private List<AbstractFilterCell<T>> filters;

        public string SearchText { get; private set; } = "";
        public int Pages { get; private set; }
        public List<T> FullList { get; private set; }
        public List<T> FilteredList { get; private set; }
        public T WillAddedElement { get; private set; }

        public ColDrawer<T> AddCol(ColDrawer<T> colDrawer) { columnDrawers.Add(colDrawer); return colDrawer; }

        public ColDrawer<T> AddCol(string name, float maxWidth, Action<T> draw, Predicate<T> drawCondition, bool editable = true)
        {
            ColDrawer<T> colDrawer = new ColDrawer<T>(name, maxWidth, draw, drawCondition, editable);
            columnDrawers.Add(colDrawer);
            return colDrawer;
        }

        public ColDrawer<T> AddCol(string name, float maxWidth, Action<T> draw, Predicate<T> drawCondition)
        {
            ColDrawer<T> colDrawer = new ColDrawer<T>(name, maxWidth, draw, drawCondition);
            columnDrawers.Add(colDrawer);
            return colDrawer;
        }

        public ColDrawer<T> AddCol(string name, float maxWidth, Action<T> draw, bool editable = true)
        {
            ColDrawer<T> colDrawer = new ColDrawer<T>(name, maxWidth, draw, editable);
            columnDrawers.Add(colDrawer);
            return colDrawer;
        }

        public ColDrawer<T> AddCol(string name, float maxWidth, Func<T, T> draw, bool editable = true)
        {
            ColDrawer<T> colDrawer = new ColDrawer<T>(name, maxWidth, draw, editable);
            columnDrawers.Add(colDrawer);
            return colDrawer;
        }

        public ColDrawer<T> AddCol(string name, float maxWidth, Func<T, int, T> draw, bool editable = true)
        {
            ColDrawer<T> colDrawer = new ColDrawer<T>(name, maxWidth, draw, editable);
            columnDrawers.Add(colDrawer);
            return colDrawer;
        }

        public ColDrawer<T> AddCol(string name, float maxWidth, Action<T, int> draw, bool editable = true)
        {
            ColDrawer<T> colDrawer = new ColDrawer<T>(name, maxWidth, draw, editable);
            columnDrawers.Add(colDrawer);
            return colDrawer;
        }

        public void AddExpandCol(ExpandableColDrawer<T> exColDrawer)
        {
            if (exColDrawer.labelGetter != null) exLabelGetters[exColDrawer] = exColDrawer.labelGetter;
            else exCols[exColDrawer] = exColDrawer.label;

            columnDrawers.Add(exColDrawer);
        }

        public void AddExpandCol(string name, string label, float maxWidth, Action<T> draw, bool editable = true)
        {
            ColDrawer<T> colDrawer = new ColDrawer<T>(name, maxWidth, draw, editable);
            columnDrawers.Add(colDrawer);
            exCols[colDrawer] = label;
        }

        public void AddExpandCol(string name, string label, float maxWidth, Action<T> draw, Predicate<T> drawCondition, bool editable = true)
        {
            ColDrawer<T> colDrawer = new ColDrawer<T>(name, maxWidth, draw, drawCondition, editable);
            columnDrawers.Add(colDrawer);
            exCols[colDrawer] = label;
        }

        public void AddExpandCol(string name, Func<T, string> labelGetter, float maxWidth, Action<T> draw, bool editable = true)
        {
            ColDrawer<T> colDrawer = new ColDrawer<T>(name, maxWidth, draw, editable);
            columnDrawers.Add(colDrawer);
            exLabelGetters[colDrawer] = labelGetter;
        }

        public TableDrawer()
        {
            columnDrawers.Add(CreateIndexColDrawer());
        }

        public TableDrawer(string name) : this()
        {
            tableConfig = EditorConfigAsset.Get(name);
            pageElement = tableConfig.Get("Page", 10);
        }

        private void Init()
        {
            _init = true;

            if (!drawIndex) columnDrawers.RemoveAt(0);
            if (drawOrder) columnDrawers.Add(CreateOrderColDrawer());
            if (drawDeleteButton) columnDrawers.Add(CreateDeleteColDrawer());
            if (allowSwap)
            {
                swapElementWindow = new BaseSwapWindow<T>();
                swapElementWindow.onSwapClicked = OnSwapPerformed;
                swapElementWindow.onMoveClicked = OnMovePerformed;
                swapElementWindow.onGetName = ele => onGetName?.Invoke(ele);
            }
            if (drawFilter)
            {
                filters = columnDrawers.FindAll(col => col.filter != null).Map(col => col.filter);
            }
        }

        private ColDrawer<T> CreateIndexColDrawer()
        {
            float w = 25;
            return new ColDrawer<T>("Idx", w, (ele, i) =>
            {
                if (i < FullList.Count) Draw.Label((i + 1).ToString(), w);
                else Draw.Label(" ", w);
            });
        }

        private ColDrawer<T> CreateDeleteColDrawer()
        {
            return new ColDrawer<T>("", 20, (ele, i) =>
            {
                // draw delete button
                if (i < FullList.Count)
                {
                    Draw.BeginDisabledGroup(checkDeletable != null ? !checkDeletable.Invoke(ele) : false);
                    if (Draw.Button("X", Color.gray, 20))
                    {
                        if (askBeforeDelete != null)
                        {
                            if (EditorUtility.DisplayDialog("Delete Element", askBeforeDelete?.Invoke(ele), "Delete", "Cancel"))
                            {
                                deletedElement = ele;
                                deletedIndex = i;
                            }
                        }
                        else
                        {
                            deletedElement = ele;
                            deletedIndex = i;
                        }
                    }
                    Draw.EndDisabledGroup();
                }

                // draw add button for inline add
                else
                {
                    Draw.BeginDisabledGroup(disableInlineAddButton);
                    if (Draw.Button("+", Color.yellow, 20))
                    {
                        willAddButtonClick = true;
                    }
                    Draw.EndDisabledGroup();
                }
            });
        }

        private ColDrawer<T> CreateOrderColDrawer()
        {
            return new ColDrawer<T>("", groupW, (ele, i) =>
            {
                GUILayout.BeginHorizontal(GUILayout.MaxWidth(groupW));
                if (i < FullList.Count && FullList.Count > 1)
                {
                    if (i > 0 && Draw.Button("▲", i == FullList.Count - 1 ? groupW : buttonW)) upIdx = i;
                    if (i < FullList.Count - 1 && Draw.Button("▼", i == 0 ? groupW : buttonW)) downIdx = i;
                }
                else// if(inlineAdd)
                {
                    Draw.Label("", groupW);
                }
                GUILayout.EndHorizontal();
            });
        }

        public virtual void DoDraw(List<T> list)
        {
            if (!_init) Init();

            deletedIndex = -1;
            deletedElement = default;

            if (FullList == null || FullList.Count != list.Count || FullList != list)
            {
                FullList = list;
                FilteredList = FullList;
            }

            upIdx = downIdx = -1;

            // split pages
            if (pageEnabled) _internalPageEnabled = FilteredList.Count > 10;
            else _internalPageEnabled = pageEnabled;

            if (_internalPageEnabled)
                Pages = (FilteredList.Count / pageElement) + (FilteredList.Count % pageElement > 0 ? 1 : 0);

            Draw.Space(6);
            DrawSplitPages();

            enabledColumnDrawers = columnDrawers.FindAll(c => c.enabled);
            tableWidth = enabledColumnDrawers.Sum(col => col.maxWidth);

            Draw.BeginHorizontal();
            {
                // draw table
                Draw.BeginVertical(tableWidth);
                {
                    DrawHeader();

                    DrawFilters();

                    int fromIdx = pageIdx * pageElement;
                    List<T> tempList = _internalPageEnabled
                        ? FilteredList.GetRange(fromIdx, Math.Min(FilteredList.Count - fromIdx, pageElement))
                        : FilteredList;

                    // draw elements
                    if (typeof(T) == typeof(string) || typeof(T) == typeof(int) || typeof(T) == typeof(short))
                    {
                        DrawPrimaryList(tempList);
                        if (_internalPageEnabled)
                        {
                            for (int i = 0; i < tempList.Count; i++)
                                FilteredList[fromIdx + i] = tempList[i];
                        }
                    }
                    else
                    {
                        DrawObjectList(tempList);
                    }

                    DrawInlineAdd();

                    UpdateElementOrder();
                    UpdateElementDeleted();

                    DrawFooter();

                    if (editingElement != null && !expandRight && editingColDrawer != null)
                    {
                        Draw.Space(8);
                        Draw.LabelBold(editingColDrawer.name + " Edit");
                        editingColDrawer?.OnDraw(editingElement, editingIndex);
                    }

                    UpdateElementInlineAdded();
                }
                Draw.EndVertical();

                if (editingElement != null && expandRight && editingColDrawer != null)
                {
                    Draw.Space(16);
                    Draw.FitLabel(">>");
                    Draw.BeginVertical();
                    Draw.LabelBold("Edit " + editingColDrawer.name);
                    editingColDrawer?.OnDraw(editingElement, editingIndex);
                    Draw.EndVertical();
                }

            }
            Draw.EndHorizontal();
        }

        private void DrawFilters()
        {
            if (drawFilter)
            {
                Draw.BeginChangeCheck();
                Draw.BeginHorizontal();
                enabledColumnDrawers.ForEach(col =>
                {
                    if (col.filter != null) col.filter.Draw(col.maxWidth);
                    else Draw.LabelBold(" ", col.maxWidth);
                });
                Draw.EndHorizontal();
                if (Draw.EndChangeCheck())
                {
                    var prevFilteredList = FilteredList;
                    FilteredList = FullList;
                    filters.ForEach(filter => FilteredList = FilteredList.FindAll(e => filter.Match(e)));
                    pageIdx = prevFilteredList.Count != FilteredList.Count ? 0 : pageIdx;
                }
                Draw.SeparateHLine();
            }
        }

        private void DrawHeader()
        {
            if (drawHeader)
            {
                Draw.BeginHorizontal();
                enabledColumnDrawers.ForEach(col => Draw.LabelBold(col.name, col.maxWidth));
                Draw.EndHorizontal();
            }
        }

        private void DrawInlineAdd()
        {
            // inline element add
            if (inlineAdd && elementCreator != null)
            {
                if (WillAddedElement == null)
                    WillAddedElement = elementCreator.Invoke();

                Draw.Space(16);
                //Draw.SeparateHLine();
                Draw.BeginDisabledGroup(disableInlineAdd);
                {
                    drawingInlineAdd = true;
                    Draw.BeginHorizontal();
                    if (typeof(T) == typeof(string) || typeof(T) == typeof(int) || typeof(T) == typeof(short))
                        enabledColumnDrawers.ForEach(col => WillAddedElement = col.OnDraw(WillAddedElement, FullList.Count, true));
                    else
                        enabledColumnDrawers.ForEach(col => col.OnDraw(WillAddedElement, FullList.Count, true));
                    Draw.EndHorizontal();
                    drawingInlineAdd = false;
                }
                Draw.EndDisabledGroup();
            }
        }

        private void UpdateElementDeleted()
        {
            if (deletedIndex != -1)
            {
                FullList.RemoveAt(deletedIndex);
                onElementDeleted?.Invoke(deletedElement);
                if (_internalPageEnabled)
                {
                    int newPageIdx = (FullList.Count - 1) / pageElement;
                    if (newPageIdx < pageIdx)
                        pageIdx = newPageIdx;
                }
            }
        }

        private void UpdateElementOrder()
        {
            if (upIdx != -1)
            {
                T a, b;
                T temp = a = FullList[upIdx];
                FullList[upIdx] = b = FullList[upIdx - 1];
                FullList[upIdx - 1] = temp;
                onOrderChanged?.Invoke(a, b);
            }

            if (downIdx != -1)
            {
                T a, b;
                T temp = a = FullList[downIdx];
                FullList[downIdx] = b = FullList[downIdx + 1];
                FullList[downIdx + 1] = temp;
                onOrderChanged?.Invoke(a, b);
            }
        }

        private void UpdateElementInlineAdded()
        {
            if (willAddButtonClick)
            {
                FullList.Add(WillAddedElement);
                onElementAdded?.Invoke(WillAddedElement);
                WillAddedElement = elementCreator.Invoke();
                willAddButtonClick = false;
                Draw.ResetInputFocus();

                if (_internalPageEnabled)
                {
                    pageIdx = (FullList.Count - 1) / pageElement;
                }
            }
        }

        public bool DoDrawWindow()
        {
            return swapElementWindow.DoDraw();
        }

        private void DrawPrimaryList(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Draw.BeginHorizontal();
                enabledColumnDrawers.ForEach(col => list[i] = col.OnDraw(list[i], i));
                Draw.EndHorizontal();
            }
        }

        private void DrawObjectList(List<T> list)
        {
            T newEle = default;
            int changedIndex = -1;
            list.ForEach((ele, i) =>
            {
                bool isEditing = editingElement != null && editingElement.Equals(ele);
                Draw.BeginHorizontal(rowHeight);
                enabledColumnDrawers.ForEach(col =>
                {
                    if (exCols.ContainsKey(col) || exLabelGetters.ContainsKey(col))
                    {
                        if (col.drawCondition == null || col.drawCondition.Invoke(ele))
                        {
                            var label = exCols.ContainsKey(col) ? exCols[col] : exLabelGetters[col].Invoke(ele);
                            if (Draw.Button(label,
                                isEditing ? Color.yellow : GUI.backgroundColor,
                                isEditing ? Color.white : GUI.skin.button.normal.textColor,
                                col.maxWidth))
                            {
                                editingElement = isEditing ? default : ele;
                                editingColDrawer = editingElement != null ? col : null;
                                editingIndex = i;
                            }
                        }
                        else
                        {
                            Draw.Label("", col.maxWidth);
                        }
                    }
                    else
                    {
                        int fromIdx = pageIdx * pageElement;
                        T _ele = col.OnDraw(ele, i + fromIdx);
                        if (_ele != null && !_ele.Equals(ele))
                        {
                            changedIndex = i;
                            newEle = _ele;
                        }
                    }

                });
                Draw.EndHorizontal();
            });
            if (changedIndex != -1)
            {
                list.Replace(newEle, changedIndex);
            }
        }

        private void DrawSplitPages()
        {
            if (_internalPageEnabled && !alwayHidePage)
            {
                Draw.BeginHorizontal();
                Draw.BeginChangeCheck();
                Draw.LabelBold("Show", 35);
                pageElement = Draw.IntPopup(pageElement, pageElements, 60);
                if (Draw.EndChangeCheck())
                {
                    pageIdx = 0;
                    tableConfig?.Set("Page", pageElement);
                }

                if (pageElement < FilteredList.Count)
                {
                    Draw.Space(16);
                    Draw.LabelBold("Pages", 40);
                    for (int i = 0; i < Pages; i++)
                    {
                        if (pageIdx == i) GUI.backgroundColor = Color.green;
                        if (Draw.FitButton((i + 1).ToString(), 14)) pageIdx = i;
                        GUI.backgroundColor = Color.white;
                    }
                    Draw.FlexibleSpace();
                }
                Draw.EndHorizontal();
            }
        }

        private void DrawFooter()
        {
            DrawAddButton();
            DrawControl();
        }

        private void DrawControl()
        {
            if (drawControl)
            {
                Draw.Space();
                showControl = Draw.FitToggleGroup(showControl, "Controls");
                if (showControl)
                {
                    Draw.BeginVertical();
                    {
                        Draw.Space(6);
                        Draw.BeginHorizontal();
                        {
                            if (Draw.FitToolbarButton("Move", Color.cyan, Color.white))
                            {
                                swapElementWindow.Move = true;
                                swapElementWindow.SetUp(FullList);
                                swapElementWindow.Show();
                            }

                            Draw.Space(6);
                            if (Draw.FitToolbarButton("Swap", Color.blue, Color.white))
                            {
                                swapElementWindow.Move = false;
                                swapElementWindow.SetUp(FullList);
                                swapElementWindow.Show();
                            }
                        }
                        Draw.EndHorizontal();
                    }
                    Draw.EndVertical();
                }
            }
        }

        private void DrawAddButton()
        {
            if (!inlineAdd && elementCreator != null && drawAddButton)
            {
                Draw.Space(6);
                if (Draw.FitButton("+", Color.yellow))
                {
                    T newElement = elementCreator.Invoke();
                    if (newElement != null)
                    {
                        FullList.Add(newElement);
                        onElementAdded?.Invoke(newElement);
                        Draw.ResetInputFocus();
                    }

                    if (_internalPageEnabled)
                    {
                        pageIdx = (FullList.Count - 1) / pageElement;
                    }
                }
            }
        }

        private void OnMovePerformed(T a, T b)
        {
            swapElementWindow.Hide();

            FullList.Remove(b);
            FullList.Insert(FullList.IndexOf(a), b);

            onMovePerformed?.Invoke(a, b);
            onOrderChanged?.Invoke(a, b);
        }

        private void OnSwapPerformed(T a, T b)
        {
            //swapElementWindow.Hide();

            int indexA = FullList.IndexOf(a);
            int indexB = FullList.IndexOf(b);
            FullList[indexA] = b;
            FullList[indexB] = a;

            onSwapPerformed?.Invoke(a, b);
            onOrderChanged?.Invoke(a, b);
        }
    }
}