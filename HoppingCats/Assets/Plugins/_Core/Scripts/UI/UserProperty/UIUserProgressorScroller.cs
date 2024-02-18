using EnhancedUI.EnhancedScroller;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Doozy.Engine.UI;
using System.Collections;

namespace moonNest
{
    public class UIUserProgressorScroller : EnhancedScroller, IEnhancedScrollerDelegate
    {
        [Header("User Progressor")]
        public UserStatId stat = -1;
        public TextMeshProUGUI currentValueText;
        public UIStatProgress defaultPrefab;
        public Image fillBar;
        public RectTransform cursor;
        public UIButton focusButton;

        StatProgressGroup group;
        StatProgressGroupDetail groupDetail;
        List<ProgressDetail> progresses = new List<ProgressDetail>();

        UIStatProgress minView, maxView;
        List<UIStatProgress> visibleViews = new List<UIStatProgress>();

        int currentValue, maxValue, minValue;
        float minHalfStepToNext, maxHalfStepToNext;
        bool willUpdateFillBarCorountine;

        Dictionary<int, bool> animateds = new Dictionary<int, bool>();

        private Canvas _canvas;
        public Canvas Canvas { get { if (!_canvas) _canvas = GetComponentInParent<Canvas>(); return _canvas; } }

        #region unity methods
        void OnValidate()
        {
            StatDefinition statDef = UserPropertyAsset.Ins.properties.FindStat(stat);
            gameObject.name = "UIUserProgressorScroller - " + (statDef ? statDef.name : "");
        }

        protected override void Awake()
        {
            base.Awake();

            if (focusButton)
            {
                focusButton.gameObject.SetActive(false);
                focusButton.OnClick.OnTrigger.Event.AddListener(() => FocusCurrent(0.5f));
            }
        }

        protected override void Start()
        {
            base.Start();

            Delegate = this;
            cellViewVisibilityChanged += OnCellViewVisibilityChanged;

            UpdateProgresses();
            ReloadData();
            FocusCurrent(0);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ScrollRect.onValueChanged.AddListener(OnScroll);

            if (groupDetail)
            {
                currentValue = UserData.Stat(groupDetail.statId);
                UpdateFillBar();
                foreach (var view in visibleViews) view.UpdateUI();
                FocusCurrent(0);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ScrollRect.onValueChanged.RemoveListener(OnScroll);
        }
        #endregion

        #region IEnhancedScrollerDelegate methods
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var prefab = progresses[dataIndex].customPrefab;
            if (!prefab) prefab = defaultPrefab;

            var cellView = GetCellView(prefab);
            cellView.gameObject.SetActive(true);
            cellView.GetComponent<UIStatProgress>().SetData(group, progresses[dataIndex]);
            return cellView;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            var prefab = progresses[dataIndex].customPrefab;
            if (!prefab) prefab = defaultPrefab;
            return prefab.Rect.rect.height;
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return progresses.Count;
        }

        void OnCellViewVisibilityChanged(EnhancedScrollerCellView cellView)
        {
            UpdateVisibleViews();

            // if visible view updated, update fillbar in next frame
            // skip updating fillbar in ScrollRect.onValueChanged callback at this time
            willUpdateFillBarCorountine = true;
            StartCoroutine(UpdateFillBarCorountine());

            if (!cellView.active) return;

            var ui = cellView as UIStatProgress;
            if (!animateds.ContainsKey(ui.Detail.id))
            {
                animateds[ui.Detail.id] = true;
                ui.PlayShowAnimation();
            }
        }

        // update fillbar in next frame
        IEnumerator UpdateFillBarCorountine()
        {
            yield return 0;
            willUpdateFillBarCorountine = false;
            OnScroll(Vector2.zero);
        }
        #endregion

        #region private methods
        void OnScroll(Vector2 val)
        {
            if (!fillBar || willUpdateFillBarCorountine) return;

            if (currentValue < minValue - minHalfStepToNext)
            {
                fillBar.fillAmount = 0;
                focusButton.gameObject.SetActive(true);
                cursor.gameObject.SetActive(false);
            }
            else if (currentValue > maxValue + maxHalfStepToNext)
            {
                fillBar.fillAmount = 1;
                focusButton.gameObject.SetActive(true);
                cursor.gameObject.SetActive(false);
            }
            else
            {
                UpdateFillBar();
                focusButton.gameObject.SetActive(false);
            }
        }

        void UpdateVisibleViews()
        {
            visibleViews.Clear();

            maxValue = 0;
            minValue = int.MaxValue;
            var activeCellViews = ActiveCellViews;
            int count = activeCellViews.Count;
            UIStatProgress _minView = null, _maxView = null;
            for (int i = 0; i < count; i++)
            {
                var cellView = activeCellViews[i];
                UIStatProgress view = cellView as UIStatProgress;
                int requireValue = view.RequireValue;
                if (requireValue > maxValue) { maxValue = requireValue + (int)view.Detail.StepToNext; _maxView = view; }
                if (requireValue < minValue) { minValue = requireValue; _minView = view; }
                visibleViews.Add(view);
            }
            visibleViews.SortAsc(ui => ui.RequireValue);

            // cached min/ max View
            if (_minView && _minView != minView)
            {
                minView = _minView;
                minHalfStepToNext = minView.Detail.StepToNext * 0.5f;
            }
            if (_maxView && _maxView != maxView)
            {
                maxView = _maxView;
                maxHalfStepToNext = maxView.Detail.StepToNext * 0.5f;
            }
        }

