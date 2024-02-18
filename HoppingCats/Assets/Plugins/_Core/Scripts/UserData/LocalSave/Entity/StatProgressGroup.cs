using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class StatProgressGroup
    {
        /// <summary>
        /// Id of group
        /// </summary>
        [SerializeField] private int id;

        /// <summary>
        /// Track for premium is updated
        /// </summary>
        [SerializeField] private bool premiumUnlocked;

        /// <summary>
        /// stat values can claims rewards
        /// </summary>
        [SerializeField] internal List<int> canClaims = new List<int>();
        [SerializeField] internal List<int> claimeds = new List<int>();

        /// <summary>
        /// stat values can claims premium rewards
        /// </summary>
        [SerializeField] internal List<int> premiumCanClaims = new List<int>();
        [SerializeField] internal List<int> premiumClaimeds = new List<int>();

        public int Id => id;
        public IReadOnlyList<int> CanClaims => canClaims;
        public IReadOnlyList<int> Claimeds => claimeds;
        public IReadOnlyList<int> PremiumCanClaims => premiumCanClaims;
        public IReadOnlyList<int> PremiumClaimeds => premiumClaimeds;
        public bool PaidPremium => premiumUnlocked;

        private StatProgressGroupDetail _detail;
        public StatProgressGroupDetail Detail { get { if(!_detail) _detail = StatProgressAsset.Ins.FindGroup(id); return _detail; } }

        public StatProgressGroup(StatProgressGroupDetail group)
        {
            id = group.id;
        }

        internal bool UpdateRewardCanClaim(ProgressDetail progressDetail)
        {
            if(!Detail.progresses.Contains(progressDetail)) return false;
            bool ret = UpdateRewardCanClaim(progressDetail, false);
            if(premiumUnlocked) ret |= UpdateRewardCanClaim(progressDetail, true);
            return ret;
        }

        internal bool UpdateRewardCanClaim(ProgressDetail progressDetail, bool premium)
        {
            bool haveReward = (premium ? progressDetail.reward.rewards.Count : progressDetail.premiumReward.rewards.Count) > 0;
            if(!haveReward) return false;

            int id = progressDetail.id;
            List<int> list = premium ? premiumCanClaims : canClaims;
            List<int> claimedLists = premium ? premiumClaimeds : claimeds;
            if(list.Contains(id) || claimedLists.Contains(id)) return false;
            list.Add(id);
            return true;
        }

        internal bool CanClaimReward(ProgressDetail progressDetail, bool premium = false)
            => (premium ? premiumCanClaims : canClaims).Contains(progressDetail.id);

        internal bool ClaimReward(ProgressDetail progressDetail, bool premium = false)
        {
            int id = progressDetail.id;
            List<int> list = premium ? premiumCanClaims : canClaims;
            List<int> claimedLists = premium ? premiumClaimeds : claimeds;
            if(claimedLists.Contains(id) || !list.Contains(id)) return false;
            list.Remove(id);
            claimedLists.Add(id);
            RewardConsumer.ConsumeReward(premium ? progressDetail.premiumReward : progressDetail.reward);
            return true;
        }

        internal bool UnlockPremium()
        {
            if(premiumUnlocked) return false;
            premiumUnlocked = true;
            return true;
        }

        internal void Reset()
        {
            premiumUnlocked = false;
            canClaims.Clear();
            claimeds.Clear();
            premiumCanClaims.Clear();
            premiumClaimeds.Clear();
        }
    }
}