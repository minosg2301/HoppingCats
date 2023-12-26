using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace moonNest
{
    [Serializable]
    public class IAPPackage : BaseData
    {
        public int groupId;
        public string productId = "";

        // display config
        public Sprite icon;
        public string displayName = "";
        public string description = "";
        public UIIAPPackage customPrefab;

        // behaviour config
        public ProductType type;
        public int quantity = -1;
        public bool activeOnLoad = true;
        public bool free = false;
        public bool promotedOnActive = false;
        public bool randomOnActive = false;

        // Random config
        public bool showAlways = false;

        // promotion config
        public string promotionProductId = "";
        public string promotionTitle = "";
        public string promotionDescription = "";
        public Sprite promotionIcon;
        public int promotionDuration = 300;

        // decoration
        public Sprite decorBackground;
        public string decorContent;
        public int saleOff = 0;

        // contents
        public bool useMultiRewards = false;
        public List<RewardDetail> rewards = new List<RewardDetail>();

        // tracking
        public string trackingId = "";

        public IAPPackage(string name, int groupId) : base(name)
        {
            this.groupId = groupId;
            this.rewards.Add(new RewardDetail(""));
        }

        public override string ToString() => name + " - " + productId;
    }

    public enum IAPDecoreType { NONE, HOT, BEST }
}