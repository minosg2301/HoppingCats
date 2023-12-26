using System;
using System.Collections.Generic;

namespace moonNest
{
    [Serializable]
    public class LayerDetail : BaseData
    {
        public int groupId;
        public int requireValue;

        public int onlineRewardLinkedLayer = -1;

        public List<OnlineRewardLayer> onlineRewards = new List<OnlineRewardLayer>();
        public List<ChestLayer> chests = new List<ChestLayer>();
        public List<QuestGroupLayer> questGroups = new List<QuestGroupLayer>();
        public List<ShopLayer> shops = new List<ShopLayer>();
        public List<IAPPackageGroupLayer> iapPackageGroups = new List<IAPPackageGroupLayer>();

        public int arenaLinkedLayer = -1;
        public List<BattlePassLevelLayer> battlePassLevels = new List<BattlePassLevelLayer>();

        public LayerDetail(string name) : base(name) { }
    }

    [Serializable]
    public class ChestLayer : Cloneable
    {
        public int chestId;
        public ChestContent content;

#if UNITY_EDITOR
        public string Name { get; set; }
#endif

        public ChestLayer(ChestDetail chest)
        {
            chestId = chest.id;
            content = chest.content.Clone();
        }
    }

    [Serializable]
    public class QuestGroupLayer : Cloneable
    {
        public string name;
        public int groupId;
        public List<QuestLayer> quests = new List<QuestLayer>();

        public QuestGroupLayer(QuestGroupDetail questGroup, List<QuestDetail> quests)
        {
            name = questGroup.name;
            groupId = questGroup.id;
            this.quests = quests.Map(q => new QuestLayer(q));
        }
    }

    [Serializable]
    public class QuestLayer : Cloneable
    {
        public string name;
        public int questId;
        public RewardDetail reward;

        public QuestLayer(QuestDetail quest)
        {
            name = quest.name;
            questId = quest.id;
            reward = quest.reward.Clone() as RewardDetail;
        }
    }

    [Serializable]
    public class ShopLayer : Cloneable
    {
        public string name;
        public int shopId;
        public List<ShopItemDetail> shopItems = new List<ShopItemDetail>();

        public ShopLayer(ShopDetail shop, List<ShopItemDetail> shopItems)
        {
            name = shop.name;
            shopId = shop.id;
            this.shopItems = shopItems.Map(item => item.Clone() as ShopItemDetail);
        }
    }

    [Serializable]
    public class IAPPackageGroupLayer : Cloneable
    {
        public string name;
        public int groupId;
        public int iapLinkedLayer = -1;
        public List<IAPPackageLayer> packages = new List<IAPPackageLayer>();

        public IAPPackageGroupLayer(IAPPackageGroup packageGroup, List<IAPPackage> packages)
        {
            name = packageGroup.name;
            groupId = packageGroup.id;
            this.packages = packages.Map(p => new IAPPackageLayer(p));
        }
    }

    [Serializable]
    public class BattlePassLevelLayer : Cloneable
    {
        public int level;
        public RewardDetail reward;
        public RewardDetail premiumReward;

        public BattlePassLevelLayer(BattlePassLevel bpLevel)
        {
            level = bpLevel.level;
            reward = bpLevel.reward.Clone() as RewardDetail;
            premiumReward = bpLevel.premiumReward.Clone() as RewardDetail;
        }
    }

    [Serializable]
    public class IAPPackageLayer : Cloneable
    {
        public string name;
        public int iapPackageId;
        public List<RewardDetail> rewards;

        public IAPPackageLayer(IAPPackage package)
        {
            name = package.name;
            iapPackageId = package.id;
            rewards = package.rewards.Map(r => r.Clone() as RewardDetail);
        }

        private IAPPackage _package;
        public IAPPackage IAPPackage { get { if (!_package) _package = IAPPackageAsset.Ins.Find(iapPackageId); return _package; } }
    }

    [Serializable]
    public class OnlineRewardLayer : Cloneable
    {
        public int onlineRewardId;
        public List<RewardDetail> rewards = new List<RewardDetail>();

#if UNITY_EDITOR
        public string Name { get; set; }
#endif

        public OnlineRewardLayer(OnlineRewardDetail onlineReward)
        {
            onlineRewardId = onlineReward.id;
            onlineReward.rewards.ForEach(reward => rewards.Add(reward.Clone() as RewardDetail));
        }
    }
}