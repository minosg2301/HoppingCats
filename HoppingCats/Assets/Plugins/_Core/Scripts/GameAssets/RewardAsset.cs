using System;
using System.Collections.Generic;

namespace moonNest
{
    public class RewardAsset : SingletonScriptObject<RewardAsset>
    {        
        public bool testInviteReward;
        public List<InviteReward> inviteRewards = new List<InviteReward>();
    }

    [Serializable]
    public class InviteReward : Cloneable
    {
        public int require;
        public RewardDetail rewardDetail = new RewardDetail();
    }    
}