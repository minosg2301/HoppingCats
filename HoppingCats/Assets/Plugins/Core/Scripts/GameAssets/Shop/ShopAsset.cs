using System;
using System.Collections.Generic;

namespace moonNest
{
    public class ShopAsset : BaseGroupAsset<ShopAsset, ShopItemDetail, ShopDetail>
    {
        public List<ShopDefinition> shopDefinitions = new List<ShopDefinition>();
        public bool layerEnabled;
        public int layerGroupId = -1;

        public List<ShopItemDetail> Items => Datas;
        public List<ShopDetail> Shops => Groups;

        public ShopDefinition FindShopDefinition(int id) => shopDefinitions.Find(_ => _.id == id);

        public ShopDefinition FindShopDefinition(string name) => shopDefinitions.Find(_ => _.name == name);

        protected override int GetGroupId(ShopItemDetail data) => data.shopId;

        public void AddItem(ShopItemDetail shopItem) => Add(shopItem);

        public void RemoveItem(ShopItemDetail shopItem) => Remove(shopItem);

        public ShopItemDetail FindItem(int detailId) => Find(detailId);

        public ShopDetail FindShop(ShopDefinition shopDefinition) => Groups.Find(_ => _.definitionId == shopDefinition.id);

        public ShopDetail FindShop(int shopDetailId) => FindGroup(shopDetailId);

        public void AddShop(ShopDetail shop) => AddGroup(shop);

        public void RemoveShop(ShopDetail shopDetail)
        {
            RemoveGroup(shopDetail.id);
            shopDefinitions.Remove(shopDetail.Definition);
        }

        public void RemoveShop(ShopDefinition shopDefinition)
        {
            ShopDetail shopDetail = Groups.Find(shop => shop.definitionId == shopDefinition.id);
            RemoveGroup(shopDetail.id);
            shopDefinitions.Remove(shopDefinition);
        }

        #region layer methods
        public ShopLayer GetShopLayer(LayerDetail layer, ShopDetail shopDetail)
        {
            return layer && shopDetail.layerEnabled
                ? layer.shops.Find(shopLayer => shopLayer.shopId == shopDetail.id)
                : null;
        }

        public LayerDetail GetActiveLayer()
        {
            return layerEnabled ? LayerHelper.GetActiveLayer(layerGroupId) : null;
        }

        public LayerDetail GetLayerById(int layerId)
        {
            if(!layerEnabled || layerGroupId == -1) return null;
            return LayerAsset.Ins.FindLayer(layerGroupId, layerId);
        }
        #endregion
    }
}