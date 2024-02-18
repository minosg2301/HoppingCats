using Doozy.Engine.UI;
using System;
using UnityEngine;

namespace moonNest
{
    [RequireComponent(typeof(UIPopup))]
    public abstract class BaseTooltip : MonoBehaviour
    {
        #region static
        public static BaseTooltip Ins { get; private set; }

        protected static void Show(string name, Transform target)
        {
            if (Ins && Ins.Target != target)
            {
                Ins.Hide();
            }

            var popup = UIPopupManager.GetPopup(name);
            if (!popup) throw new NullReferenceException($"Tooltip {name} does not exist!");

            var tooltip = popup.GetComponent<BaseTooltip>();
            tooltip.SetTarget(target);
            UIPopupManager.ShowPopup(popup, false, false);
        }

        protected static void ShowWithParams(string name, Transform target, params object[] @params)
        {
            if (Ins && Ins.Target != target)
            {
                Ins.Hide();
            }

            UIPopup popup = UIPopupManager.GetPopup(name);
            if (!popup) throw new NullReferenceException($"Tooltip {name} does not exist!");

            var tooltip = popup.GetComponent<BaseTooltip>();
            tooltip.SetTarget(target);
            tooltip.SetParams(@params);
            UIPopupManager.ShowPopup(popup, false, false);
        }
        #endregion

        #region events
        public Action onHide = delegate { };
        #endregion

        #region properties
        public Transform Target { get; private set; }
        public Vector3 TargetPosition { get; private set; }

        private UIPopup _popup;
        public UIPopup UIPopup { get { if (!_popup) _popup = GetComponent<UIPopup>(); return _popup; } }
        #endregion

        public Vector2 offset = Vector2.zero;

        #region private field
        public bool Hiding { get; set; } = false;
        public bool Showing { get; set; } = false;
        #endregion

        #region unity methods
        void Awake()
        {
            UIPopup.ShowBehavior.OnStart.Event.AddListener(OnShow);
            UIPopup.ShowBehavior.OnFinished.Event.AddListener(OnShowFinish);
            UIPopup.HideBehavior.OnStart.Event.AddListener(OnHideStart);
            UIPopup.HideBehavior.OnFinished.Event.AddListener(OnHide);
        }
        #endregion

        #region protected methods
        protected virtual void SetParams(params object[] @params) { }
        protected virtual void OnShow()
        {
            Ins = this;
            Showing = true;

            // update container position
            var camera = UICanvas.GetMasterCanvas().Canvas.worldCamera;
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, TargetPosition);

            // update pivot
            Vector2 pivot = Vector2.right * 0.5f;
            if (screenPoint.x > Screen.width * 0.66f) pivot.x = screenPoint.x / (float)Screen.width;
            else if (screenPoint.x < Screen.width * 0.33f) pivot.x = screenPoint.x / (float)Screen.width;

            if (screenPoint.y > Screen.height * 0.66f) pivot.y = 1f;
            else pivot.y = 0f;

            UIPopup.Container.RectTransform.pivot = pivot;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                UIPopup.Container.RectTransform.parent.GetComponent<RectTransform>(),
                screenPoint, camera, out var position);

            var _offset = offset;
            if ((pivot.x > 0.6f && _offset.x > 0) || (pivot.x < 0.3f && _offset.x < 0))
                _offset.x = -_offset.x;

            if (pivot.y > 0.5f)
                _offset.y = -_offset.y;

            UIPopup.Container.RectTransform.anchoredPosition = position + _offset;
        }
        protected virtual void OnShowFinish() { Showing = false; }
        protected virtual void OnHideStart() { Hiding = true; }
        protected virtual void OnHide()
        {
            if (Ins == this)
                Ins = null;

            onHide();
        }
        #endregion

        void SetTarget(Transform target)
        {
            Target = target;
            TargetPosition = target.position;
        }

        public void Hide() { UIPopup.Hide(); }
    }
}