using System;
using System.Collections.Generic;
using System.Linq;

namespace moonNest
{
    public class ItemAsset : SingletonScriptObject<ItemAsset>
    {
        public List<ItemDefinition> definitions = new List<ItemDefinition>();
        public List<ItemDetail> items = new List<ItemDetail>();
        public List<RandomDetail> itemRandoms = new List<RandomDetail>();

        private readonly Dictionary<int, ItemDetail> itemMap = new Dictionary<int, ItemDetail>();
        private readonly Dictionary<ItemDefinition, List<ItemDetail>> definitionItemsMap = new Dictionary<ItemDefinition, List<ItemDetail>>();

        /// <summary>
        /// Invoke when asset loaded
        /// </summary>
        void Init()
        {
            definitionItemsMap.Clear();
            itemMap.Clear();
            foreach (var item in items.ToList())
            {
                if (!item.Definition)
                {
                    items.Remove(item);
                    continue;
                }
                GetOrCreateList(item.Definition).Add(item);
                itemMap[item.id] = item;
            }
        }

        public void Clear()
        {
            definitionItemsMap.Clear();
            items.Clear();
        }

        #region Item Definition
        /// <summary>
        /// Find Item Definition by id
        /// </summary>
        /// <param name="id">Id of ItemDefinition</param>
        /// <returns></returns>
        public ItemDefinition FindDefinition(int id) => definitions.Find(_ => _.id == id);

        /// <summary>
        /// Find Item Definition by name
        /// </summary>
        /// <param name="name">Name of ItemDefinition</param>
        /// <returns></returns>
        public ItemDefinition FindDefinition(string name) => definitions.Find(_ => _.name == name);

        #endregion

        #region Item Detail

        /// <summary>
        /// Short hand for get/create item list
        /// </summary>
        /// <param name="itemDefinition"></param>
        /// <returns></returns>
        private List<ItemDetail> GetOrCreateList(ItemDefinition itemDefinition)
            => definitionItemsMap.GetOrCreate(itemDefinition, itemDef => new List<ItemDetail>());

        public ItemDetail Find(int id) => items.Find(_ => _.id == id);

        public List<ItemDetail> FindByDefinition(ItemDefinition definition, Predicate<ItemDetail> selector = null)
        {
            if (definition == null) return new List<ItemDetail>();
            if (selector != null)
                return definitionItemsMap.GetOrCreate(definition, (id) => new List<ItemDetail>()).FindAll(selector);
            else
                return definitionItemsMap.GetOrCreate(definition, (id) => new List<ItemDetail>()).ToList();
        }

        public List<ItemDetail> FindByDefinition(int definitionId)
        {
            return FindByDefinition(FindDefinition(definitionId));
        }

        public List<ItemDetail> FindByDefinition(string itemDefinitionName, Predicate<ItemDetail> predicate = null)
        {
            return FindByDefinition(FindDefinition(itemDefinitionName), predicate);
        }

        public List<ItemDetail> FindByUnlockCondition(UnlockConditionDetail unlockCondition)
        {
            List<ItemDetail> items = new List<ItemDetail>();
            foreach (var pair in definitionItemsMap.FindAll(d => d.Key.unlockedByProgress))
            {
                items.AddRange(pair.Value.FindAll(item => item.unlockContentId == unlockCondition.id));
            }
            return items;
        }

        public void RemoveItemByDefinition(ItemDefinition itemDefinition)
        {
            var itemDetails = GetOrCreateList(itemDefinition);
            items.RemoveAll(itemDetails);
        }

        #endregion

        #region Item Random Setting

        public RandomDetail FindRandomDetail(int itemRandomId) => itemRandoms.Find(_ => _.id == itemRandomId);



        #endregion

        #region editor
#if UNITY_EDITOR
        //public void AddItem(ItemDetail item)
        //{
        //    items.Add(item);
        //    GetOrCreateList(item.Definition).Add(item);

        //    // add item in shop asset
        //    if (!item.Definition.sellInShop) return;

        //    var shopDefinition = ShopAsset.Ins.FindShopDefinition(item.definitionId);
        //    var shopDetail = ShopAsset.Ins.FindShop(shopDefinition);
        //    var shopItem = new ShopItemDetail(item.name) { contentId = item.id, shopId = shopDetail.id, type = ShopItemType.Item };
        //    ShopAsset.Ins.AddItem(shopItem);
        //}

        public void Editor_RemoveItem(ItemDetail item)
        {
            items.Remove(item);
            GetOrCreateList(item.Definition).Remove(item);

            // remove in shop asset
            if (!item.Definition.sellInShop) return;

            ShopDefinition shopDefinition = ShopAsset.Ins.FindShopDefinition(item.definitionId);
            ShopDetail shopDetail = ShopAsset.Ins.FindShop(shopDefinition);
            ShopItemDetail shopItem = new ShopItemDetail(item.name) { contentId = item.id, shopId = shopDetail.id, type = ShopItemType.Item };
            ShopAsset.Ins.RemoveItem(shopItem);
        }

        public void Editor_AddItem(ItemDetail item)
        {
            items.Add(item);

            var listByDef = GetOrCreateList(item.Definition);
            if (!listByDef.Contains(item))
            {
                listByDef.Add(item);
            }
        }

        public void Editor_SwapItem(ItemDetail a, ItemDetail b)
        {
            int indexA = items.IndexOf(a);
            int indexB = items.IndexOf(b);
            items[indexA] = b;
            items[indexB] = a;

            var listByDef = GetOrCreateList(a.Definition);
            listByDef.Clear();
            listByDef.AddRange(items.FindAll(i => i.Definition.id == a.definitionId));
        }

        public List<ItemDetail> FindWithFilter(ItemDefinition itemDefinition, ItemFilter itemfilter)
        {
            List<ItemDetail> list = new List<ItemDetail>();
            foreach (var item in FindByDefinition(itemDefinition))
            {
                // match all stat filter
                bool itemStatSatified = true;
                foreach (var statFilter in itemfilter.StatFilters)
                {
                    var itemStat = item.stats.Find(stat => stat.name == statFilter.name);
                    if (itemStat != null)
                    {
                        itemStatSatified = itemStatSatified && statFilter.Satify(itemStat);
                    }

                    if (!itemStatSatified) break;
                }

                // match any enum filter
                bool itemEnumSatified = true;
                foreach (var enumFilter in itemfilter.EnumFilters)
                {
                    var enumDetail = item.EnumDetail(enumFilter.id);
                    if (enumDetail != null)
                    {
                        if (enumFilter.matchType == MatchType.Or)
                        {
                            itemEnumSatified = itemEnumSatified || enumFilter.value == enumDetail.name;
                        }
                        else if (enumFilter.matchType == MatchType.Must)
                        {
                            itemEnumSatified = itemEnumSatified && enumFilter.value == enumDetail.name;

                            if (!itemEnumSatified)
                                break;
                        }
                    }
                }

                if (itemStatSatified && itemEnumSatified)
                {
                    list.Add(item);
                }
            }
            return list;
        }
#endif
        #endregion
    }
}