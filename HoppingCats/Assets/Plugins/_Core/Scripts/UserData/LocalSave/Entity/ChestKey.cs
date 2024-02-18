using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class ChestKey
    {
        [SerializeField] private int chestId;
        [SerializeField] private int amount;

        public int ChestId => chestId;
        public int Amount => amount;


        private ChestDetail _chest;
        public ChestDetail ChestDetail { get { if(!_chest) _chest = ChestAsset.Ins.Find(chestId); return _chest; } }

        public ChestKey(int id)
        {
            chestId = id;
        }

        public void AddAmount(int value, UnitApplyType applyType = UnitApplyType.Single)
        {
            int lastAmount = amount;
            amount = Math.Max(0, amount + value);
            int diff = amount - lastAmount;

            UserInventory.Ins.dirty = true;
            UserInventory.Ins.Notify(chestId.ToString());

            if(diff > 0) UserInventory.Ins.onChestKeyAdded?.Invoke(this, diff, applyType);
            else if(diff < 0) UserInventory.Ins.onChestKeyRemoved?.Invoke(this, diff, applyType);
        }
    }
}