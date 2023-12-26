using System;
using UnityEngine;

namespace moonNest
{
    public class UIOnlineRewardController : MonoBehaviour, IObserver
    {
        public UICountDownTime refreshTime;

        private RectTransform _rectTransform;
        public RectTransform RectTransform { get { if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }

        readonly UIListContainer<OnlineReward, UIOnlineReward> onlineRewardContainer = new UIListContainer<OnlineReward, UIOnlineReward>();

        public event Action<UIOnlineReward> OnOnlineRewardClicked = delegate { };

        void OnEnable()
        {
            UserOnlineReward.Ins.UpdateOnlineDuration();
            UserOnlineReward.Ins.Subscribe(this);
        }

        void OnDisable()
        {
            foreach (var ui in onlineRewardContainer.UIList)
                ui.OnClick -= HandleOnlineRewardClicked;

            UserOnlineReward.Ins?.Unsubscribe(this);
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            var currentReward = UserOnlineReward.Ins.CurrentReward;
            var haveReward = currentReward != null;
            var duration = haveReward ? currentReward.Detail.Seconds : 0;
            float remainingTime = Mathf.Max(0, duration - UserOnlineReward.Ins.OnlineDuration);

            onlineRewardContainer.SetList(RectTransform, UserOnlineReward.Ins.Rewards, ui =>
            {
                var isHighlight = currentReward == ui.OnlineReward;
                ui.SetHighlight(isHighlight);
                ui.SetCanClaim(isHighlight && remainingTime <= 0);
                ui.SetRemainingTime(remainingTime);
                ui.OnClick += HandleOnlineRewardClicked;
            });

            if (refreshTime) refreshTime.StartWithDuration((float)UserOnlineReward.Ins.LastSeconds);
        }

        void HandleOnlineRewardClicked(UIOnlineReward ui)
        {
            OnOnlineRewardClicked(ui);
        }
    }
}