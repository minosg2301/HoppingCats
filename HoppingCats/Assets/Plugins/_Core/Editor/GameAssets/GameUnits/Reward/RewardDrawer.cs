using UnityEngine;
using moonNest;

public class RewardDrawer
{
    private readonly ListCardDrawer<RewardObject> rewardCardDrawer;

    public bool DrawOnce { get; set; } = false;
    public bool DrawHeader { get; set; } = true;
    public bool CreateFirstReward { get; set; } = true;
    public RewardDetail Reward { get; private set; }

    public int MaxWidth { set { rewardCardDrawer.MaxWidth = value; } }

    readonly string name = "";

    public RewardDrawer(string name = "")
    {
        this.name = name;

        rewardCardDrawer = new ListCardDrawer<RewardObject>();
        rewardCardDrawer.onDrawElement = DrawReward;
        rewardCardDrawer.onDrawEditElement = DrawEditReward;
        rewardCardDrawer.elementCreator = () => new RewardObject { type = RewardType.Currency };
        rewardCardDrawer.onElementAdded = ele =>
        {
            switch(ele.type)
            {
                case RewardType.Currency: ele.currency = new CurrencyReward() { contentId = GameDefinitionAsset.Ins.currencies[0].id }; break;
                case RewardType.Item: ele.item = new ItemReward(); break;
                case RewardType.Chest: ele.chest = new ChestReward(); break;
                case RewardType.Stat: ele.stat = new StatReward(); break;
            }
        };
        rewardCardDrawer.EditBeforeAdd = true;
        rewardCardDrawer.CardHeight = 130;
    }

    public bool DrawEditReward(RewardObject ele)
    {
        ele.type = Draw.EnumField(TextPack.Select, ele.type);
        return true;
    }

    public void DrawReward(RewardObject rewardObject)
    {
        switch(rewardObject.type)
        {
            case RewardType.Currency: DrawCurrency(rewardObject.currency); break;
            case RewardType.Item: DrawItem(rewardObject.item); break;
            case RewardType.Chest: DrawChest(rewardObject.chest); break;
            case RewardType.Stat: DrawStat(rewardObject.stat); break;
        }
    }

    private static void DrawStat(StatReward stat)
    {
        Draw.LabelBoldBox(TextPack.UserStat, Color.magenta);

        Draw.Space();
        stat.contentId = Draw.IntPopupField(TextPack.Stat, stat.contentId, UserPropertyAsset.Ins.properties.stats, TextPack.name, TextPack.id);
        var statDef = UserPropertyAsset.Ins.properties.FindStat(stat.contentId);
        if(statDef)
        {
            stat.displayName = Draw.TextField(TextPack.Name, stat.displayName);
            stat.icon = Draw.SpriteField(TextPack.Icon, stat.icon);
            stat.value = Draw.StatValueField(TextPack.Value, stat.value);
            stat.applyType = Draw.EnumField(TextPack.Apply, stat.applyType);
        }
    }

    private static void DrawAmount(BaseReward reward)
    {
        Draw.BeginHorizontal();
        Draw.Label("Amount", Draw.kLabelWidth);
        reward.useRange = Draw.Toggle(reward.useRange);
        if(reward.useRange)
        {
            reward.amount.min = Draw.Float(reward.amount.min);
            reward.amount.max = Draw.Float(reward.amount.max);
            reward.amount.Validate();
        }
        else
        {
            reward.amount.min = reward.amount.max = Draw.Float(reward.amount.min);
        }
        Draw.EndHorizontal();
    }

    public static void DrawChest(ChestReward reward)
    {
        Draw.LabelBoldBox(TextPack.Chest, Color.red);

        Draw.Space();
        reward.icon = Draw.SpriteField(TextPack.Icon, reward.icon);
        reward.contentId = Draw.IntPopupField(TextPack.Chest, reward.contentId, ChestAsset.Ins.chests, TextPack.name, TextPack.id);
        reward.useKey = Draw.ToggleField(TextPack.UseKey, reward.useKey);
        DrawAmount(reward);
    }

