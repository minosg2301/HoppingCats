using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace moonNest
{
    public class UIShop : MonoBehaviour, IObserver
    {
        public ShopId shopId = -1;
        public UICountDownTime refreshTime;

        public IReadOnlyList<UIShopItem> UIShopItems => ItemContainer.UIList.AsReadOnly();

        private ShopItemContainer _itemContainer;
        public ShopItemContainer ItemContainer { get { if(_itemContainer == null) _itemContainer = new ShopItemContainer(this); return _itemContainer; } }

        public Action<UIShopItem> onShopItemSelected;
        public Action<UIShopItem> onBuyShopItem;
        public Action<ShopItem, Item> onBuyItem;
        public Action<ShopItem, Currency> onBuyCurrency;
        public Action<ShopItem, ChestDetail> onBuyChest;

        protected virtual void Start()
        {
            if(shopId != -1)
            {
                UserShop.Ins.Subscribe(this, shopId.ToString());
            }
        }

        void OnValidate()
        {
            if(shopId != -1)
            {
                ShopDetail shop = ShopAsset.Ins.FindShop(shopId);
                gameObject.name = "UIShop - " + (shop != null ? shop.name : "");
            }
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            UpdateShopItems();
        }

        protected virtual List<ShopItem> SortShopItems(List<ShopItem> shopItems) => shopItems;

        protected virtual void UpdateShopItems()
        {
            Shop shop = UserShop.Ins.Find(shopId);
            List<ShopItem> shopItems = shop.ShopItems;
            shopItems = SortShopItems(shopItems);
            ItemContainer.replacePrefab = shop.Detail.replacePrefab;
            ItemContainer.SetList(transform, shopItems, ui => ui.onShopItemClicked = OnShopItemClicked);

            if(refreshTime && shop.RefreshEnabled)
            {
                refreshTime.StartWithDuration((float)shop.LastSeconds);
            }
        }

        protected virtual UIShopItem GetCustomPrefab(ShopItem shopItem) => null;

        protected virtual void OnSelectItem(UIShopItem uiShopItem) { }

        /// <summary>
        /// Invoke when a shop item is clicked
        /// </summary>
        /// <param name="uiShopItem"></param>
        protected virtual void OnShopItemClicked(UIShopItem uiShopItem)
        {
            ShopItem shopItem = uiShopItem.ShopItem;
            if(uiShopItem.UserCanPay && !uiShopItem.OutStock)
            {
                PriceConfig price = shopItem.Detail.price;

                // update currency by price
                if(price.value > 0) UserCurrency.Get(price.currencyId).AddValue(-price.value);

                shopItem.AddQuantity(-1);
                switch(shopItem.Detail.type)
                {
                    case ShopItemType.Chest: ApplyBuyChest(shopItem); break;
                    case ShopItemType.Currency: ApplyBuyCurrency(shopItem); break;
                    case ShopItemType.Random:
                    case ShopItemType.Item: ApplyBuyItem(shopItem); break;
                }

                onBuyShopItem?.Invoke(uiShopItem);
            }
            else
            {
                onShopItemSelected?.Invoke(uiShopItem);
                OnSelectItem(uiShopItem);
            }
        }

        protected virtual void ApplyBuyItem(ShopItem shopItem)
        {
            Item item = UserInventory.FindOrCreate<Item>(shopItem.ContentId);
            if(item)
            {
                item.AddAmount(shopItem.ContentValue);
                onBuyItem?.Invoke(shopItem, item);
            }
        }

        protected virtual void ApplyBuyCurrency(ShopItem shopItem)
        {
            Currency currency = UserCurrency.Get(shopItem.ContentId);
            currency.AddValue(shopItem.ContentValue);
            onBuyCurrency?.Invoke(shopItem, currency);
        }

        protected virtual void ApplyBuyChest(ShopItem shopItem)
        {
            ChestDetail chest = ChestAsset.Ins.Find(shopItem.ContentId);
            RewardConsumer.OpenChest(chest, shopItem.ContentValue);
            onBuyChest?.Invoke(shopItem, chest);
        }

        public class ShopItemContainer : UIListContainer<ShopItem, UIShopItem>
        {
            readonly UIShop shop;

            public ShopItemContainer(UIShop shop) { this.shop = shop; }

            protected override UIShopItem GetPrefab(ShopItem element, int index)
            {
                var uiShopItem = shop.GetCustomPrefab(element);
                if(uiShopItem) return uiShopItem;
                else if(element.Detail.prefab) return element.Detail.prefab;
                else return base.GetPrefab(element, index);
            }
        }
    }
}