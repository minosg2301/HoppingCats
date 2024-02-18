using System;

namespace moonNest
{
    [Serializable]
    public class UnlockConditionGroup : BaseData
    {
        public int statId;

        [NonSerialized] private StatDefinition _stat;
        public StatDefinition StatDefinition { get { if(!_stat) _stat = UserPropertyAsset.Ins.FindStat(statId); return _stat; } }

        public UnlockConditionGroup(StatDefinition statDef) : base(statDef.name)
        {
            id = statId = statDef.id;
        }
    }
}