        void FocusCurrent(float duration)
        {
            int index = progresses.FindLastIndex(p => p.requireValue <= currentValue);
            JumpToDataIndex(index, 0.5f, 0.5f, true, TweenType.easeOutSine, duration);
        }

        void UpdateProgresses()
        {
            if (!groupDetail)
            {
                if (stat != -1)
                {
                    groupDetail = StatProgressAsset.Ins.FindGroup(stat);
                    groupDetail = groupDetail.LinkedGroup ? groupDetail.LinkedGroup : groupDetail;
                    progresses = groupDetail.progresses;
                    progresses.SortAsc(progress => progress.requireValue);
                    group = UserProgress.Ins.FindGroup(groupDetail.id);
                    currentValue = UserData.Stat(groupDetail.statId);
                }
                else
                {
                    throw new ArgumentException("Stat == -1");
                }
            }
        }

        void UpdateFillBar()
        {
            if (visibleViews.Count == 0) return;

            UIStatProgress currentUI = visibleViews.FindLast(ui => currentValue >= ui.RequireValue);
            if (!currentUI) currentUI = visibleViews.Find(ui => currentValue < ui.RequireValue);
            UIStatProgress nextUI = visibleViews.Next(currentUI);
            float delta = currentValue - currentUI.RequireValue;
            Vector2 localPosition = UpdateLocalPosition(delta, currentUI, nextUI);
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Canvas.worldCamera, currentUI.transform.TransformPoint(localPosition));
            RectTransformUtility.ScreenPointToLocalPointInRectangle(fillBar.rectTransform, screenPoint, Canvas.worldCamera, out Vector2 localPoint);

            float pos = GetFillPos(localPoint);
            float maxValue = fillBar.fillMethod == Image.FillMethod.Vertical ? fillBar.rectTransform.rect.height : fillBar.rectTransform.rect.width;
            fillBar.fillAmount = pos / maxValue;

            if (currentValueText) currentValueText.text = currentValue.ToString();
            if (cursor)
            {
                cursor.gameObject.SetActive(true);
                cursor.anchoredPosition = fillBar.fillMethod == Image.FillMethod.Vertical
                    ? new Vector2(cursor.anchoredPosition.x, localPoint.y)
                    : new Vector2(localPoint.x, cursor.anchoredPosition.y);
            }
        }

        Vector2 UpdateLocalPosition(float delta, UIStatProgress current, UIStatProgress next)
        {
            bool haveNext = next != null;
            float step = current.Detail.StepToNext;
            if (step == 0) return Vector2.zero;
            float halfStep = step * 0.5f;
            if (fillBar.fillMethod == Image.FillMethod.Vertical)
            {
                float currentSize = current.Rect.rect.height;
                float nextSize = haveNext && currentSize != next.Rect.rect.height ? next.Rect.rect.height : currentSize;
                float pos = (currentSize * Mathf.Min(halfStep, delta) + nextSize * Mathf.Max(0, delta - halfStep)) / step;
                return new Vector2(0, fillBar.fillOrigin == (int)Image.OriginVertical.Bottom ? pos : -pos);
            }
            else if (fillBar.fillMethod == Image.FillMethod.Horizontal)
            {
                float currentSize = current.Rect.rect.width;
                float nextSize = haveNext && currentSize != next.Rect.rect.width ? next.Rect.rect.width : currentSize;
                float pos = (currentSize * Mathf.Min(halfStep, delta) + nextSize * Mathf.Max(0, delta - halfStep)) / step;
                return new Vector2(fillBar.fillOrigin == (int)Image.OriginHorizontal.Left ? pos : -pos, 0);
            }
            else
            {
                throw new ArgumentException($"Fill Method {fillBar.fillMethod} is not supported!!");
            }
        }

        float GetFillPos(Vector2 localPoint)
        {
            RectTransform _rectTransform = fillBar.rectTransform;
            if (fillBar.fillMethod == Image.FillMethod.Vertical)
            {
                return fillBar.fillOrigin == (int)Image.OriginVertical.Bottom
                    ? _rectTransform.rect.height * _rectTransform.pivot.y + localPoint.y
                    : _rectTransform.rect.height * _rectTransform.pivot.y - localPoint.y;
            }
            else if (fillBar.fillMethod == Image.FillMethod.Horizontal)
            {
                return fillBar.fillOrigin == (int)Image.OriginHorizontal.Left
                    ? _rectTransform.rect.width * _rectTransform.pivot.x + localPoint.x
                    : _rectTransform.rect.width * _rectTransform.pivot.x - localPoint.x;
            }
            else
            {
                throw new ArgumentException($"Fill Method {fillBar.fillMethod} is not supported!!");
            }
        }
        #endregion
    }
}