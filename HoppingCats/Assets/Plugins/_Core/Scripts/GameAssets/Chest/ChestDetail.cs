using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class ChestDetail : BaseData
    {
        public string displayName = "";
        public Sprite icon;
        public Sprite keyIcon;
        public ChestContent content = new ChestContent();

        // for layer override
        public bool layerEnabled;

        // use for tracking
        public string trackingId = "";

        public ChestDetail(string name) : base(name)
        {
            displayName = name;
        }
    }

    [Serializable]
    public class ChestContent
    {
        public List<CurrencyReward> currencies = new List<CurrencyReward>();
        public List<ItemReward> items = new List<ItemReward>();

        public ChestContent Clone()
        {
            ChestContent chestContent = new ChestContent();
            chestContent.currencies = currencies.Map(c => c.Clone() as CurrencyReward);
            chestContent.items = items.Map(c => c.Clone() as ItemReward);
            return chestContent;
        }
    }
}