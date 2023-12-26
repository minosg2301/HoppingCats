using System;
using UnityEngine;

namespace moonNest
{
    public enum UnitApplyType { Single, Bundle }

    public class RewardConsumer
    {
        private static readonly ConsumedRewards rewardResult = new ConsumedRewards();

        #region event
        public delegate void RewardConsumedEvent(ConsumedRewards rewardResults);
        public delegate void IAPPackageConsumedEvent(IAPPackage iapPackage, ConsumedRewards rewardResults);
        public delegate void ChestConsumeEvent(ChestDetail chestDetail, ConsumedRewards rewardResults, Action<ConsumedRewards> onDone);

        public static RewardConsumedEvent onRewardConsumed = delegate { };
        public static IAPPackageConsumedEvent onIAPPackageConsumed = delegate { };
        public static ChestConsumeEvent onChestOpened = delegate { };
        #endregion

        private static bool currencyChanged = false;
        private static bool itemChanged = false;

        #region public methods
        public static ConsumedRewards ConsumeIAPPackage(InStorePackage inStorePackage, int multiply = 1)
        {
            Reset();

            var finalReward = inStorePackage.RewardDetail;
            finalReward.chests.ForEach(chest => Apply(chest, multiply));
            finalReward.currencies.ForEach(currency => Apply(currency, multiply));
            finalReward.items.ForEach(item => Apply(item, multiply));
            finalReward.stats.ForEach(stat => Apply(stat, multiply));

            if(currencyChanged) UserCurrency.Ins.Save();
            if(itemChanged) UserInventory.Ins.Save();

            if(GlobalConfig.Ins.VerboseLog) LogResult("Consume IAP Package");

            onIAPPackageConsumed(inStorePackage.Detail, rewardResult);

            return rewardResult;
        }

        public static ConsumedRewards ConsumeReward(RewardDetail reward, int multiply = 1)
        {
            if(multiply <= 0) return null;

            Reset();

            if(reward.chests.Count == 1 && reward.rewards.Count == reward.chests.Count && !reward.chests[0].useKey)
            {
                var chestDetail = ChestAsset.Ins.Find(reward.chests[0].contentId);
                if(chestDetail != null)
                {
                    OpenChest(chestDetail, multiply);
                }
                return rewardResult;
            }
            else
            {
                reward.chests.ForEach(chest => Apply(chest, multiply));
                reward.currencies.ForEach(currency => Apply(currency, multiply));
                reward.items.ForEach(item => Apply(item, multiply));
                reward.stats.ForEach(stat => Apply(stat, multiply));
            }

            if(currencyChanged) UserCurrency.Ins.Save();
            if(itemChanged) UserInventory.Ins.Save();

            if(GlobalConfig.Ins.VerboseLog) LogResult("Consume Reward");

            onRewardConsumed(rewardResult);

            return rewardResult;
        }

        public static ConsumedRewards OpenChest(ChestDetail chest, Action<ConsumedRewards> onCompleted)
        {
            return OpenChest(chest, 1, onCompleted);
        }

        public static ConsumedRewards OpenChest(ChestDetail chest, int multiply = 1, Action<ConsumedRewards> onCompleted = null)
        {
            Reset();

            CoreHandler.DoAction(ActionDefinition.OpenChestAction(chest.id));

            ChestContent chestContent = ChestAsset.Ins.GetActualChestContent(chest);
            chestContent.currencies.ForEach(currency => Apply(currency, multiply));
            chestContent.items.ForEach(item => Apply(item, multiply));

            if(currencyChanged) UserCurrency.Ins.Save();
            if(itemChanged) UserInventory.Ins.Save();

            if(GlobalConfig.Ins.VerboseLog) LogResult("Open Chest");

            if(onChestOpened != null) onChestOpened.Invoke(chest, rewardResult, onCompleted);
            else onCompleted(rewardResult);

            return rewardResult;
        }

