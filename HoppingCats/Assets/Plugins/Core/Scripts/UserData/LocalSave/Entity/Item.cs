using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class Item : DataObject<ItemDetail>
    {
        #region serialize field
        [SerializeField] internal long amount;
        [SerializeField] internal bool locked;
        [SerializeField] internal Dictionary<int, StatValue> stats = new Dictionary<int, StatValue>();
        [SerializeField] internal Dictionary<int, byte> enums = new Dictionary<int, byte>();
        #endregion

        #region properties
        public long Amount => amount;
        public bool Locked => locked;

        public ItemDefinition Definition => Detail.Definition;
        protected override ItemDetail GetDetail() => ItemAsset.Ins.Find(DetailId);
        #endregion

        protected Item() { }

        #region internal methods
        internal void Init(ItemDetail detail)
        {
            id = detailId = detail.id;
            amount = detail.initAmount;
            locked = detail.Definition.unlockedByProgress;

            foreach(var stat in detail.stats)
            {
                StatDefinition statDefinition = detail.Definition.FindStat(stat.name);
                if(statDefinition.savable) stats[stat.id] = stat.value;
            }
        }

        internal void InitDistinct(ItemDetail detail, int customId = 0)
        {
            Init(detail);
            id = customId == 0
                ? UnityEngine.Random.Range(0, int.MaxValue)
                : customId;
        }

        internal void SetStats(DistinctStat[] distinctStats)
        {
            foreach(var stat in distinctStats)
                stats[stat.id] = stat.value;
        }

        internal void SetEnums(DistinctEnum[] distinctEnums)
        {
            foreach(var @enum in distinctEnums)
                enums[@enum.id] = @enum.value;
        }

        internal void Unlock()
        {
            locked = false;
            DirtyAndNotifyUpdate();
        }

        protected void DirtyAndNotifyUpdate()
        {
            var ins = UserInventory.Ins;
            ins.dirty = true;
            ins.Notify(GetType().ToString(), DetailId.ToString());
            ins.onItemUpdated(this);
        }
        #endregion

        #region public methods
        public void AddAmount(int value, UnitApplyType applyType = UnitApplyType.Single)
        {
            long max = Detail.capacity > 0 ? Detail.capacity : long.MaxValue;
            long lastAmount = amount;
            amount = MathExt.Clamp(amount + value, 0, max);
            int diff = (int)(amount - lastAmount);

            UserInventory ins = UserInventory.Ins;
            ins.Notify(GetType().ToString(), DetailId.ToString());
            if(diff > 0) ins.onItemAdded?.Invoke(this, diff, applyType);
            else if(diff < 0) ins.onItemRemoved?.Invoke(this, diff, applyType);
            ins.onItemUpdated(this);
            ins.dirty = true;
        }
        #endregion

        #region item datas
        protected internal StatValue GetStat(int statId)
        {
            if(stats.TryGetValue(statId, out var s))
                return s;

            return -1;
        }

        protected internal StatValue SetStat(int statId, StatValue value)
        {
            stats[statId] = value;
            DirtyAndNotifyUpdate();
            return stats[statId];
        }

        protected internal int GetEnum(int enumId) => enums.TryGetValue(enumId, out var e) ? e : 0;

        protected internal void SetEnum(int enumId, byte @enum)
        {
            enums[enumId] = @enum;
            DirtyAndNotifyUpdate();
        }

        protected internal AttributeDetail Attr(int attrId) => Detail.Attr(attrId);

        protected internal ReferenceDetail Reference(int attrId) => Detail.Reference(attrId);
        #endregion
    }
}