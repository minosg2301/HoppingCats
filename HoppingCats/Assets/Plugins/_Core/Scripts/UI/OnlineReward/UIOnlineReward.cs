using UnityEngine;
using TMPro;
using I2.Loc;
using Doozy.Engine.UI;
using System;

namespace moonNest
{
    public class UIOnlineReward : BaseUIData<OnlineReward>
    {
        public UIButton button;
        public GameObject receivedNode;
        public GameObject highlightNode;
        public TextMeshProUGUI minuteText;
        public UIReward reward;
        public UICountDownTime countDownTime;

        private LocalizationParamsManager _minuteTextParam;
        public LocalizationParamsManager MinuteTextParam { get { if (_minuteTextParam == null) _minuteTextParam = minuteText.GetComponent<LocalizationParamsManager>(); return _minuteTextParam; } }

        public OnlineReward OnlineReward { get; private set; }

        public event Action<UIOnlineReward> OnClick = delegate { };

        void Start()
        {
            if (button) button.OnClick.OnTrigger.Event.AddListener(() => OnClick(this));
        }

        void Reset()
        {
            if (!button) button = GetComponentInChildren<UIButton>();
        }

        void OnEnable()
        {
            if (countDownTime) countDownTime.OnEndCountdown += HandleEndCountdown;
        }

        void OnDisable()
        {
            if (countDownTime) countDownTime.OnEndCountdown -= HandleEndCountdown;
        }

        public override void SetData(OnlineReward onlineReward)
        {
            OnlineReward = onlineReward;
            if (minuteText)
            {
                if (MinuteTextParam) MinuteTextParam.SetParameterValue("minute", onlineReward.Detail.minutes.ToString());
                else minuteText.text = "Minute " + onlineReward.Detail.minutes.ToString();
            }

            if (reward) reward.SetData(onlineReward.Reward.rewards[0]);
            if (receivedNode) receivedNode.SetActive(onlineReward.Claimed);
        }

        public void SetHighlight(bool highlight)
        {
            if (highlightNode) highlightNode.SetActive(highlight);
        }

        public void SetRemainingTime(float seconds)
        {
            if (countDownTime)
            {
                countDownTime.gameObject.SetActive(seconds > 0);
                countDownTime.StartWithDuration(seconds);
            }
        }

        public void SetCanClaim(bool canClaim)
        {
            if (button) button.Interactable = canClaim;
        }

        void HandleEndCountdown()
        {
            if (countDownTime) countDownTime.gameObject.SetActive(false);
            SetData(OnlineReward);
            SetCanClaim(true);
        }
    }
}