using Doozy.Engine.UI;
using I2.Loc;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public class UIShopItem : BaseUIData<ShopItem>
    {
        public Image icon;
        public Transform prefabContainer;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI valueText;
        public bool alwaysShowValue = true;
        public UIPrice price;
        public UIButton button;
        public bool selectable;
        public GameObject outStockNode;

        public ShopItem ShopItem { get; set; } = null;
        public bool OutStock => ShopItem.Quantity == 0;
        public bool UserCanPay => price.UserCanPay;

        private Localize _nameloc;
        public Localize NameLoc { get { if(!_nameloc && nameText) _nameloc = nameText.GetComponent<Localize>(); return _nameloc; } }

        private GameObject uiPrefab;

        public Action<UIShopItem> onShopItemClicked;

        protected virtual void Start()
        {
            if(button) button.OnClick.OnTrigger.Event.AddListener(() => onShopItemClicked?.Invoke(this));
        }

        protected virtual void OnEnable() => Subscribe();

        protected virtual void OnDisable() => Unsubscribe();

        void Unsubscribe()
        {
            if(ShopItem != null)
            {
                UserCurrency.Ins?.Unsubscribe(ShopItem.Price.CurrencyName, OnDataUpdated);
                UserShop.Ins?.Unsubscribe(ShopItem.DetailId.ToString(), OnDataUpdated);
            }
        }

        void Subscribe()
        {
            if(ShopItem != null)
            {
                UserCurrency.Ins.Subscribe(ShopItem.Price.CurrencyName, OnDataUpdated, false);
                UserShop.Ins.Subscribe(ShopItem.DetailId.ToString(), OnDataUpdated);
            }
        }

        void OnDataUpdated(BaseUserData obj) => UpdateUI();

        public override void SetData(ShopItem shopItem)
        {
            if(ShopItem == shopItem) return;

            if(uiPrefab)
            {
                Destroy(uiPrefab);
                uiPrefab = null;
            }

            Unsubscribe();
            ShopItem = shopItem;
            Subscribe();
        }

        protected virtual void UpdateUI()
        {
            GameObject itemPrefab = ShopItem.ItemPrefab;
            if(itemPrefab && prefabContainer)
            {
                if(!uiPrefab)
                {
                    uiPrefab = Instantiate(itemPrefab, prefabContainer);
                    uiPrefab.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                }
            }
            else if(icon)
            {
                icon.sprite = ShopItem.Icon;
            }

            int contentValue = ShopItem.ContentValue;
            if(valueText)
            {
                valueText.gameObject.SetActive(alwaysShowValue || contentValue > 1);
                valueText.text = contentValue.ToShortString(6);
            }

            if(NameLoc) NameLoc.Term = ShopItem.Detail.name;
            else if(nameText) nameText.text = ShopItem.Detail.name;

            if(price) price.SetPrice(ShopItem.Price);
            if(outStockNode) outStockNode.gameObject.SetActive(OutStock);
            if(button) button.Interactable = (selectable || UserCanPay) && !OutStock;
        }
    }
}