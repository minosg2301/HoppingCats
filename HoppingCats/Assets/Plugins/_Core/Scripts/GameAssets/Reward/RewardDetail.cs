using System;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
using Range = moonNest.Range;

[Serializable]
public class RewardDetail : BaseData
#if UNITY_EDITOR
, ISerializationCallbackReceiver
#endif
{
    public List<ChestReward> chests = new List<ChestReward>();
    public List<CurrencyReward> currencies = new List<CurrencyReward>();
    public List<ItemReward> items = new List<ItemReward>();
    public List<StatReward> stats = new List<StatReward>();

    private List<RewardObject> _rewards;
    public List<RewardObject> rewards
    {
        get
        {
            if (_rewards == null)
            {
                _rewards = new List<RewardObject>();
                chests.ForEach(chest => _rewards.Add(new RewardObject(chest)));
                currencies.ForEach(currency => _rewards.Add(new RewardObject(currency)));
                items.ForEach(item => _rewards.Add(new RewardObject(item)));
                stats.ForEach(stat => _rewards.Add(new RewardObject(stat)));
            }
            return _rewards;
        }
    }

    public RewardDetail() { }

    public RewardDetail(string name) : base(name) { }

    public RewardDetail(List<BaseReward> list)
    {
        foreach (var r in list)
        {
            if (r is ChestReward a) chests.Add(a);
            else if (r is CurrencyReward b) currencies.Add(b);
            else if (r is ItemReward c) items.Add(c);
            else if (r is StatReward d) stats.Add(d);
        }
    }

    internal RewardDetail(List<RewardObject> rewards)
    {
        _rewards = rewards;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Call after deserialize data from asset
    /// </summary>
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (_rewards == null) _rewards = new List<RewardObject>();
        _rewards.Clear();
        chests.ForEach(chest => _rewards.Add(new RewardObject(chest)));
        currencies.ForEach(currency => _rewards.Add(new RewardObject(currency)));
        items.ForEach(item => _rewards.Add(new RewardObject(item)));
        stats.ForEach(stat => _rewards.Add(new RewardObject(stat)));
    }

    /// <summary>
    /// Call before serialize data to asset
    /// </summary>
    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        chests.Clear();
        currencies.Clear();
        items.Clear();
        stats.Clear();

        if (_rewards != null)
        {
            foreach (var reward in _rewards)
            {
                switch (reward.type)
                {
                    case RewardType.Currency: currencies.Add(reward.currency); break;
                    case RewardType.Item: items.Add(reward.item); break;
                    case RewardType.Chest: chests.Add(reward.chest); break;
                    case RewardType.Stat: stats.Add(reward.stat); break;
                }
            }
        }
    }
#endif
}

public class RewardObject
{
    public RewardType type;
    public ChestReward chest;
    public CurrencyReward currency;
    public ItemReward item;
    public StatReward stat;

    public RewardObject() { }

    public RewardObject(StatReward stat)
    {
        type = RewardType.Stat;
        this.stat = stat;
    }

    public RewardObject(ChestReward chest)
    {
        type = RewardType.Chest;
        this.chest = chest;
    }

    public RewardObject(CurrencyReward currency)
    {
        type = RewardType.Currency;
        this.currency = currency;
    }

    public RewardObject(ItemReward item)
    {
        type = RewardType.Item;
        this.item = item;
    }

    public BaseReward OriginReward
    {
        get
        {
            switch (type)
            {
                case RewardType.Currency: return currency;
                case RewardType.Item: return item;
                case RewardType.Chest: return chest;
                case RewardType.Stat: return stat;
                default: return null;
            }
        }
    }

    public Sprite Icon
    {
        get
        {
            switch (type)
            {
                case RewardType.Currency: return currency.Icon;
                case RewardType.Item: return item.Icon;
                case RewardType.Chest: return chest.Icon;
                case RewardType.Stat: return stat.Icon;
                default: return null;
            }
        }
    }

    public GameObject Prefab
    {
        get
        {
            if (type == RewardType.Item) return item.Prefab;

            return null;
        }
    }

    public string AmountString
    {
        get
        {
            switch (type)
            {
                case RewardType.Currency: return currency.AmountString;
                case RewardType.Chest: return chest.AmountString;
                case RewardType.Item: return item.AmountString;
                case RewardType.Stat: return stat.value.ToString();
                default: return "";
            }
        }
    }

    /// <summary>
    /// Get amount of reward => 
    /// </summary>
    public int Amount
    {
        get
        {
            switch (type)
            {
                case RewardType.Currency: return currency.amount.RandomInt();
                case RewardType.Item: return item.amount.RandomInt();
                case RewardType.Chest: return chest.amount.RandomInt();
                case RewardType.Stat: return stat.value;
                default: return 0;
            }
        }
    }

