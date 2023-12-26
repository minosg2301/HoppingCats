using System;
using System.Collections.Generic;

namespace moonNest
{
    [Serializable]
    public class LayerRewardDetail : RewardDetail
    {
        public int layerId;
    }

    [Serializable]
    public class LayerRewardList
    {
        public int layerId;
        public List<RewardDetail> rewards = new List<RewardDetail>();
    }
}