using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using DG.Tweening;
using Doozy.Engine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace moonNest
{
    /// <summary>
    /// UI for User Progressor, use OSA
    /// </summary>
    public class UIUserProgressorOSA : OSA<UserProgressorParams, ProgressDetailViewHolder>
    {
        public UserStatId stat = -1;
        public TextMeshProUGUI currentValueText;
        public UIStatProgress statProgress;
        public Image fillBar;
        public RectTransform cursor;
        public UIButton focusButton;
        public float focusInset = 0.5f;

        StatProgressGroup group;
        StatProgressGroupDetail groupDetail;
        List<ProgressDetail> progresses = new List<ProgressDetail>();
        List<UIStatProgress> visibleItems = new List<UIStatProgress>();

        private Canvas _canvas;
        public Canvas Canvas { get { if(!_canvas) _canvas = GetComponentInParent<Canvas>(); return _canvas; } }

        int currentValue;
        ProgressDetailViewHolder minView, maxView;
        float minHalfStepToNext, maxHalfStepToNext;

        void OnValidate()
        {
            StatDefinition statDef = UserPropertyAsset.Ins.properties.FindStat(stat);
            gameObject.name = "UIUserProgressorOSA - " + (statDef ? statDef.name : "");
        }

        #region unity methods
        protected override void Awake()
        {
            base.Awake();

            if(statProgress.transform.parent) statProgress.gameObject.SetActive(false);

            if(focusButton)
            {
                focusButton.gameObject.SetActive(false);
                focusButton.OnClick.OnTrigger.Event.AddListener(() => FocusCurrent(0.5f));
            }
        }

        protected override void Start()
        {
            base.Start();

            UpdateProgresses();
            ResetItems(progresses.Count);
            FocusCurrent(0);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateProgresses();

            if(IsInitialized)
            {
                currentValue = UserData.Stat(groupDetail.statId);
                UpdateFillBar();
                // update ui
                int count = VisibleItemsCount;
                for(int i = 0; i < count; i++)
                    GetItemViewsHolder(i).UIProgressNode.UpdateUI();

                FocusCurrent(0);
            }
        }
        #endregion

        #region override methods
        protected override void OnScrollPositionChanged(double normPos)
        {
            base.OnScrollPositionChanged(normPos);

            if(!fillBar) return;

            currentValue = UserData.Stat(groupDetail.statId);
            visibleItems.Clear();

            float minY = 1000;
            int max = 0, min = int.MaxValue;
            ProgressDetailViewHolder _minView = null, _maxView = null;
            int count = VisibleItemsCount;
            for(int i = 0; i < count; i++)
            {
                ProgressDetailViewHolder view = GetItemViewsHolder(i);
                UIStatProgress ui = view.UIProgressNode;
                int requireValue = ui.RequireValue;
                if(requireValue > max) { max = requireValue + (int)ui.Detail.StepToNext; _maxView = view; }
                if(requireValue < min) { min = requireValue; _minView = view; }

                float absY = Math.Abs(view.root.transform.position.y);
                if(absY < minY)
                {
                    minY = absY;
                }

                visibleItems.Add(ui);
            }

            // cached min/max View
            if(_minView != minView)
            {
                minView = _minView;
                minHalfStepToNext = minView.ProgressDetail.StepToNext * 0.5f;
            }
            if(_maxView != maxView)
            {
                maxView = _maxView;
                maxHalfStepToNext = maxView.ProgressDetail.StepToNext * 0.5f;
            }

            cursor.gameObject.SetActive(false);

            if(currentValue < min - minHalfStepToNext)
            {
                fillBar.fillAmount = 0;
                focusButton.gameObject.SetActive(true);
            }
            else if(currentValue > max + maxHalfStepToNext)
            {
                fillBar.fillAmount = 1;
                focusButton.gameObject.SetActive(true);
            }
            else
            {
                UpdateFillBar();
                focusButton.gameObject.SetActive(false);
            }
        }

        protected override ProgressDetailViewHolder CreateViewsHolder(int itemIndex)
        {
            ProgressDetailViewHolder view = new ProgressDetailViewHolder(this);
            view.Init(Parameters.ItemPrefab, Content, itemIndex);
            return view;
        }

        protected override void UpdateViewsHolder(ProgressDetailViewHolder newOrRecycled)
        {
            newOrRecycled.SetData(group, progresses[newOrRecycled.ItemIndex]);

            if(newOrRecycled.RequestUpdateSize) ScheduleComputeVisibilityTwinPass();
        }
        #endregion

        #region private methods
        void FocusCurrent(float duration)
        {
            if(!IsInitialized) return;

            int index = progresses.FindLastIndex(p => p.requireValue <= currentValue);
            SmoothScrollTo(index, duration, 0.5f, focusInset);
        }

        void UpdateProgresses()
        {
            if(!groupDetail)
            {
                if(stat != -1)
                {
                    groupDetail = StatProgressAsset.Ins.FindGroup(stat);
                    groupDetail = groupDetail.LinkedGroup ? groupDetail.LinkedGroup : groupDetail;
                    progresses = groupDetail.progresses;
                    progresses.SortAsc(progress => progress.requireValue);
                    group = UserProgress.Ins.FindGroup(groupDetail.id);
                }
                else
                {
                    throw new ArgumentException("Stat == -1");
                }
            }
        }

        void UpdateFillBar()
        {
            visibleItems.SortAsc(ui => ui.RequireValue);
            UIStatProgress currentUI = visibleItems.FindLast(ui => currentValue >= ui.RequireValue);
            if(!currentUI) currentUI = visibleItems.Find(ui => currentValue < ui.RequireValue);
            UIStatProgress nextUI = visibleItems.Next(currentUI);
            float delta = currentValue - currentUI.RequireValue;
            Vector2 localPosition = UpdateLocalPosition(delta, currentUI, nextUI);
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Canvas.worldCamera, currentUI.transform.TransformPoint(localPosition));
            RectTransformUtility.ScreenPointToLocalPointInRectangle(fillBar.rectTransform, screenPoint, Canvas.worldCamera, out Vector2 localPoint);

            float pos = GetFillPos(localPoint);
            float maxValue = fillBar.fillMethod == Image.FillMethod.Vertical ? fillBar.rectTransform.rect.height : fillBar.rectTransform.rect.width;
            fillBar.fillAmount = pos / maxValue;

            if(currentValueText) currentValueText.text = currentValue.ToString();
            if(cursor)
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
            if(step == 0) return Vector2.zero;
            float halfStep = step * 0.5f;
            if(fillBar.fillMethod == Image.FillMethod.Vertical)
            {
                float currentSize = current.Rect.rect.height;
                float nextSize = haveNext && currentSize != next.Rect.rect.height ? next.Rect.rect.height : currentSize;
                float pos = (currentSize * Mathf.Min(halfStep, delta) + nextSize * Mathf.Max(0, delta - halfStep)) / step;
                return new Vector2(0, fillBar.fillOrigin == (int)Image.OriginVertical.Bottom ? pos : -pos);
            }
            else if(fillBar.fillMethod == Image.FillMethod.Horizontal)
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
            if(fillBar.fillMethod == Image.FillMethod.Vertical)
            {
                return fillBar.fillOrigin == (int)Image.OriginVertical.Bottom
                    ? _rectTransform.rect.height * _rectTransform.pivot.y + localPoint.y
                    : _rectTransform.rect.height * _rectTransform.pivot.y - localPoint.y;
            }
            else if(fillBar.fillMethod == Image.FillMethod.Horizontal)
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

    public class UICachedProgressInfo
    {
        public int requireValue;
        public float size;
    }

    public class ProgressDetailViewHolder : BaseItemViewsHolder
    {
        readonly UIUserProgressorOSA scroller;

        public ProgressDetail ProgressDetail { get; private set; }
        public UIStatProgress UIProgressNode { get; private set; }
        public Transform Container { get; private set; }

        public bool RequestUpdateSize { get; internal set; }
        public Vector2 SizeDelta { get; internal set; }

        UIStatProgress defaultNode;
        readonly Dictionary<int, UIStatProgress> progressNodes = new Dictionary<int, UIStatProgress>();
        readonly static Dictionary<int, bool> animateds = new Dictionary<int, bool>();

        public ProgressDetailViewHolder(UIUserProgressorOSA scroller)
        {
            this.scroller = scroller;
        }

        public override void CollectViews()
        {
            base.CollectViews();

            Container = root.Find("Container");
        }

        public void SetData(StatProgressGroup group, ProgressDetail progressDetail)
        {
            UIStatProgress node;
            if(progressDetail.customPrefab)
            {
                int key = progressDetail.customPrefab.GetInstanceID();
                if(!progressNodes.TryGetValue(key, out node))
                {
                    node = Object.Instantiate(progressDetail.customPrefab, Container);
                    node.Rect.anchoredPosition = Vector2.zero;
                    progressNodes[key] = node;
                }
            }
            else
            {
                if(!defaultNode)
                {
                    defaultNode = Object.Instantiate(scroller.statProgress, Container);
                    defaultNode.Rect.anchoredPosition = Vector2.zero;
                }

                node = defaultNode;
            }

            if(UIProgressNode) UIProgressNode.gameObject.SetActive(false);

            ProgressDetail = progressDetail;
            UIProgressNode = node;
            UIProgressNode.gameObject.SetActive(true);
            UIProgressNode.SetData(group, progressDetail);

            if(!animateds.ContainsKey(progressDetail.id))
            {
                animateds[progressDetail.id] = true;
                UIProgressNode.PlayShowAnimation();
            }

            RequestUpdateSize = true;
            SizeDelta = UIProgressNode.Rect.sizeDelta;
        }

        public override void MarkForRebuild()
        {
            if(RequestUpdateSize)
            {
                RequestUpdateSize = false;
                root.sizeDelta = SizeDelta;
            }

            base.MarkForRebuild();
        }
    }

    [Serializable]
    public class UserProgressorParams : BaseParamsWithPrefab { }
}