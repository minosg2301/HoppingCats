using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using moonNest.remotedata;

namespace moonNest
{
    public class UserShop : RemotableUserData<FirestoreUserData>
    {
        public static UserShop Ins => LocalData.Get<UserShop>();

        [SerializeField] protected Dictionary<int, Shop> shops = new Dictionary<int, Shop>();

        public List<Shop> Shops => shops.Values.ToList();

        public event Action<Shop> OnShopRefresh = delegate { };
        public event Action<Shop, ShopItem> OnShopItemUpdated = delegate { };
        public event Action<Shop, ShopItem> OnShopItemRemoved = delegate { };

        protected internal override void OnLoad()
        {
            base.OnLoad();

            // remove if shop not exist in asset
            shops.Values.ToList().ForEach(shop =>
            {
                if (!ShopAsset.Ins.FindShop(shop.Id)) shops.Remove(shop.Id);
            });

            // create new shop if any
            foreach (ShopDetail shopDetail in ShopAsset.Ins.Shops)
            {
                if (shopDetail.activeOnLoad && !shopDetail.refreshConfig.enabled)
                {
                    // active non-refreshable shop
                    if (!shops.ContainsKey(shopDetail.id))
                    {
                        List<ShopItemDetail> shopItems = ShopAsset.Ins.FindByGroup(shopDetail.id);
                        Shop shop = shops.GetOrCreate(shopDetail.id, id => new Shop(shopDetail));
                        shop.shopItems = shopItems.ToMap(shopItemDetail => shopItemDetail.id, shopItemDetail => new ShopItem(shopItemDetail));
                    }
                }
            }

            // update shop onload
            shops.Values.ForEach(shop => shop.OnLoad());
        }

        /// <summary>
        /// Find shop items of shop
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<ShopItem> FindShopItems(int shopId)
        {
            Shop shop = Find(shopId);
            return shop != null ? shop.ShopItems : new List<ShopItem>();
        }

        /// <summary>
        /// Find shop by id
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public Shop Find(int shopId) => shops.TryGetValue(shopId, out var shop) ? shop : null;

        /// <summary>
        /// Called every time user login in game
        /// </summary>
        public void UpdateLogin()
        {
            var refreshableShops = ShopAsset.Ins.Shops.FindAll(s => s.activeOnLoad && s.refreshConfig.enabled);
            foreach (ShopDetail shopDetail in refreshableShops)
            {
                Shop shop = shops.GetOrCreate(shopDetail.id, id => new Shop(shopDetail));
                shop.DoRefreshAsync();
            }
        }

        internal void NotifyShopRefresh(Shop shop)
        {
            dirty = true;
            Notify(shop.Id.ToString());
            OnShopRefresh(shop);
        }

        internal void NotifyShopItemUpdated(Shop shop, ShopItem shopItem)
        {
            dirty = true;
            Notify(shopItem.DetailId.ToString());
            Notify(shop.Id.ToString());
            OnShopItemUpdated(shop, shopItem);
        }

        internal void NotifyShopItemRemoved(Shop shop, ShopItem shopItem)
        {
            dirty = true;
            Notify(shop.Id.ToString());
            OnShopItemRemoved(shop, shopItem);
        }

        protected override void OnRemoteDataSync(FirestoreUserData remoteData)
        {
            
        }

        protected override void OnRemoteDataCreated(FirestoreUserData remoteData)
        {

        }
    }
}