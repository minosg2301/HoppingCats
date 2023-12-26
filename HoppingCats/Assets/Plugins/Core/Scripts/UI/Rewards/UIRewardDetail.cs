using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    public class UIRewardDetail : MonoBehaviour
    {
        private UIReward[] _uiRewards;
        public UIReward[] UIRewards { get { if(_uiRewards == null) _uiRewards = GetComponentsInChildren<UIReward>(true); return _uiRewards; } }

        public RewardDetail RewardDetail { get; private set; }

        readonly UIListContainer<RewardObject, UIReward> listContainer = new UIListContainer<RewardObject, UIReward>();

        public virtual void SetData(RewardDetail rewardDetail)
        {
            RewardDetail = rewardDetail;
            listContainer.SetList(transform, RewardDetail.rewards);
        }

        public virtual void SetData(List<RewardObject> rewards)
        {
            RewardDetail = new RewardDetail(rewards);
            listContainer.SetList(transform, rewards);
        }
    }
}