    public string DisplayName
    {
        get
        {
            switch (type)
            {
                case RewardType.Currency: return currency.DisplayName;
                case RewardType.Item: return item.DisplayName;
                case RewardType.Chest: return chest.DisplayName;
                case RewardType.Stat: return stat.displayName;
                default: return "";
            }
        }
    }
}

public enum RewardType { None, Currency, Item, Chest, Stat }

[Serializable]
public class BaseReward : Cloneable
{
    public int contentId = -1;
    public Sprite icon;
    public Range amount = Range.By(1, 1);
    public bool useRange = false;

    public string AmountString
    {
        get
        {
            return useRange ? $"{amount.min}~{amount.max}" : amount.min.ToShortString(6, 0);
        }
    }
}

[Serializable]
public class StatReward : BaseReward
{
    public string displayName;
    public int value;
    public ApplyType applyType;

    [NonSerialized] private Sprite _icon = null;
    public Sprite Icon
    {
        get
        {
            if (icon) return icon;

            if (!_icon)
            {
                var statDef = UserPropertyAsset.Ins.FindStat(contentId);
                _icon = statDef ? statDef.displayIcon : null;
            }
            return _icon;
        }
    }
}

public enum ApplyType { Override, Addition }

public enum ItemRewardType { Specific, Random }

[Serializable]
public class ItemReward : BaseReward
{
    public ItemRewardType type = ItemRewardType.Specific;
    public int definitionId = -1;
    public DistinctStat[] stats = new DistinctStat[0];
    public float probability = 1f;

    [NonSerialized] private Sprite _icon = null;
    public Sprite Icon
    {
        get
        {
            if (icon) return icon;

            if (!_icon)
            {
                if (type == ItemRewardType.Specific)
                {
                    ItemDetail item = ItemAsset.Ins.Find(contentId);
                    _icon = item ? item.icon : null;
                }
                else //if (type == ItemRewardType.Random)
                {
                    RandomDetail randomSetting = ItemAsset.Ins.FindRandomDetail(contentId);
                    _icon = randomSetting ? randomSetting.icon : null;
                }
            }
            return _icon;
        }
    }

    [NonSerialized] private GameObject _prefab = null;
    public GameObject Prefab
    {
        get
        {
            if (type == ItemRewardType.Specific)
            {
                if (!_prefab)
                {
                    ItemDetail item = ItemAsset.Ins.Find(contentId);
                    _prefab = item ? item.UIPrefab : null;
                }
                return _prefab;
            }
            else
            {
                return null;
            }
        }
    }

    [NonSerialized] private string _displayName = null;
    public string DisplayName
    {
        get
        {
            if (type == ItemRewardType.Specific)
            {
                ItemDetail item = ItemAsset.Ins.Find(contentId);
                _displayName = item ? item.name : "";
            }
            else //if (type == ItemRewardType.Random)
            {
                RandomDetail randomSetting = ItemAsset.Ins.FindRandomDetail(contentId);
                _displayName = randomSetting ? randomSetting.displayName : "";
            }
            return _displayName;
        }
    }

    public bool RandomAbilityAdd() => UnityEngine.Random.Range(0f, 1f) < probability;
}

[Serializable]
public class CurrencyReward : BaseReward
{
    [NonSerialized] private Sprite _icon = null;
    public Sprite Icon
    {
        get
        {
            if (icon) return icon;

            if (!_icon && contentId != -1)
            {
                CurrencyDefinition currency = GameDefinitionAsset.Ins.FindCurrency(contentId);
                _icon = currency ? currency.bigIcon : null;
            }
            return _icon;
        }
    }

    [NonSerialized] private string _displayName = null;
    public string DisplayName
    {
        get
        {
            if (_displayName == null)
            {
                CurrencyDefinition currency = GameDefinitionAsset.Ins.FindCurrency(contentId);
                _displayName = currency ? currency.name : "";
            }
            return _displayName;
        }
    }
}

[Serializable]
public class ChestReward : BaseReward
{
    public bool useKey = false;

    [NonSerialized] private Sprite _icon = null;
    public Sprite Icon
    {
        get
        {
            if (icon) return icon;

            if (!_icon)
            {
                ChestDetail chest = ChestAsset.Ins.Find(contentId);
                _icon = chest ? (useKey ? chest.keyIcon : chest.icon) : null;
            }
            return _icon;
        }
    }

    [NonSerialized] private string _displayName = null;
    public string DisplayName
    {
        get
        {

            if (_displayName == null)
            {
                ChestDetail chest = ChestAsset.Ins.Find(contentId);
                _displayName = chest ? chest.displayName : "";
            }
            return _displayName;
        }
    }
}