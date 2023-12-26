using System;
using System.Collections.Generic;

namespace moonNest
{
    [Serializable]
    public class LayerGroup : BaseData
    {
        public int statId;
        public bool overrideQuest;
        public bool overrideOnlineReward;
        public bool overrideChest;
        public bool overrideShop;
        public bool overrideIAPOffer;
        public bool overrideBattlePass;
        public List<int> shopIds = new List<int>();
        public List<int> questGroupIds = new List<int>();
        public List<int> packageGroupIds = new List<int>();

        [NonSerialized] private StatDefinition _stat;

        public StatDefinition StatDefinition { get { if(!_stat) _stat = UserPropertyAsset.Ins.FindStat(statId); return _stat; } }

        public LayerGroup(StatDefinition stat) : base(stat.name)
        {
            id = statId = stat.id;
        }
    }
}