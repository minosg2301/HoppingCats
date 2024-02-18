using System;

namespace moonNest
{
    [Serializable]
    public class BattlePassLevel : ICloneable
    {
        public int level;
        public int requireValue;
        public RewardDetail reward;
        public RewardDetail premiumReward;

        public BattlePassLevel(int level)
        {
            this.level = level;
            reward = new RewardDetail("Reward");
            premiumReward = new RewardDetail("Premium Reward");
        }

        public object Clone()
        {
            BattlePassLevel clone = new BattlePassLevel(level)
            {
                requireValue = requireValue,
                reward = reward.Clone() as RewardDetail,
                premiumReward = premiumReward.Clone() as RewardDetail
            };
            return clone;
        }

        public override string ToString() => "Level " + level;
    }
}