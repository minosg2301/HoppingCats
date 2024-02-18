using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class ShopItemDetail : BaseData
    {
        public int shopId;
        public Sprite icon;
        public int filteredId = -1;
        public int contentId = -1;
        public int contentValue = 1;
        public int quantity = -1;
        public int slot = -1;
        public UIShopItem prefab;

        public ShopItemKind kind = ShopItemKind.Many;
        public ShopItemType type;

        public PriceConfig price = new PriceConfig();

        public string trackingId = "";

        public ShopItemDetail(string name) : base(name) { }
    }

    public enum ShopItemKind { One, Many }

    public enum ShopItemType { All, Currency, Item, Chest, Random }
}