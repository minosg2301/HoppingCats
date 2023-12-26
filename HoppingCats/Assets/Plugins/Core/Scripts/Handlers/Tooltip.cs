using System;
using UnityEngine;

namespace moonNest
{
    public class Tooltip
    {
        public delegate void RequestChestTooltip(Transform target, ChestContent chestContent);
        public static RequestChestTooltip requestChestTooltip;

        public delegate void RequestItemTooltip(Transform target, ItemDetail itemDetail);
        public static RequestItemTooltip requestItemTooltip;

        public delegate void RequestRandomItemTooltip(Transform target, RandomDetail itemDetail);
        public static RequestRandomItemTooltip requestRandomItemTooltip;

        public delegate void RequestCurrencyTooltip(Transform target, CurrencyDefinition currency);
        public static RequestCurrencyTooltip requestCurrencyTooltip;

        public delegate void RequestStatTooltip(Transform target, StatDetail statDetail);
        public static RequestStatTooltip requestStatTooltip;

        public static void ShowReward(UIReward uiReward)
        {
            switch(uiReward.Reward.type)
            {
                case RewardType.Chest:
                {
                    var chestId = uiReward.Reward.chest.contentId;
                    ChestDetail chest = ChestAsset.Ins.Find(chestId);
                    if(!chest)
                    {
                        Debug.LogFormat("Try Request show chest tooltip but can not find chest detail id {0}", chestId);
                        return;
                    }
                    ShowChestTooltip(uiReward.infoButton.transform, chest);
                }
                break;
                case RewardType.Item:
                {
                    if(uiReward.Reward.item.type == ItemRewardType.Specific)
                    {
                        var itemId = uiReward.Reward.item.contentId;
                        var itemDetail = ItemAsset.Ins.Find(itemId);
                        if(!itemDetail)
                        {
                            Debug.LogFormat("Try Request show item tooltip but can not find item detail id {0}", itemId);
                            return;
                        }
                        ShowItemTooltip(uiReward.infoButton.transform, itemDetail);
                    }
                    else
                    {
                        var randomId = uiReward.Reward.item.contentId;
                        var randomDetail = ItemAsset.Ins.FindRandomDetail(randomId);
                        if(!randomDetail)
                        {
                            Debug.LogFormat("Try Request show random item tooltip but can not find random detail id {0}", randomId);
                            return;
                        }
                        ShowRandomItemTooltip(uiReward.infoButton.transform, randomDetail);
                    }
                }
                break;
                case RewardType.Currency:
                {
                    var currencyId = uiReward.Reward.currency.contentId;
                    var currency = GameDefinitionAsset.Ins.FindCurrency(currencyId);
                    if(!currency)
                    {
                        Debug.LogFormat("Try Request show currency tooltip but can not find currency id {0}", currencyId);
                        return;
                    }
                    ShowCurrencyTooltip(uiReward.infoButton.transform, currency);
                }
                break;
            }
        }

        private static void ShowCurrencyTooltip(Transform target, CurrencyDefinition currency)
        {
            if(!currency)
                throw new NullReferenceException("ShowChestTooltip null currency");

            requestCurrencyTooltip?.Invoke(target, currency);
        }

        public static void ShowRandomItemTooltip(Transform target, RandomDetail randomDetail)
        {
            if(!randomDetail)
                throw new NullReferenceException("ShowChestTooltip null random Detail");

            requestRandomItemTooltip?.Invoke(target, randomDetail);
        }

        public static void ShowItemTooltip(Transform target, ItemDetail itemDetail)
        {
            if(!itemDetail)
                throw new NullReferenceException("ShowChestTooltip null item detail");

            requestItemTooltip?.Invoke(target, itemDetail);
        }

        public static void ShowChestTooltip(Transform target, ChestDetail chest)
        {
            if(!chest)
                throw new NullReferenceException("ShowChestTooltip null chest detail");

            ChestContent chestContent = ChestAsset.Ins.GetActualChestContent(chest);
            requestChestTooltip?.Invoke(target, chestContent);
        }
    }
}