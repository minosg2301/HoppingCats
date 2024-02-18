using Doozy.Engine.UI;
using LateExe;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    public class CollectHandler : MonoBehaviour, IObserver
    {
        public CollectType type;
        public AnimConfig animConfig;
        public SoundyPlayer startSfx;
        public SoundyPlayer collectSfx;

        public event Action OnStart = delegate { };
        public event Action OnEnd = delegate { };
        public event Action<int, float> OnProcess = delegate { };

        protected Executer executer;

        #region unity methods
        protected virtual void Awake()
        {
            executer = new Executer(this);
            animConfig.onAnimDone = OnAnimDone;
        }

        protected virtual void Start() { }

        protected virtual void Reset()
        {
            if (!animConfig) animConfig = GetComponentInChildren<AnimConfig>();
        }

        protected virtual void OnEnable()
        {
            UIPopupManager.onQueueUpdated += OnQueueUpdated;
            CollectingManager.Ins.Subcribe(this, type);
        }

        protected virtual void OnDisable()
        {
            UIPopupManager.onQueueUpdated -= OnQueueUpdated;
            CollectingManager.Ins.Unsubcribe(this);
        }
        #endregion

        #region play anim methods
        protected virtual void PlayAnim()
        {
            var requests = CollectingManager.Ins.Find(type, andRemove: true);
            if (requests != null) DoPlay(requests);
        }

        protected void DoPlay<T>(List<T> requests) where T : CollectRequest
        {
            if (startSfx) startSfx.Play();
            OnStart();
            requests.ForEach(PlayCollect);
        }

        protected void DoPlay<T>(T request) where T : CollectRequest
        {
            if (startSfx) startSfx.Play();
            OnStart();
            PlayCollect(request);
        }

        void PlayCollect(CollectRequest request)
        {
            animConfig.Prefab = request.prefab;
            animConfig.Icon = request.icon;
            animConfig.Collect(request.amount);
        }

        protected virtual void OnAnimDone(int count, float percent)
        {
            if (collectSfx) collectSfx.Play();

            OnProcess(count, percent);

            if (percent >= 1)
            {
                HandleCollectingEnd();
            }
        }

        protected void HandleCollectingEnd()
        {
            OnEnd();
        }
        #endregion

        #region events callback
        private bool CanPlayCollect
            => CollectingManager.Ins.AllowCollect
                && (CollectingManager.Ins.AllowPopupCollect || UIPopupManager.PopupQueue.Count == 0);

        void OnQueueUpdated()
        {
            if (CanPlayCollect) PlayAnim();
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            if (CanPlayCollect) PlayAnim();
        }
        #endregion
    }
}