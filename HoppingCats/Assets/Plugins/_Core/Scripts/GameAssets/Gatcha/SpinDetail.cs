using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class SpinDetail : BaseData
    {
        public bool freePerDay = true;
        public int multiSpin = 5;
        public float multiSpinDiscount = 0.8f;
        public PriceConfig cost = new PriceConfig();
        public List<SpinItemConfig> spinItems = new List<SpinItemConfig>();

        public bool pointEnabled = false;
        public Sprite pointIcon;
        public List<PointReward> pointRewards = new List<PointReward>();

        // for editor
        public GatchaDrawMethod drawMethod;

        public SpinDetail(string name) : base(name) { }

        public SpinItemConfig FindSpinItem(int id) => spinItems.Find(_ => _.id == id);
    }

    [Serializable]
    public class SpinItemConfig : BaseGatchaItemDetail
    {
        public SpinItemConfig(string name) : base(name) { }
    }
}