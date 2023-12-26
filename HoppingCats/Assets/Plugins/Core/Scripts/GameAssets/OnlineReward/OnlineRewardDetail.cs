using System;
using System.Collections.Generic;

namespace moonNest
{
    [Serializable]
    public class OnlineRewardDetail : BaseData
    {
        public int minutes;
        public int serverId;
        public List<RewardDetail> rewards = new List<RewardDetail>();

        public OnlineRewardDetail(string name) : base(name) { }

        public int Seconds => minutes * 60;

        public override string ToString() => minutes + " minutes";
    }
}