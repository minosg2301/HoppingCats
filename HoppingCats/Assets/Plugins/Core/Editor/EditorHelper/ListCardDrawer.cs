using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest
{
    public class ListCardDrawer<T>
    {
        public int LineGap { get; set; } = 0;
        public int Padding { get; set; } = 4;
        public int MaxWidth { get; set; } = -1;
        public int LabelWidth { get; set; } = 60;
        public int CardWidth { get; set; } = 220;
        public int CardHeight { get; set; } = 90;
        public bool EditBeforeAdd { get; set; } = false;
        public bool DrawAddButton { get; set; } = true;
        public bool DisableAddButton { get; set; } = false;
        public bool DrawingWillAddedElement { get; private set; } = false;
        public bool DrawEditElement { get; set; } = true;
        public bool DrawDeleteElement { get; set; } = true;
        public T WillAddedElement { get; private set; } = default;

        public Action<T> onElementAdded;
        public Action<T> onElementRemoved;
        public Action<T> onDrawElement;
        public Func<T, bool> onDrawEditElement;
        public Func<T> elementCreator;

        protected virtual void DoDrawElement(T element) { }
        protected virtual bool DoDrawEditElement(T element) => false;
        protected virtual T CreateNewElement() { return default; }

        public void DoDraw(List<T> list)
        {
            float labelWidth = Draw.kLabelWidth;
            Draw.kLabelWidth = LabelWidth;

            float maxW = MaxWidth == -1 ? Screen.width : MaxWidth;
            maxW = Math.Max(maxW, CardWidth + Padding + LineGap);
            float x = 0;// EditorGUILayout.GetControlRect().x;

            var _list = list.ToList();
            T deletedElement = default;
            bool _drawAddButton = false;
            do
            {
                float totalW = x;
                Draw.BeginHorizontal();
                {
                    while(_list.Count > 0)
                    {
                        var ele = _list.Shift();
                        if(totalW + CardWidth + Padding + LineGap <= maxW)
                        {
                            totalW += CardWidth + Padding;
                            Draw.BeginVertical(Draw.BoxStyle, GUILayout.MaxWidth(CardWidth), GUILayout.MaxHeight(CardHeight));
                            {
                                if(onDrawElement == null) DoDrawElement(ele);
                                else onDrawElement?.Invoke(ele);

                                Draw.Space();

                                if (DrawDeleteElement)
                                {
                                    Draw.BeginHorizontal();
                                    {
                                        Draw.FlexibleSpace();

                                        if (Draw.Button("X", Color.gray, 20, 20))
                                        {
                                            deletedElement = ele;
                                        }
                                    }

                                    Draw.EndHorizontal();
                                }
                            }
                            Draw.EndVertical();

                            if(Padding > 0) Draw.Space(Padding);
                        }
                        else
                        {
                            _list.Unshift(ele);
                            break;
                        }
                    }

                    if(totalW + CardWidth + Padding <= maxW && _list.Count == 0)
                    {
                        _drawAddButton = true;
                        DoDrawAddButton(list);
                    }
                }
                Draw.EndHorizontal();

                if(Padding > 0) Draw.Space(Padding);

            } while(_list.Count > 0);

            if(!_drawAddButton)
            {
                DoDrawAddButton(list);
            }

            if(deletedElement != null)
            {
                list.Remove(deletedElement);
                onElementRemoved?.Invoke(deletedElement);
            }

            Draw.kLabelWidth = labelWidth;
        }

        public void CreateWillAddElement()
        {
            WillAddedElement = elementCreator.Invoke();
        }

        private void DoDrawAddButton(List<T> list)
        {
            if(EditBeforeAdd)
            {
                if(!DrawEditElement) return;

                if(WillAddedElement != null)
                {
                    Draw.BeginVertical(GUILayout.MaxWidth(CardWidth), GUILayout.MaxHeight(CardHeight));
                    {
                        DrawingWillAddedElement = true;

                        bool validated = onDrawEditElement == null ? DoDrawEditElement(WillAddedElement) : onDrawEditElement.Invoke(WillAddedElement);

                        Draw.FlexibleSpace();

                        Draw.BeginHorizontal();
                        {
                            Draw.BeginDisabledGroup(!validated);
                            if(Draw.Button("+", Color.green))
                            {
                                list.Add(WillAddedElement);
                                onElementAdded?.Invoke(WillAddedElement);
                                WillAddedElement = default;
                            }
                            Draw.EndDisabledGroup();
                            if(Draw.Button("X", Color.gray))
                            {
                                WillAddedElement = default;
                            }
                        }
                        Draw.EndHorizontal();

                        DrawingWillAddedElement = false;
                    }
                    Draw.EndVertical();
                }
                else
                {
                    if(!DrawAddButton) return;

                    if(Draw.Button("+", CardHeight, CardHeight))
                    {
                        WillAddedElement = elementCreator.Invoke();
                    }
                }
            }
            else
            {
                if(!DrawAddButton) return;

                if(Draw.Button("+", CardHeight, CardHeight))
                {
                    T addedElement;
                    if(elementCreator == null) addedElement = CreateNewElement();
                    else addedElement = elementCreator.Invoke();

                    list.Add(addedElement);
                    onElementAdded?.Invoke(addedElement);
                }
            }
        }
    }
}