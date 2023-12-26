using Doozy.Engine.UI;
using System;
using UnityEngine;

namespace moonNest
{
    [RequireComponent(typeof(UIPopup))]
    public abstract class BaseToast : MonoBehaviour
    {
        public static BaseToast Ins { get; private set; }

        #region Public Param
        public UIToastContent toastContent;
        public float hideContentAfter = 3f;
        public int maxContent = 10;
        #endregion

        #region events
        public Action onHide = delegate { };
        #endregion

        #region properties

        private UIPopup _popup;
        public UIPopup UIPopup { get { if (!_popup) _popup = GetComponent<UIPopup>(); return _popup; } }
        #endregion


        #region private field
        
        private int count = 0;
        public bool Hiding { get; set; } = false;
        public bool Showing { get; set; } = false;

        #endregion

        protected static void Show(string name)
        {
            if (Ins)
            {
                Ins.HandleClick();
                return;
            }

            var popup = UIPopupManager.GetPopup(name);
            if (!popup) throw new NullReferenceException($"Toast {name} does not exist!");
            UIPopupManager.ShowPopup(popup, false, false);
        }

        protected static void ShowWithParams(string name, params object[] @params)
        {
            if (Ins)
            {
                Ins.HandleClick();
                return;
            }

            UIPopup popup = UIPopupManager.GetPopup(name);
            if (!popup) throw new NullReferenceException($"Toast {name} does not exist!");

            var toast = popup.GetComponent<BaseToast>();
            toast.SetParams(@params);
            UIPopupManager.ShowPopup(popup, false, false);
        }

        #region unity methods
        void Awake()
        {
            toastContent.gameObject.SetActive(false);
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

            HandleClick();
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


        private void HandleClick()
        {
            if(maxContent <= count) return;
            
            count++;
            var content = Instantiate(toastContent, UIPopup.Container.RectTransform);
            if (!content.completeAnimation)
            {
                UIPopup.AutoHideAfterShowDelay = 3f;
                UIPopup.AutoHideAfterShow = true;
                return;
            }
            content.completeAnimation.hasOnComplete = true;
            content.completeAnimation.onComplete.AddListener(() => HandleCompleteAnimation(content));
            content.gameObject.SetActive(true);
        }

        private void HandleCompleteAnimation(UIToastContent toastContent)
        {
            count--;
            toastContent.gameObject.SetActive(false);
            if(count == 0)
                UIPopup.Hide();
        }
    }
}