    public static void DrawItem(ItemReward reward)
    {
        Draw.LabelBoldBox("Item", Color.yellow);

        Draw.Space();
        reward.icon = Draw.SpriteField(TextPack.Icon, reward.icon);
        reward.type = Draw.EnumField(TextPack.Type, reward.type);
        if(reward.type == ItemRewardType.Specific)
        {
            Draw.BeginChangeCheck();
            int lastDefId = reward.definitionId;
            reward.definitionId = Draw.IntPopupField(TextPack.ItemType, reward.definitionId, ItemAsset.Ins.definitions, TextPack.name, TextPack.id);
            reward.contentId = Draw.IntPopupField(TextPack.Item, reward.contentId, ItemAsset.Ins.FindByDefinition(reward.definitionId), TextPack.name, TextPack.id);
            if(Draw.EndChangeCheck() && reward.definitionId != -1)
            {
                ItemDefinition definition = ItemAsset.Ins.definitions.Find(def => def.id == reward.definitionId);
                int distinctCount = definition.stats.FindAll(s => s.distinct).Count;
                if(definition.storageType == StorageType.Several)
                {
                    if(lastDefId != reward.definitionId)
                    {
                        reward.stats = new DistinctStat[distinctCount];
                        for(int i = 0; i < distinctCount; i++)
                            reward.stats[i] = new DistinctStat(definition.stats[i].id, definition.stats[i].initValue);
                    }
                }
                else
                {
                    reward.stats = new DistinctStat[0];
                }
            }
        }
        else
        {
            reward.contentId = Draw.IntPopupField(TextPack.Random, reward.contentId, ItemAsset.Ins.itemRandoms, TextPack.name, TextPack.id);
        }

        var stats = reward.stats;
        for(int i = 0; i < stats.Length; i++)
        {
            if(stats[i].name?.Length == 0)
            {
                ItemDefinition definition = ItemAsset.Ins.definitions.Find(def => def.id == reward.definitionId);
                stats[i].name = definition.stats.Find(s => s.id == stats[i].id).name;
            }
            Draw.BeginHorizontal();
            Draw.Label(stats[i].name, Draw.kLabelWidth);
            stats[i].value = Draw.Int(stats[i].value);
            Draw.EndHorizontal();
        }

        DrawAmount(reward);

        reward.probability = Draw.FloatSliderField(TextPack.Probability, reward.probability, 0f, 1f);
    }

    public static void DrawCurrency(CurrencyReward reward)
    {
        Draw.LabelBoldBox(TextPack.Currency, Color.blue);

        Draw.Space();
        reward.icon = Draw.SpriteField(TextPack.Icon, reward.icon);
        reward.contentId = Draw.IntPopupField(TextPack.Currency, reward.contentId, GameDefinitionAsset.Ins.currencies, TextPack.name, TextPack.id);
        Draw.BeginHorizontal();
        Draw.Label(TextPack.Amount, Draw.kLabelWidth);
        reward.useRange = Draw.Toggle(reward.useRange);
        if(reward.useRange)
        {
            reward.amount.min = Draw.Float(reward.amount.min);
            reward.amount.max = Draw.Float(reward.amount.max);
            reward.amount.Validate();
        }
        else
        {
            reward.amount.min = reward.amount.max = Draw.Float(reward.amount.min);
        }
        Draw.EndHorizontal();
    }

    public void DoDraw(RewardDetail reward, ref bool show)
    {
        show = Draw.BeginFoldoutGroup(show, TextPack.Rewards, Color.cyan);
        if(show) _DoDraw(reward);
        Draw.EndFoldoutGroup();
    }

    public void DoDraw(RewardDetail reward)
    {
        Reward = reward;
        if(DrawHeader)
        {
            Draw.BeginVertical();
            Draw.LabelBoldBox(name.Length > 0 ? name : TextPack.Rewards, Color.cyan);
            _DoDraw(reward);
            Draw.EndVertical();
        }
        else
        {
            _DoDraw(reward);
        }
    }

    private void _DoDraw(RewardDetail reward)
    {
        if(CreateFirstReward && reward.rewards.Count == 0)
        {
            CurrencyReward currencyReward = new CurrencyReward() { contentId = GameDefinitionAsset.Ins.currencies[0].id };
            reward.rewards.Add(new RewardObject(currencyReward));
        }

        if(DrawOnce)
        {
            rewardCardDrawer.DrawAddButton = reward.rewards.Count == 0;
            rewardCardDrawer.DrawEditElement = reward.rewards.Count == 0;
        }

        rewardCardDrawer.DoDraw(reward.rewards);
    }
}

