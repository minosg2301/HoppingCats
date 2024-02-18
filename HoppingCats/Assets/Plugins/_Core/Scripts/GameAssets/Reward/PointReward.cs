using System;

namespace moonNest
{
    [Serializable]
    public class PointReward : ICloneable
    {
        public int point = 10;
        public RewardDetail reward = new RewardDetail();

        public object Clone()
        {
            return new PointReward() { point = point, reward = reward.Clone() as RewardDetail };
        }
    }
}