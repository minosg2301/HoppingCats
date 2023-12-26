using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using moonNest.remotedata;

namespace moonNest
{
    public class UserInventory : RemotableUserData<FirestoreUserData>
    {
        #region static functions
        public static UserInventory Ins => LocalData.Get<UserInventory>();

        public static T Create<T>(int itemDetailId, int customId = 0) where T : Item
        {
            var itemDetail = ItemAsset.Ins.Find(itemDetailId);
            if (!itemDetail)
                throw new NullReferenceException($"Can not FindOrCreate. ItemDetailId {itemDetailId} is not exist in assets");

            return Create<T>(itemDetail, customId);
        }

        public static T Create<T>(ItemDetail detail, int customId = 0) where T : Item => Ins.CreateDistinct(detail, customId) as T;

        public static T FindOrCreate<T>(int itemDetailId) where T : Item
        {
            var itemDetail = ItemAsset.Ins.Find(itemDetailId);
            if (!itemDetail)
                throw new NullReferenceException($"Can not FindOrCreate. ItemDetailId {itemDetailId} is not exist in assets");

            return itemDetail ? FindOrCreate<T>(itemDetail) : null;
        }

        public static T FindOrCreate<T>(ItemDetail detail) where T : Item
        {
            T t = Ins.FindById<T>(detail.id);
            return t ? t : Ins.Create_Internal(detail) as T;
        }

        public static T FindOrCreate<T>(int itemDetailId, DistinctStat[] distincts) where T : Item
        {
            var itemDetail = ItemAsset.Ins.Find(itemDetailId);
            if (!itemDetail)
                throw new NullReferenceException($"Can not FindOrCreate. ItemDetailId {itemDetailId} is not exist in assets");

            return FindOrCreate<T>(itemDetail, distincts);
        }

        public static T FindOrCreate<T>(int itemDetailId, DistinctEnum[] distinctEnums) where T : Item
            => FindOrCreate<T>(itemDetailId, null, distinctEnums);

        public static T FindOrCreate<T>(int itemDetailId, DistinctStat[] distinctStats, DistinctEnum[] distinctEnums) where T : Item
        {
            var itemDetail = ItemAsset.Ins.Find(itemDetailId);
            if (!itemDetail)
                throw new NullReferenceException($"Can not FindOrCreate. ItemDetailId {itemDetailId} is not exist in assets");

            return FindOrCreate<T>(itemDetail, distinctStats, distinctEnums);
        }

        public static T FindOrCreate<T>(ItemDetail detail, DistinctStat[] distinctStats) where T : Item
            => FindOrCreate<T>(detail, distinctStats, null);

        public static T FindOrCreate<T>(ItemDetail detail, DistinctEnum[] distinctEnums) where T : Item
            => FindOrCreate<T>(detail, null, distinctEnums);

        public static T FindOrCreate<T>(ItemDetail detail, DistinctStat[] distinctStats, DistinctEnum[] distinctEnums) where T : Item
        {
            if (!detail)
                throw new NullReferenceException($"Can not FindOrCreate. ItemDetail is null.");

            T t = Ins.FindByDetail_Internal<T>(detail.id, distinctStats, distinctEnums);
            if (t) return t;

            t = Ins.CreateDistinct(detail, -1) as T;
            if (distinctStats != null && distinctStats.Length > 0) t.SetStats(distinctStats);
            if (distinctEnums != null && distinctEnums.Length > 0) t.SetEnums(distinctEnums);
            return t;
        }

        public static T FindByDetail<T>(int detailId, DistinctStat[] distincts) where T : Item
            => Ins.FindByDetail_Internal<T>(detailId, distincts, null);

        public static T FindByDetail<T>(int detailId, DistinctEnum[] distincts) where T : Item
            => Ins.FindByDetail_Internal<T>(detailId, null, distincts);

        public static T FindByDetail<T>(int detailId, DistinctStat[] distinctStats, DistinctEnum[] distinctEnums) where T : Item
            => Ins.FindByDetail_Internal<T>(detailId, distinctStats, distinctEnums);

        public static T Find<T>(int itemId) where T : Item => Ins.FindById<T>(itemId);

