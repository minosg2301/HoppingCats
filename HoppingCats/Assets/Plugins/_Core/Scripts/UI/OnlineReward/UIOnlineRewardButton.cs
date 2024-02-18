using Doozy.Engine.UI;
using UnityEngine;

namespace moonNest
{
    public class UIOnlineRewardButton : MonoBehaviour, IObserver
    {
        public UIButton button;
        public UICountDownTime countDownTime;
        public UIReward reward;
        public GameObject highlightNode;

        public int MaxSeconds { get; private set; }

        private OnlineReward nextOnlineReward;

        void Awake()
        {
            button.OnClick.OnTrigger.Event.AddListener(OnClick);
        }

        void Reset()
        {
            if (!button) button = GetComponent<UIButton>();
            if (!countDownTime) countDownTime = GetComponentInChildren<UICountDownTime>();
            if (!reward) reward = GetComponentInChildren<UIReward>();
        }

        void OnEnable()
        {
            UserOnlineReward.Ins.UpdateOnlineDuration();
            UserOnlineReward.Ins.Subscribe(this);
        }

        void OnDisable()
        {
            UserOnlineReward.Ins.Unsubscribe(this);
        }

        void LateUpdate()
        {
            if (nextOnlineReward != null)
            {
                if (countDownTime && countDownTime.gameObject.activeSelf && countDownTime.Seconds <= 0)
                {
                    countDownTime.gameObject.SetActive(false);
                    if (highlightNode) highlightNode.SetActive(true);
                }
            }
        }

        public virtual void HandleOnClick() { }

        public void OnClick()
        {
            if (nextOnlineReward != null)
            {
                if (countDownTime.Seconds > 0)
                {
                    HandleOnClick();
                }
                else
                {
                    if (UserOnlineReward.Ins.FirstTimeReceive)
                    {
                        UserOnlineReward.Ins.DisableFirstTime();
                        HandleOnClick();
                    }
                    else
                    {
                        UserOnlineReward.Ins.ClaimReward();
                    }
                }
            }
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            nextOnlineReward = UserOnlineReward.Ins.Rewards.Find(onlineReward => !onlineReward.Claimed);
            gameObject.SetActive(nextOnlineReward != null);
            if (nextOnlineReward != null)
            {
                MaxSeconds = nextOnlineReward.Detail.Seconds;

                if (reward) reward.SetData(nextOnlineReward.Reward.rewards[0]);

                float remainingTime = Mathf.Max(0, MaxSeconds - UserOnlineReward.Ins.OnlineDuration);
                if (remainingTime > 0)
                {
                    if (highlightNode) highlightNode.SetActive(false);
                    if (countDownTime)
                    {
                        countDownTime.gameObject.SetActive(true);
                        countDownTime.StartWithDuration(remainingTime);
                    }
                }
                else
                {
                    if (highlightNode) highlightNode.SetActive(true);
                    if (countDownTime)
                    {
                        countDownTime.gameObject.SetActive(false);
                        countDownTime.StartWithDuration(0);
                    }
                }
            }
        }
    }
}