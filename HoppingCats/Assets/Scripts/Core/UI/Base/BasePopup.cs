using Doozy.Engine.UI;
using System;
using UnityEngine;

namespace vgame
{
    public enum PopupShowMethod
    {
        /// <summary>
        /// Default Method, push pop to queue then show
        /// </summary>
        QUEUE,

        /// <summary>
        /// No Queue, show popup immediately when show called
        /// </summary>
        NO_QUEUE,

        /// <summary>
        /// Advance methods, wait util popup queue is empty when push this popup to queue then show
        /// </summary>
        QUEUE_EMPTY
    }

    [RequireComponent(typeof(UIPopup))]
    public abstract class BasePopup : MonoBehaviour
    {
        #region static methods
        /// <summary>
        /// Show popup by name
        /// </summary>
        /// <param name="name"></param>
        protected static UIPopup Show(string name, PopupShowMethod showMethod = PopupShowMethod.QUEUE)
        {
            UIPopup popup = UIPopupManager.GetPopup(name);
            if (!popup) throw new NullReferenceException($"Popup {name} does not exist!");
            ShowPopup_Internal(popup, showMethod);
            return popup;
        }

        /// <summary>
        /// Show popup by name and get popup instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected static T Show<T>(string name, PopupShowMethod showMethod = PopupShowMethod.QUEUE) where T : BasePopup
        {
            UIPopup popup = UIPopupManager.GetPopup(name);
            if (!popup) throw new NullReferenceException($"Popup {name} does not exist!");
            ShowPopup_Internal(popup, showMethod);
            return popup.GetComponent<T>();
        }

        /// <summary>
        /// Show popup with params by NORMAL show method
        /// </summary>
        /// <param name="name"></param>
        /// <param name="params"></param>
        protected static UIPopup ShowWithParams(string name, params object[] @params)
            => ShowWithParamsAndMethod(name, PopupShowMethod.QUEUE, @params);


        /// <summary>
        /// Show popup with params and custom show method
        /// </summary>
        /// <param name="name"></param>
        /// <param name="showMethod"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        protected static UIPopup ShowWithParamsAndMethod(string name, PopupShowMethod showMethod = PopupShowMethod.QUEUE, params object[] @params)
        {
            UIPopup popup = UIPopupManager.GetPopup(name);
            if (!popup) throw new NullReferenceException($"Popup {name} does not exist!");
            popup.GetComponent<BasePopup>().SetParams(@params);
            ShowPopup_Internal(popup, showMethod);
            return popup;
        }

        /// <summary>
        /// Show popup with params by NORMAL show method and get popup instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        protected static T ShowWithParams<T>(string name, params object[] @params) where T : BasePopup
            => ShowWithParamsAndMethod<T>(name, PopupShowMethod.QUEUE, @params);

        /// <summary>
        /// Show popup with params  and custom show method. Then get popup instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        protected static T ShowWithParamsAndMethod<T>(string name, PopupShowMethod showMethod, params object[] @params) where T : BasePopup
        {
            UIPopup popup = UIPopupManager.GetPopup(name);
            if (!popup) throw new NullReferenceException($"Popup {name} does not exist!");
            popup.GetComponent<BasePopup>().SetParams(@params);
            ShowPopup_Internal(popup, showMethod);
            return popup.GetComponent<T>();
        }

        static void ShowPopup_Internal(UIPopup popup, PopupShowMethod showMethod)
        {
            switch (showMethod)
            {
                case PopupShowMethod.NO_QUEUE: UIPopupManager.ShowPopup(popup, false, false); break;
                case PopupShowMethod.QUEUE_EMPTY: ShowPopupWhenQueueEmpty(popup); break;
                default: UIPopupManager.ShowPopup(popup, true, false); break;
            }
        }

        static void ShowPopupWhenQueueEmpty(UIPopup popup)
        {
            void _OnQueueUpdated()
            {
                if (UIPopupManager.PopupQueue.Count == 0)
                {
                    UIPopupManager.onQueueUpdated -= _OnQueueUpdated;
                    UIPopupManager.ShowPopup(popup, true, false);
                }
            }

            if (UIPopupManager.PopupQueue.Count == 0) UIPopupManager.ShowPopup(popup, true, false);
            else UIPopupManager.onQueueUpdated += _OnQueueUpdated;
        }
        #endregion

        #region event
        public Action onHide = delegate { };
        #endregion

        #region components
        private UIPopup _popup;
        public UIPopup UIPopup { get { if (!_popup) _popup = GetComponent<UIPopup>(); return _popup; } }
        #endregion

        #region unity methods
        protected virtual void Awake()
        {
            UIPopup.ShowBehavior.OnStart.Event.AddListener(OnShow);
            UIPopup.ShowBehavior.OnFinished.Event.AddListener(OnShowDone);
            UIPopup.HideBehavior.OnStart.Event.AddListener(OnWillHide);
            UIPopup.HideBehavior.OnFinished.Event.AddListener(OnHide);
        }
        #endregion

        #region protected methods
        protected virtual void SetParams(params object[] @params) { }
        protected virtual void OnShow() { }
        protected virtual void OnShowDone() { }
        protected virtual void OnWillHide() { }
        protected virtual void OnHide() { onHide(); }
        #endregion

        public void Hide() { UIPopup.Hide(); }
    }
}