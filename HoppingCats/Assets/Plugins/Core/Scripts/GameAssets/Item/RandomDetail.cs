using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class RandomDetail : BaseData
    {
        public string displayName = "";
        public Sprite icon;
        public List<RandomContentDetail> randoms = new List<RandomContentDetail>();

        // editor filter
        [NonSerialized] public ItemFilter itemFilter = new ItemFilter();

        public RandomDetail()
        {
            itemFilter = new ItemFilter();
        }

        public RandomDetail(string name) : base(name)
        {
            displayName = name;
            itemFilter = new ItemFilter();
        }

        public RandomContentDetail DoRandom()
        {
            List<RandomContentDetail> sortedList = randoms.ToList().FindAll(content =>
            {
                Item item = UserInventory.Ins.FindById<Item>(content.contentId);
                return item && !item.Locked;
            });

            if(sortedList.Count == 0) return null;

            sortedList.SortAsc(_ => _.weight);

            int sum = sortedList.Sum(_ => _.weight);
            float ran = UnityEngine.Random.Range(0f, sum);
            float k = 0;
            foreach(RandomContentDetail randomContent in sortedList)
            {
                k += randomContent.weight;

                if(ran < k)
                {
                    if(randomContent.type == RandomContentType.Item)
                        return randomContent;

                    RandomDetail randomDetail = ItemAsset.Ins.FindRandomDetail(randomContent.contentId);
                    return randomDetail.DoRandom();
                }
            }

            return sortedList[0];
        }
    }

    [Serializable]
    public class ItemFilter : ISerializationCallbackReceiver
    {
        public List<StatFilter> statFilters = new List<StatFilter>();
        public List<EnumFilter> enumFilters = new List<EnumFilter>();
        public string searchName = "";

        [NonSerialized] public Dictionary<string, StatFilter> statFilterMap = new Dictionary<string, StatFilter>();
        [NonSerialized] public Dictionary<int, EnumFilter> enumFilterMap = new Dictionary<int, EnumFilter>();

        public int itemDefininitionId = -1;
        public bool show = false;

        [NonSerialized] public List<ItemDetail> filteredItems = new List<ItemDetail>();

        public List<EnumFilter> EnumFilters => enumFilters.ToList();
        public List<StatFilter> StatFilters => statFilters.ToList();

        public ItemFilter() { }

        public ItemFilter(int itemDefininitionId)
        {
            this.itemDefininitionId = itemDefininitionId;
        }

        // stat filter
        public void AddStatFilter(StatFilter statFilter)
        {
            statFilters.Add(statFilter);
            statFilterMap[statFilter.name] = statFilter;
        }

        public void RemoveStatFilter(string name)
        {
            if(statFilterMap.TryGetValue(name, out var statFilter))
            {
                statFilterMap.Remove(name);
                statFilters.Remove(statFilter);
            }
        }

        public bool TryGetStatFilter(string name, out StatFilter statFilter) => statFilterMap.TryGetValue(name, out statFilter);

        public StatFilter GetStatFilter(string name) => statFilterMap[name];


        // enum filter
        public void AddEnumFilter(EnumFilter enumFilter)
        {
            enumFilters.Add(enumFilter);
            enumFilterMap[enumFilter.id] = enumFilter;
        }

        public void RemoveEnumFilter(int id)
        {
            if(enumFilterMap.TryGetValue(id, out var enumFilter))
            {
                enumFilterMap.Remove(id);
                enumFilters.Remove(enumFilter);
            }
        }

        public bool TryGetEnumFilter(int enumId, out EnumFilter enumValue) => enumFilterMap.TryGetValue(enumId, out enumValue);

        public EnumFilter GetEnumFilter(int enumId) => enumFilterMap[enumId];

        // serialize callback
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            foreach(var statFilter in statFilters) statFilterMap[statFilter.name] = statFilter;
            foreach(var enumFilter in enumFilters) enumFilterMap[enumFilter.id] = enumFilter;
        }
    }

    [Serializable]
    public class StatFilter
    {
        public string name;
        public CompareFunc type = CompareFunc.None;
        public StatValue value;

        public StatFilter(string name)
        {
            this.name = name;
        }

        public bool Satify(StatDetail stat)
        {
            switch(type)
            {
                case CompareFunc.LessThan: return stat.value < value;
                case CompareFunc.LessOrEqual: return stat.value <= value;
                case CompareFunc.Equal: return stat.value == value;
                case CompareFunc.GreaterOrEqual: return stat.value >= value;
                case CompareFunc.Greater: return stat.value > value;
                default: return true;
            }
        }
    }

    [Serializable]
    public class EnumFilter
    {
        public int id;
        public int definitionId;
        public string value;
        public MatchType matchType = MatchType.Or;

        public EnumFilter(EnumPropertyDefinition @enum)
        {
            id = @enum.id;
            definitionId = @enum.definitionId;
        }
    }

    public enum MatchType { Or, Must }
    public enum CompareFunc { None, LessThan, LessOrEqual, Equal, GreaterOrEqual, Greater, NotEqual }

    [Serializable]
    public class RandomContentDetail
    {
        public int itemDefinitionId = -1;
        public int contentId = -1;
        public RandomContentType type;
        public DistinctStat[] stats = new DistinctStat[0];
        public int weight = 100;
        public float probability;

        public ItemDetail Item => ItemAsset.Ins.Find(contentId);
    }

    public enum RandomContentType { Item, SubRandom }
}