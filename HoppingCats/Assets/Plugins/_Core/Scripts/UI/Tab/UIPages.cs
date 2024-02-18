using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace moonNest
{
    public class UIPages : UITab, IDragHandler, IEndDragHandler, IBeginDragHandler, IInitializePotentialDragHandler
    {
        public float dragThreshold = 0.2f;
        public float dragDamp = 0.5f;
        public bool dragEnabled = true;
        public bool playAnimation = true;

        ForwardParentDrag forwardParentDrag;

        private Vector2 lastPosition;
        private Vector2 rootPosition;
        private float pageSize;
        private bool isOutBound;
        private bool smoothMoving;
        private bool skipDrag;
        private List<UITabContent> visibleTabContents = new List<UITabContent>();

        public Action<UITabItem> onPageChanged = delegate { };
        public Action<UITabItem> onPageWillChange = delegate { };

        int visiblePage;
        public int VisiblePage
        {
            get { return visiblePage; }
            protected set { visiblePage = Math.Clamp(value, 0, TabItems.Count); }
        }

        #region override methods
        protected override void Start()
        {
            base.Start();

            CurrTabContent.RectTransform.anchoredPosition = Vector2.zero;
            CurrTabContent.Show(true);
            PlayCursorAnim(CurrTabItem.RectTransform);

            forwardParentDrag = new ForwardParentDrag(transform);

            visiblePage = TabItems.Count;
        }

        public override void AddTab(UITabItem tabItem, UITabContent tabContent)
        {
            base.AddTab(tabItem, tabContent);
            visiblePage = TabItems.Count;
        }

        public override void UpdateListTab()
        {
            base.UpdateListTab();

            if (CurrTabContent == null) return;

            CurrTabContent.RectTransform.anchoredPosition = Vector2.zero;
            CurrTabContent.Show(true);
            PlayCursorAnim(CurrTabItem.RectTransform);

            forwardParentDrag = new ForwardParentDrag(transform);
        }

        protected override void FocusTab(UITabItem tabItem)
        {
            if (smoothMoving) return;

            int selectedIndex = TabItems.IndexOf(tabItem);
            if (CurrTabIndex != selectedIndex)
            {
                if (playAnimation && CurrTabContent)
                {
                    int deltaIndex = selectedIndex - CurrTabIndex;
                    int sign = (int)Mathf.Sign(deltaIndex);
                    pageSize = CurrTabContent.RectTransform.rect.width;
                    visibleTabContents.Add(CurrTabContent);
                    UpdateVisibility(-deltaIndex);
                    SmoothChangePage(deltaIndex, pageSize * -sign * Vector2.right);
                }
                else
                {
                    base.FocusTab(tabItem);
                    onPageChanged(tabItem);
                }
            }
        }

        protected override void SetFocusTab(UITabItem tabItem)
        {
            if (playAnimation && CurrTabContent)
            {
                if (CurrTabItem) CurrTabItem.Selected = false;

                CurrTabIndex = TabItems.IndexOf(tabItem);
                CurrTabItem = tabItem;
                CurrTabItem.Selected = true;
                CurrTabContent = tabItem.tabContent;

                onTabSelected(CurrTabItem);
            }
            else
            {
                base.SetFocusTab(tabItem);
            }
        }
        #endregion

        #region event drag handlers
        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
        {
            forwardParentDrag.OnInitializePotentialDrag(eventData);
        }

        /// <summary>
        /// Begin drag event
        /// </summary>
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            forwardParentDrag.OnBeginDrag(eventData);

            if (!dragEnabled) return;

            if (forwardParentDrag.RouteToParent) return;

            isOutBound = false;
            rootPosition = CurrTabContent.RectTransform.anchoredPosition;
            pageSize = CurrTabContent.RectTransform.rect.width;
            visibleTabContents.Clear();
            visibleTabContents.Add(CurrTabContent);
            lastPosition = eventData.pressPosition;
            if (smoothMoving) skipDrag = true;
            else skipDrag = false;
        }

        /// <summary>
        /// Drag event
        /// </summary>
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (forwardParentDrag.RouteToParent)
            {
                forwardParentDrag.OnDrag(eventData);
                return;
            }

            HandleOnDrag(eventData);
        }

        /// <summary>
        /// End drag event
        /// </summary>
        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (forwardParentDrag.RouteToParent)
            {
                forwardParentDrag.OnEndDrag(eventData);
                return;
            }

            HandleOnEndDrag(eventData);
        }

        void HandleOnDrag(PointerEventData eventData)
        {
            if (!dragEnabled || skipDrag) return;

            isOutBound = false;
            float delta = eventData.position.x - lastPosition.x;
            lastPosition = eventData.position;

            if (IsDragOutBound(delta))
            {
                isOutBound = true;
                forwardParentDrag.RouteToParent = true;
                forwardParentDrag.ParentBeginDragHandler?.OnBeginDrag(eventData);
                return;
            }

            int sign = (int)Mathf.Sign(delta);
            UpdateVisibility(sign);
            UpdatePosition(delta);
        }

        void HandleOnEndDrag(PointerEventData eventData)
        {
            if (!dragEnabled || skipDrag || isOutBound) return;

            Vector2 deltaPos = CurrTabContent.RectTransform.anchoredPosition - rootPosition;
            float percent = (eventData.pressPosition.x - eventData.position.x) / pageSize;
            if (Mathf.Abs(percent) >= dragThreshold)
            {
                int sign = -(int)Mathf.Sign(deltaPos.x);
                deltaPos = (-sign) * (Vector2.right * pageSize + deltaPos * sign);

                SmoothChangePage(sign, deltaPos);
            }
            else
            {
                smoothMoving = true;
                visibleTabContents.ForEach(content => SmoothMove(content.RectTransform, -deltaPos, scrollDuration));
                DOVirtual.DelayedCall(scrollDuration, () =>
                {
                    visibleTabContents.ForEach(content => content.gameObject.SetActive(CurrTabItem.tabContent == content));
                    visibleTabContents.Clear();
                    smoothMoving = false;
                });
            }
        }
        #endregion

        #region private methods
        bool IsDragOutBound(float delta)
        {
            Vector2 deltaFromRoot = CurrTabContent.RectTransform.anchoredPosition - rootPosition;
            return (delta > 0 && CurrTabIndex == 0 && deltaFromRoot.x >= 0)
                || (delta < 0 && CurrTabIndex == visiblePage - 1 && deltaFromRoot.x <= 0);
        }

        void UpdateVisibility(int count)
        {
            Vector2 currPosition = CurrTabContent.RectTransform.anchoredPosition;
            Rect currRect = CurrTabContent.RectTransform.rect;
            if (count > 0)
            {
                if (CurrTabIndex - count < 0) return;
                UITabContent tabContent = TabItems[CurrTabIndex - count].tabContent;
                if (!visibleTabContents.Contains(tabContent))
                {
                    Vector2 anchorPos = tabContent.RectTransform.anchoredPosition;
                    anchorPos.x = currPosition.x - currRect.width;
                    anchorPos.y = currPosition.y;
                    tabContent.RectTransform.anchoredPosition = anchorPos;
                    tabContent.Show(true);
                    visibleTabContents.Add(tabContent);
                }

            }
            else
            {
                count = -count;
                if (CurrTabIndex + count >= VisiblePage) return;
                UITabContent tabContent = TabItems[CurrTabIndex + count].tabContent;
                if (!visibleTabContents.Contains(tabContent))
                {
                    Vector2 anchorPos = tabContent.RectTransform.anchoredPosition;
                    anchorPos.x = currPosition.x + currRect.width;
                    anchorPos.y = currPosition.y;
                    tabContent.RectTransform.anchoredPosition = anchorPos;
                    tabContent.Show(true);
                    visibleTabContents.Add(tabContent);
                }

            }
        }

        void UpdatePosition(float delta)
        {
            Vector2 deltaPos = (1f - dragDamp) * delta * Vector2.right;
            Vector2 deltaFromRoot = CurrTabContent.RectTransform.anchoredPosition - rootPosition;
            float xFromRoot = Mathf.Abs(deltaFromRoot.x);
            if (Mathf.Abs(deltaPos.x) > xFromRoot && xFromRoot != 0)
            {
                deltaPos.x = deltaPos.x > 0 ? xFromRoot : -xFromRoot;
            }
            for (int i = 0; i < visibleTabContents.Count; i++)
            {
                visibleTabContents[i].RectTransform.anchoredPosition += deltaPos;
            }
        }

        void SmoothChangePage(int index, Vector2 deltaPos)
        {
            if (CurrTabIndex + index < 0 || CurrTabIndex + index > VisiblePage - 1)
                return;

            smoothMoving = true;
            SetFocusTab(TabItems[CurrTabIndex + index]);
            visibleTabContents.ForEach(content => SmoothMove(content.RectTransform, deltaPos, scrollDuration));
            onPageWillChange(CurrTabItem);
            DOVirtual.DelayedCall(scrollDuration, () =>
            {
                visibleTabContents.ForEach(content => content.gameObject.SetActive(CurrTabItem.tabContent == content));
                visibleTabContents.Clear();
                smoothMoving = false;
                onPageChanged(CurrTabItem);
            });
            PlayCursorAnim(CurrTabItem.RectTransform);
        }

        void SmoothMove(RectTransform target, Vector2 deltaPos, float duration)
        {
            target.DOAnchorPos(target.anchoredPosition + deltaPos, duration);
        }
        #endregion
    }
}