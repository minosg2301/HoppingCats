using System.Collections.Generic;
using UnityEngine;
using moonNest;

public class ConsumedRewards
{
    public List<ConsumedReward> list = new List<ConsumedReward>();

    public bool Contains(RewardType type) => list.Find(reward => reward.Type == type) != null;
    public List<ConsumedReward> FindAll(RewardType type) => list.FindAll(reward => reward.Type == type);

    public void Clear() => list.Clear();

    public override string ToString()
    {
        string content = "";
        list.ForEach(reward => content += reward + "\n");
        return content;
    }
}

public abstract class ConsumedReward
{
    public RewardType Type { get; protected set; } = RewardType.None;
    public Sprite Icon { get; protected set; }
    public GameObject Prefab { get; protected set; }
    public int Amount { get; protected set; }
    public string Name { get; protected set; }
    public bool New { get; set; } = false;

    public ConsumedReward(string name, int amount)
    {
        Name = name;
        Amount = amount;
    }

    public ConsumedReward(string name, Sprite icon, int amount)
    {
        Name = name;
        Icon = icon;
        Amount = amount;
    }

    public ConsumedReward(string name, GameObject prefab, int amount)
    {
        Name = name;
        Prefab = prefab;
        Amount = amount;
    }
}

public class StatConsumedReward : ConsumedReward
{
    public int StatId { get; private set; }
    public int Value { get; private set; }

    public StatConsumedReward(int statId, int value) : base("", 0)
    {
        Type = RewardType.Stat;
        StatId = statId;
        Value = value;
    }

    public override string ToString()
    {
        return $"User Stat: {UserPropertyAsset.Ins.FindStat(StatId).name} - {Value}";
    }
}

public class ItemConsumedReward : ConsumedReward
{
    public Item Item { get; private set; }

    public ItemConsumedReward(Item item, int amount) : base(item.Detail.name, item.Detail.icon, amount)
    {
        Type = RewardType.Item;
        Prefab = item.Detail.UIPrefab;
        Item = item;
    }

    public override string ToString()
    {
        return $"Item: {Item.Detail.name} - {Amount}";
    }
}

public class CurrencyConsumedReward : ConsumedReward
{
    public Currency Currency { get; private set; }

    public CurrencyConsumedReward(Currency currency, int fund) : base(currency.Name, currency.Icon, fund)
    {
        Type = RewardType.Currency;
        Currency = currency;
    }

    public CurrencyConsumedReward(Sprite icon, Currency currency, int fund) : base(currency.Name, icon, fund)
    {
        Type = RewardType.Currency;
        Currency = currency;
    }

    public override string ToString()
    {
        return $"Currency: {Currency.Name}\t{Amount}";
    }
}

public class ChestKeyConsumedReward : ConsumedReward
{
    public ChestDetail Chest { get; private set; }

    public ChestKeyConsumedReward(Sprite icon, ChestDetail chest, int amount) : base(chest.displayName, icon, amount)
    {
        Type = RewardType.Chest;
        Chest = chest;
    }

    public override string ToString()
    {
        return $"Chest Key: {Chest.name}\t{Amount}";
    }
}