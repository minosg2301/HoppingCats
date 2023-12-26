using DG.Tweening;
using Doozy.Engine.UI;
using System;
using UnityEngine;

namespace moonNest
{
    [RequireComponent(typeof(UIDrawer))]
    public abstract class BaseDrawer : MonoBehaviour
    {
        public bool autoClose = true;
        public float closeAfter = 2f;

        #region static methods
        /// <summary>
        /// Open drawer
        /// </summary>
        /// <param name="name"></param>
        protected static void Open(string name)
        {
            UIDrawer drawer = UIDrawer.Get(name);
            if(!drawer) { Debug.LogError($"Drawer {name} does not exist!"); return; }
            drawer.Open();
        }

        /// <summary>
        /// Open drawer and get drawer instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected static T Open<T>(string name) where T : BaseDrawer
        {
            UIDrawer drawer = UIDrawer.Get(name);
            if(!drawer) { Debug.LogError($"Drawer {name} does not exist!"); return default; }
            drawer.Open();
            return drawer.GetComponent<T>();
        }

        /// <summary>
        /// Open drawer with params
        /// </summary>
        /// <param name="name"></param>
        /// <param name="params"></param>
        protected static void OpenWithParams(string name, params object[] @params)
        {
            UIDrawer drawer = UIDrawer.Get(name);
            if(!drawer) { Debug.LogError($"Drawer {name} does not exist!"); return; }
            drawer.GetComponent<BaseDrawer>().SetParams(@params);
            drawer.Open();
        }

        /// <summary>
        /// Open drawer with params and get drawer instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        protected static T OpenWithParams<T>(string name, params object[] @params) where T : BaseDrawer
        {
            UIDrawer drawer = UIDrawer.Get(name);
            if(!drawer) { Debug.LogError($"Drawer {name} does not exist!"); return default; }
            drawer.GetComponent<BaseDrawer>().SetParams(@params);
            drawer.Open();
            return drawer.GetComponent<T>();
        }
        #endregion

        #region components
        private UIDrawer _drawer;
        public UIDrawer UIDrawer { get { if(!_drawer) _drawer = GetComponent<UIDrawer>(); return _drawer; } }

        private UICanvas _rootCanvas;
        public UICanvas RootCanvas { get { if (!_rootCanvas) _rootCanvas = GetComponentInParent<UICanvas>(); return _rootCanvas; } }
        #endregion

        #region unity methods
        protected virtual void Awake()
        {
            UIDrawer.OpenBehavior.OnStart.Event.AddListener(OnOpen);
            UIDrawer.OpenBehavior.OnFinished.Event.AddListener(OnOpenFinished);
            UIDrawer.CloseBehavior.OnFinished.Event.AddListener(OnClose);
        }
        #endregion

        #region protected methods
        protected virtual void SetParams(params object[] @params) { }
        protected virtual void OnOpen() { }

        protected virtual void OnOpenFinished()
        {
            if(autoClose) UpdateAutoClose();
        }

        protected virtual void OnClose() { }

        protected virtual void UpdateAutoClose()
        {
            DOVirtual.DelayedCall(closeAfter, () => { if(CheckWillClose()) Close(); });
        }

        protected virtual bool CheckWillClose() => true;
        #endregion

        public void Close() { UIDrawer.Close(); }
    }
}