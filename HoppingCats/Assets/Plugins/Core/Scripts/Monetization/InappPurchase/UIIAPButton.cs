using Doozy.Engine.UI;
using I2.Loc;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

namespace moonNest
{
    [RequireComponent(typeof(UIButton))]
    public class UIIAPButton : MonoBehaviour
    {
        [Serializable]
        public class OnPurchaseCompletedEvent : UnityEvent<Product> { };

        [Serializable]
        public class OnPurchaseFailedEvent : UnityEvent<Product, PurchaseFailureReason> { };

        public IAPPackageId packageId;

        [Space]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI priceText;

        [Space]
        [Header("Promotion")]
        public TextMeshProUGUI promotionPriceText;

        public InStorePackage InStorePackage { get; set; } = null;
        public string ProductId => InStorePackage.InPromotion ? InStorePackage.Detail.promotionProductId : InStorePackage.Detail.productId;

        private UIButton _button;
        public UIButton Button { get { if(_button == null) _button = GetComponent<UIButton>(); return _button; } }


        public Action<UIIAPButton> onPerformPurchase = delegate { };

        void Start()
        {
            if(Button) Button.OnClick.OnTrigger.Event.AddListener(OnPurchaseClicked);

            InStorePackage inStorePackage = UserStore.Ins.Find(packageId);
            if(inStorePackage != null)
            {
                SetData(inStorePackage);
            }
        }

        public void SetData(InStorePackage inStorePackage)
        {
            InStorePackage = inStorePackage;

            if(IAPManager.Ins.InitializationComplete)
            {
                UpdateUI();
            }
        }

        void OnEnable()
        {
            if(IAPManager.Ins.InitializationComplete) UpdateUI();
            else IAPManager.Ins.onInitialized.AddListener(OnIAPInit);
        }

        private void OnIAPInit()
        {
            IAPManager.Ins.onInitialized.RemoveListener(OnIAPInit);
            UpdateUI();
        }

        protected virtual void UpdateUI()
        {
            Button.Interactable = false;
            if(InStorePackage != null)
            {
                var product = IAPManager.Ins.GetProduct(InStorePackage.Detail.productId);
                var promotionProduct = IAPManager.Ins.GetProduct(InStorePackage.Detail.promotionProductId);
                bool onPromoted = InStorePackage.InPromotion && promotionProduct != null;
                Button.Interactable = product != null;

                var _p = onPromoted ? promotionProduct : product;
                if(_p != null && product != null)
                {
                    if(titleText)
                        titleText.GetComponent<Localize>().Term = onPromoted ? InStorePackage.Detail.promotionTitle : InStorePackage.Detail.name;

                    if(descriptionText)
                        descriptionText.GetComponent<Localize>().Term = onPromoted ? InStorePackage.Detail.promotionDescription : InStorePackage.Detail.description;

                    if(priceText) priceText.text = _p.metadata.localizedPriceString;

                    if(promotionPriceText)
                    {
                        promotionPriceText.gameObject.SetActive(onPromoted);
                        promotionPriceText.text = product.metadata.localizedPriceString;
                    }
                }
            }
        }

        protected virtual void OnPurchaseClicked()
        {
            IAPManager.Ins.DoPurchase(InStorePackage);

            onPerformPurchase(this);
        }
    }
}