        public static List<T> FindAll<T>() where T : Item => Ins.FindAll_Internal<T>();

        public static List<T> FindAll<T>(Predicate<T> predicate) where T : Item => Ins.FindAll_Internal(predicate);

        public static void RemoveAll<T>() where T : Item => Ins.RemoveAll_Internal<T>();

        public static ChestKey Find(int chestId) => Ins.Find_Internal(chestId);

        public static ChestKey FindChestKey(int chestId) => Ins.FindChestKey_Internal(chestId);

        public static ChestKey FindOrCreate(int chestId) => Ins.FindOrCreate_Internal(chestId);
        #endregion

        #region serialize field

        [SerializeField] private Dictionary<int, Item> items = new Dictionary<int, Item>();
        [SerializeField] private Dictionary<int, ChestKey> chestKeys = new Dictionary<int, ChestKey>();
        #endregion

        #region runtime variables
        private Dictionary<ItemDefinition, List<Item>> lockedItems = new Dictionary<ItemDefinition, List<Item>>();
        private Dictionary<ItemDefinition, List<Item>> definitionItemMap = new Dictionary<ItemDefinition, List<Item>>();
        private Dictionary<int, List<Item>> distinctItems = new Dictionary<int, List<Item>>();
        #endregion

        #region properties
        public List<ChestKey> ChestKeys => chestKeys.Values.ToList();
        #endregion

        #region events
        public delegate void ItemAmountEvent(Item item, int amount, UnitApplyType applyType);
        public delegate void ItemEvent(Item item);
        public ItemAmountEvent onItemAdded = delegate { };
        public ItemAmountEvent onItemRemoved = delegate { };
        public ItemEvent onItemCreated = delegate { };
        public ItemEvent onItemUpdated = delegate { };
        public Action<List<Item>> onItemsUnlocked = delegate { };

        public Action<ChestKey, int, UnitApplyType> onChestKeyAdded;
        public Action<ChestKey, int, UnitApplyType> onChestKeyRemoved;
        public Action<ChestKey> onChestKeyUpdated;
        #endregion

        #region OnLoad
        protected internal override void OnLoad()
        {
            InitItems();
        }

        void InitItems()
        {
            // remove data which detail not exists in asset
            items.Values
                .FindAll(item => item.Detail == null || item.Detail.Definition == null)
                .ForEach(item => items.Remove(item.Id));

            // create origin item which id and detailId have same
            foreach (ItemDetail itemDetail in ItemAsset.Ins.items)
            {
                if (!items.ContainsKey(itemDetail.id) && itemDetail.active) Create_Internal(itemDetail);
            }

            definitionItemMap = items.Values.ToListMap(item => item.Detail.Definition);
            lockedItems = definitionItemMap.Filter(definition => definition.unlockedByProgress);

            // init distinct items
            definitionItemMap.ForEach(pair =>
            {
                if (pair.Key.stats.FindAll(stat => stat.distinct).Count > 0)
                {
                    var itemsMap = pair.Value.ToListMap(item => item.DetailId);
                    foreach (var p in itemsMap) distinctItems[p.Key] = p.Value;
                }
            });
        }
        #endregion

        #region item methods
        Item CreateDistinct(ItemDetail detail, int customId = 0)
        {
            if (customId != 0 && items.ContainsKey(customId))
            {
                Debug.LogError($"Item with Id {customId} exists");
            }

            string className = detail.Definition.name;
            Item item = CSharpAssemblyHelper.CreateInstance<Item>(className);
            item.InitDistinct(detail, customId);
            items[item.Id] = item;
            distinctItems.GetOrCreate(item.DetailId, id => new List<Item>()).Add(item);
            definitionItemMap.GetOrCreate(item.Definition, id => new List<Item>()).Add(item);
            return item;
        }

        Item Create_Internal(ItemDetail detail)
        {
            string className = detail.Definition.name;
            Item item = CSharpAssemblyHelper.CreateInstance<Item>(className);
            item.Init(detail);
            items[item.Id] = item;
            if (distinctItems.TryGetValue(item.DetailId, out var list)) list.Add(item);
            definitionItemMap.GetOrCreate(item.Definition, id => new List<Item>()).Add(item);
            onItemCreated(item);
            return item;
        }

