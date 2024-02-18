using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class Shop
    {
        [SerializeField] private int shopId;
        [SerializeField] internal SynchronizableTime refreshTime;
        [SerializeField] internal Dictionary<int, ShopItem> shopItems;
        [SerializeField] internal int layerId = -1;

        public int Id => shopId;
        public ShopType Type => Detail.type;
        public ShopDetail Detail => ShopAsset.Ins.FindShop(shopId);
        public List<ShopItem> ShopItems => shopItems.Values.ToList();

        public double LastSeconds => refreshTime.LocalTime.Subtract(DateTime.Now).TotalSeconds;
        public bool RefreshEnabled => Detail.refreshConfig.enabled;
        public bool CanRefresh => Detail.refreshConfig.enabled && refreshTime.LocalTime < DateTime.Now && !refreshing;

        /// <summary>
        /// keep variable to prevent serialized field
        /// </summary>
        private bool refreshing = false;
        public bool Refreshing => refreshing;

        public bool HaveFreeItem => ShopItems.Find(shopItem => shopItem.Quantity > 0 && shopItem.Detail.price.value == 0) != null;

        internal Shop(ShopDetail shopDetail)
        {
            shopId = shopDetail.id;
            refreshTime = new SynchronizableTime(shopId);
            shopItems = new Dictionary<int, ShopItem>();
        }

        internal void OnLoad()
        {
            // update shop item detail
            if (Detail.layerEnabled && layerId != -1)
            {
                // update with layer
                LayerDetail layer = ShopAsset.Ins.GetLayerById(layerId);
                ShopLayer shopLayer = ShopAsset.Ins.GetShopLayer(layer, Detail);
                shopItems.Values.ToList().ForEach(shopItem =>
                {
                    ShopItemDetail detail = shopLayer.shopItems.Find(s => s.id == shopItem.DetailId);
                    if (detail) shopItem.Detail = detail;
                    else shopItems.Remove(shopItem.DetailId);
                });
            }
            else
            {
                // skip if resfreshable
                if (!Detail.activeOnLoad || RefreshEnabled) return;

                // add new items to shop
                List<ShopItemDetail> shopItemsAsset = ShopAsset.Ins.FindByGroup(Detail.id);
                shopItemsAsset.ForEach(shopItemDetail =>
                {
                    if (!shopItems.Keys.Contains(shopItemDetail.id))
                    {
                        shopItems.Add(shopItemDetail.id, new ShopItem(shopItemDetail));
                    }
                });

                shopItems.Values.ToList().ForEach(shopItem =>
                {
                    ShopItemDetail detail = ShopAsset.Ins.Find(shopItem.DetailId);
                    if (detail) shopItem.Detail = detail;
                    else shopItems.Remove(shopItem.DetailId);
                });
            }

            shopItems.Values.ForEach(shopItem => shopItem.OnUpdated += OnShopItemUpdated);
        }

        void OnShopItemUpdated(ShopItem shopItem)
        {
            if (Detail.removeItemOutStock && shopItem.quantity == 0)
            {
                Remove(shopItem);
            }
            else
            {
                UserShop.Ins.NotifyShopItemUpdated(this, shopItem);
            }
        }

        internal void Add(ShopItem shopItem)
        {
            shopItem.OnUpdated += OnShopItemUpdated;
            shopItems[shopItem.DetailId] = shopItem;
        }

        internal void Remove(ShopItem shopItem)
        {
            shopItem.OnUpdated -= OnShopItemUpdated;
            shopItems.Remove(shopItem.DetailId);
            UserShop.Ins.NotifyShopItemRemoved(this, shopItem);
        }

        /// <summary>
        /// Refresh shop with checking time
        /// </summary>
        public async void DoRefreshAsync(bool forced = false)
        {
            if (refreshing) return;
            refreshing = true;
            if (forced)
            {
                await DoRefresh();
            }
            else
            {
                DateTime nextRefreshTime = await refreshTime.GetTime(UserData.UserId);
                if (nextRefreshTime <= DateTime.Now) await DoRefresh();
                else Notify();
            }
            refreshing = false;
        }

        /// <summary>
        /// Find shop item by id
        /// </summary>
        /// <param name="shopItemId"></param>
        /// <returns></returns>
        public ShopItem FindShopItem(int shopItemId) => shopItems.TryGetValue(shopItemId, out var shopItem) ? shopItem : null;

        /// <summary>
        /// Refresh shop with no time check
        /// </summary>
        async Task DoRefresh()
        {
            ShopDetail shopDetail = Detail;

            // reset
            shopItems.Clear();
            layerId = -1;
            await refreshTime.UpdateTimeBySecond(UserData.UserId, Detail.refreshConfig.period * 60);

            // get origin shop items
            List<ShopItemDetail> shopItemDetails = ShopAsset.Ins.FindByGroup(shopDetail.id).ToList();

            // override if shop enables layer
            LayerDetail activeLayer = ShopAsset.Ins.GetActiveLayer();
            if (activeLayer)
            {
                ShopLayer shopLayer = ShopAsset.Ins.GetShopLayer(activeLayer, shopDetail);
                if (shopLayer != null && shopLayer.shopItems.Count > 0)
                {
                    shopItemDetails = shopLayer.shopItems;
                    layerId = activeLayer.id;
                }
            }

            ShopRotateConfig rotateConfig = Detail.refreshConfig;
            // handle shop list type
            if (shopDetail.type == ShopType.List)
            {
                if (rotateConfig.maxSlot > 0)
                {
                    List<ShopItemDetail> oneList = ListExt.RemoveAll(shopItemDetails, s => s.kind == ShopItemKind.One);
                    List<ShopItemDetail> manyList = shopItemDetails;
                    List<ShopItem> items = oneList.Count == 0
                                ? new List<ShopItem>()
                                : new List<ShopItem> { new ShopItem(oneList.Random()) };

                    while (items.Count < rotateConfig.maxSlot && manyList.Count > 0)
                        items.Add(new ShopItem(manyList.PopRandom()));

                    items.ForEach(shopItem => Add(shopItem));
                }
                else
                {
                    ShopAsset.Ins.FindByGroup(shopId)
                        .Map(shopItemDetail => new ShopItem(shopItemDetail))
                        .ForEach(quest => Add(quest));
                }
            }

            // handle shop slot type
            else if (shopDetail.type == ShopType.Slot)
            {
                Dictionary<int, List<ShopItemDetail>> slotItemsMap = shopItemDetails.ToListMap(shopItem => shopItem.slot);
                List<ShopItem> items = new List<ShopItem>();
                foreach (var pair in slotItemsMap)
                {
                    ShopItemDetail itemDetail = pair.Value.Random();
                    items.Add(new ShopItem(itemDetail));
                }
                items.ForEach(shopItem => Add(shopItem));
            }

            UserShop.Ins.NotifyShopRefresh(this);
        }

        /// <summary>
        /// Short hand for notify
        /// </summary>
        void Notify() => UserShop.Ins.Notify(shopId.ToString());
    }
}