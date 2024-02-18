using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class OnlineReward
    {
        [SerializeField] private int id;
        [SerializeField] private int rewardId;
        [SerializeField] private bool claimed;

        public int Id => id;
        public int RewardId => rewardId;
        public RewardDetail Reward { get; internal set; }
        public bool Claimed { get { return claimed; } internal set { claimed = value; } }

        public OnlineRewardDetail Detail => OnlineRewardAsset.Ins.Find(id);

        public OnlineReward(OnlineRewardDetail detail, int rewardId)
        {
            id = detail.id;
            claimed = false;
            this.rewardId = rewardId;
        }

        public override string ToString() => Detail.minutes + " Minutes";
    }
}