        internal T FindById<T>(int itemId) where T : Item => (items.TryGetValue(itemId, out var item) ? item : null) as T;

        internal T FindByDetail_Internal<T>(int detailId, DistinctStat[] stats, DistinctEnum[] enums) where T : Item
        {
            if (stats.Length == 0) return FindById<T>(detailId) as T;

            if (distinctItems.TryGetValue(detailId, out var list) && list.Count > 0)
            {
                if (stats != null) for (int i = 0; i < stats.Length && list.Count > 0; i++) list = list.FindAll(t => Satify(t, stats[i]));
                if (enums != null) for (int i = 0; i < enums.Length && list.Count > 0; i++) list = list.FindAll(t => Satify(t, enums[i]));
                if (list.Count > 0) return list[0] as T;
            }
            return null;
        }

        internal List<T> FindAll_Internal<T>() where T : Item
        {
            string className = typeof(T).Name;
            ItemDefinition itemDefinition = definitionItemMap.Keys.ToList().Find(_ => _.name == className);
            return itemDefinition ? definitionItemMap[itemDefinition].Map(t => t as T) : null;
        }

        internal List<T> FindAll_Internal<T>(Predicate<T> predicate) where T : Item
        {
            string className = typeof(T).Name;
            ItemDefinition itemDefinition = definitionItemMap.Keys.ToList().Find(_ => _.name == className);
            return itemDefinition ? definitionItemMap[itemDefinition].Map(t => (T)t).FindAll(predicate) : null;
        }

        internal List<T> FindAllByDetail_Internal<T>(int detailId, params DistinctStat[] stats) where T : Item
        {
            if (stats.Length == 0) return new List<T>() { Find_Internal(detailId) as T };

            if (distinctItems.TryGetValue(detailId, out var list) && list.Count > 0)
            {
                for (int i = 0; i < stats.Length; i++) list = list.FindAll(t => Satify(t, stats[i]));
                return list.Map(t => (T)t);
            }
            return null;
        }

        internal void RemoveAll_Internal<T>()
        {
            string className = typeof(T).Name;
            ItemDefinition itemDefinition = definitionItemMap.Keys.ToList().Find(_ => _.name == className);
            if (itemDefinition)
            {
                var _items = definitionItemMap[itemDefinition];
                foreach (var item in _items.ToList())
                {
                    _items.Remove(item);
                    items.Remove(item.Id);
                    distinctItems.Remove(item.DetailId);
                }
            }
        }

        bool Satify(Item t, DistinctStat distinctStat)
            => t.stats.TryGetValue(distinctStat.id, out var statValue) && statValue == distinctStat.value;

        bool Satify(Item t, DistinctEnum distinctEnum)
            => t.enums.TryGetValue(distinctEnum.id, out var enumValue) && enumValue == distinctEnum.value;

        internal List<Item> UnlockItems(UnlockConditionDetail unlockCondition)
        {
            List<Item> unlockedItems = new List<Item>();
            if (!unlockCondition) return unlockedItems;
            foreach (KeyValuePair<ItemDefinition, List<Item>> pair in lockedItems)
            {
                unlockedItems.AddRange(
                   pair.Value.FindAll(item => item.Locked
                                               && item.Detail.UnlockContent.UnlockCondition
                                               && item.Detail.UnlockContent.UnlockCondition.id == unlockCondition.id));
            }
            unlockedItems.ForEach(item => item.Unlock());
            dirty = unlockedItems.Count > 0;
            return unlockedItems;
        }
        #endregion

        #region chest key
        internal ChestKey Find_Internal(int chestId) => chestKeys.TryGetValue(chestId, out var chestKey) ? chestKey : null;
        internal ChestKey FindOrCreate_Internal(int chestId) => chestKeys.GetOrCreate(chestId, id => new ChestKey(id));
        internal ChestKey FindChestKey_Internal(int chestId) => chestKeys.TryGetValue(chestId, out var chestKey) ? chestKey : null;
        #endregion

        #region remote methods
        protected override void OnRemoteDataSync(FirestoreUserData remoteData)
        {

        }

        protected override void OnRemoteDataCreated(FirestoreUserData remoteData)
        {

        }
        #endregion
    }
}