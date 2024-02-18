using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class ShopItem
    {
        [SerializeField] private int detailId = -1;
        [SerializeField] internal int quantity = 0;
        [SerializeField] internal int contentId = -1;

        internal event Action<ShopItem> OnUpdated = delegate { };

        public int DetailId => detailId;
        public int Quantity => quantity;
        public int ContentId => contentId != -1 ? contentId : Detail.contentId;
        public int ContentValue => Detail.contentValue;
        public PriceConfig Price => Detail.price;

        private ShopItemDetail _detail;
        public ShopItemDetail Detail
        {
            get { return _detail; }
            internal set { _detail = value; }
        }

        public Sprite Icon
        {
            get
            {
                ShopItemDetail detail = Detail;
                if (detail.icon) return detail.icon;
                switch (detail.type)
                {
                    case ShopItemType.Currency: return GameDefinitionAsset.Ins.FindCurrency(detail.contentId).bigIcon;
                    case ShopItemType.Chest: return ChestAsset.Ins.Find(detail.contentId).icon;
                    case ShopItemType.Item: return ItemAsset.Ins.Find(detail.contentId).icon;
                    case ShopItemType.Random: return contentId != -1 ? ItemAsset.Ins.Find(contentId).icon : null;
                    default: return null;
                }
            }
        }

        public GameObject ItemPrefab
        {
            get
            {
                ShopItemDetail detail = Detail;
                switch (detail.type)
                {
                    case ShopItemType.Item: return ItemAsset.Ins.Find(detail.contentId).UIPrefab;
                    case ShopItemType.Random: return contentId != -1 ? ItemAsset.Ins.Find(contentId).UIPrefab : null;
                    default: return null;
                }
            }
        }

        internal ShopItem(ShopItemDetail detail)
        {
            detailId = detail.id;
            quantity = detail.quantity;
            contentId = GetContentId(detail);
            Detail = detail;
        }

        /// <summary>
        /// Update quantity if any
        /// </summary>
        /// <param name="amount"></param>
        public void AddQuantity(int amount)
        {
            if (quantity != -1)
            {
                quantity = Mathf.Max(0, quantity + amount);
                OnUpdated(this);
            }
        }

        /// <summary>
        /// Cached content id if type is Random
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        private int GetContentId(ShopItemDetail detail)
        {
            if (detail.type != ShopItemType.Random) return -1;

            RandomDetail randomItem = ItemAsset.Ins.FindRandomDetail(detail.contentId);
            RandomContentDetail randomContent = randomItem.DoRandom();
            return randomContent != null ? randomContent.contentId : -1;
        }
    }
}