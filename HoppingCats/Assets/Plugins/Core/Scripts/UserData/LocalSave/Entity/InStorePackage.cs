using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    public class InStorePackage : DataObject<IAPPackage>
    {
        [SerializeField] internal bool active = false;
        [SerializeField] internal DateTime promotionEndTime;
        [SerializeField] internal int quantity = 0;
        [SerializeField] internal int purchaseCount = 0;
        [SerializeField] internal List<BaseReward> rewards = new List<BaseReward>();

        public int GroupId => Detail.groupId;
        public int Quantity => quantity;
        public bool Active => active;
        public bool Available => active && !OutStock;
        public bool OutStock => quantity == 0;

        public string OriginProductId => Detail.productId;
        public string AvailableProductId => InPromotion ? Detail.promotionProductId : Detail.productId;

        // promotion getter
        public bool InPromotion => promotionEndTime > DateTime.Now;
        public double PromotionLastSeconds => promotionEndTime.Subtract(DateTime.Now).TotalSeconds;
        public DateTime PromotionEndTime => promotionEndTime;

        private InStorePackageGroup _group;
        public InStorePackageGroup Group { get { if(_group == null) _group = UserStore.Ins.FindGroup(Detail.groupId); return _group; } }

        private RewardDetail _rewardDetail;
        public RewardDetail RewardDetail
        {
            get
            {

                if(_rewardDetail == null)
                {
                    _rewardDetail = rewards == null || rewards.Count == 0
                                    ? Group.GetRewards(Detail)[0]
                                    : new RewardDetail(rewards);
                }
                return _rewardDetail;
            }
        }

        protected override IAPPackage GetDetail() => IAPPackageAsset.Ins.Find(DetailId);

        internal InStorePackage(IAPPackage package) : base(package)
        {
            quantity = package.quantity;
            promotionEndTime = DateTime.Today.AddDays(-1); // disable promotion by default

            if(package.activeOnLoad) Activate();
        }

        internal bool Activate()
        {
            if(active) return false;

            active = true;

            // update promotion
            if(Detail.promotedOnActive)
                promotionEndTime = DateTime.Now.AddMinutes(Detail.promotionDuration);

            UpdateMultiRewards();
            RandomItemOnActive();

            return true;
        }

        internal void UpdateBuy()
        {
            purchaseCount++;

            if(quantity > 0)
                quantity = Mathf.Max(0, quantity - 1);
        }

        /// <summary>
        /// if UseMultiRewards is true, pick a random reward from list rewards in asset
        /// else use single reward in asset
        /// </summary>
        void UpdateMultiRewards()
        {
            if(!Detail.useMultiRewards) return;

            // Use group to get rewards in layer if any
            var _rewardDetails = Group.GetRewards(Detail);

            // pick random reward
            var rewardDetail = _rewardDetails.Random();

            // cached to save data
            rewards.Clear();
            foreach(var ro in rewardDetail.rewards)
            {
                var clone = ro.OriginReward.Clone() as BaseReward;
                rewards.Add(clone);
            }
        }

        /// <summary>
        /// If randomOnActive is true, create new list of reward from IAPPackage to save data. Use this list for actual reward<br/>
        /// Else use reward in asset
        /// </summary>
        void RandomItemOnActive()
        {
            // if not random on active, skip cached reward
            if(!Detail.randomOnActive) return;

            if(Detail.useMultiRewards)
            {
                // update randomItem if any
                foreach(var reward in RewardDetail.rewards)
                    __RandomItem(reward.OriginReward);
            }
            else
            {
                // cache rewards with clone
                foreach(var ro in RewardDetail.rewards)
                {
                    var clone = ro.OriginReward.Clone() as BaseReward;
                    __RandomItem(clone);
                    rewards.Add(clone);
                }
            }

            static void __RandomItem(BaseReward reward)
            {
                // random item reward on active to get specific item
                if(reward is ItemReward itemReward && itemReward.type == ItemRewardType.Random)
                {
                    var randomDetail = ItemAsset.Ins.FindRandomDetail(itemReward.contentId);
                    itemReward.type = ItemRewardType.Specific;
                    itemReward.contentId = randomDetail.DoRandom().contentId;
                }
            }
        }

        public void RefreshRewards()
        {
            _rewardDetail = null;
            UpdateMultiRewards();
            RandomItemOnActive();
            UserStore.Ins.DirtyAndNotify(detailId.ToString());
        }
    }
}