        /// <summary>
        /// Helper method to consume Consumed Rewards
        /// Used for x2 reward ads
        /// </summary>
        /// <param name="consumedRewards"></param>
        public static void ConsumeReward(ConsumedRewards consumedRewards)
        {
            foreach(var reward in consumedRewards.list)
            {
                switch(reward.Type)
                {
                    case RewardType.Currency:
                    if(reward is CurrencyConsumedReward currency)
                    {
                        currency.Currency.AddValue(reward.Amount);
                    }
                    break;
                    case RewardType.Item:
                    if(reward is ItemConsumedReward item)
                    {
                        item.Item.AddAmount(reward.Amount);
                    }
                    break;
                }
            }
        }
        #endregion

        #region private methods
        private static void LogResult(string scope)
        {
            Debug.Log(scope);
            string[] contents = rewardResult.ToString().Split('\n');
            contents.ForEach(content => Debug.Log($"\t{content}"));
        }

        private static void Reset()
        {
            rewardResult.Clear();
            currencyChanged = itemChanged = false;
        }

        private static void Apply(ChestReward chestReward, int multiply = 1)
        {
            var chest = ChestAsset.Ins.Find(chestReward.contentId);
            if(chestReward.useKey)
            {
                ChestKey chestKey = UserInventory.FindOrCreate(chest.id);
                int amount = chestReward.amount.RandomInt() * multiply;
                chestKey.AddAmount(amount, UnitApplyType.Bundle);
                rewardResult.list.Add(new ChestKeyConsumedReward(chestReward.Icon, chest, amount));
            }
            else
            {
                ChestContent chestContent = ChestAsset.Ins.GetActualChestContent(chest);
                chestContent.currencies.ForEach(currency => Apply(currency, multiply));
                chestContent.items.ForEach(item => Apply(item, multiply));
            }
        }

        private static void Apply(ItemReward itemReward, int multiply = 1)
        {
            bool canAdd = itemReward.RandomAbilityAdd();
            if(canAdd)
            {
                DistinctStat[] stats;
                int itemDetailId;
                int amount = itemReward.amount.RandomInt() * multiply;
                if(itemReward.type == ItemRewardType.Random)
                {
                    RandomDetail randomDetail = ItemAsset.Ins.FindRandomDetail(itemReward.contentId);
                    RandomContentDetail contentDetail = randomDetail.DoRandom();
                    itemDetailId = contentDetail.contentId;
                    stats = contentDetail.stats;
                }
                else
                {
                    itemDetailId = itemReward.contentId;
                    stats = itemReward.stats;
                }

                ItemDetail itemDetail = ItemAsset.Ins.Find(itemDetailId);
                if(!itemDetail)
                {
                    Debug.LogError($"Can not apply Random Item Reward with Item Id {itemDetailId}");
                    return;
                }

                Item item = UserInventory.FindOrCreate<Item>(itemDetail, stats);
                item.AddAmount(amount);
                rewardResult.list.Add(new ItemConsumedReward(item, amount));
            }
        }

        private static void Apply(CurrencyReward reward, int multiply = 1)
        {
            Currency currency = UserCurrency.Get(reward.contentId);
            if(currency == null)
            {
                Debug.LogError($"Can not apply Currency Reward with id {reward.contentId}");
                return;
            }
            int fund = reward.amount.RandomInt() * multiply;
            currency.AddValue(fund, false, UnitApplyType.Bundle);
            rewardResult.list.Add(new CurrencyConsumedReward(reward.Icon, currency, fund));
        }

        private static void Apply(StatReward reward, int multiply = 1)
        {
            int statId = reward.contentId;
            int value = reward.value * multiply;
            var success = reward.applyType == ApplyType.Override
                ? UserData.SetStat(statId, value)
                : UserData.AddStat(statId, value);

            if(success)
            {
                rewardResult.list.Add(new StatConsumedReward(statId, UserData.Stat(statId)));
            }
            else
            {
                Debug.LogError($"Can not apply Stat Reward with id {statId}");
            }
        }
        #endregion
    }
}