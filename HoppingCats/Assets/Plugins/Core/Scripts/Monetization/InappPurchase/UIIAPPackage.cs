using Doozy.Engine.UI;
using I2.Loc;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace moonNest
{
    public class UIIAPPackage : BaseUIData<InStorePackage>, IObserver
    {
        public IAPPackageId packageId = -1;

        [Header("Basic Info")]
        public Image icon;
        public UIButton purchaseButton;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI oldPriceText;
        public UIRewardDetail rewards;

        [Header("Quantity")]
        public TextMeshProUGUI quantityText;
        public string quantityParamName = "count";

        [Header("Promotion")]
        public GameObject promotionNode;
        public Image promotionIcon;
        public UICountDownTime promotionTime;
        public TextMeshProUGUI promotionDescriptionText;

        [Header("Sale Off")]
        public GameObject saleOffNode;
        public TextMeshProUGUI saleOffText;

        [Header("Decorations")]
        public GameObject decorNode;
        public Image decorBackground;
        public TextMeshProUGUI decorText;

        [Header("Nodes")]
        public GameObject freeNode;
        public GameObject freeNotifyNode;
        public GameObject outStockNode;
        public GameObject x2Node;

        [Header("Behaviour")]
        public bool hideWhenUnavailable = true;
        public bool hideWhenOutStock = false;

        public InStorePackage InStorePackage { get; private set; }
        public InStorePackageGroup Group { get; private set; }

        private TickInterval _tickInterval;
        public TickInterval TickInterval { get { if (!_tickInterval) _tickInterval = gameObject.AddComponent<TickInterval>(); return _tickInterval; } }

        private Localize _titleLoc;
        public Localize TitleLoc { get { if (!_titleLoc && titleText) _titleLoc = titleText.GetComponent<Localize>(); return _titleLoc; } }

        private Localize _descLoc;
        public Localize DescLoc { get { if (!_descLoc && descriptionText) _descLoc = descriptionText.GetComponent<Localize>(); return _descLoc; } }

        private Localize _promoDescLoc;
        public Localize PromoDescLoc { get { if (!_promoDescLoc && promotionDescriptionText) _promoDescLoc = descriptionText.GetComponent<Localize>(); return _descLoc; } }

        private LocalizeParams _quantityParam;
        public LocalizeParams QuantityParam { get { if (!_quantityParam && quantityText) _quantityParam = quantityText.GetComponent<LocalizeParams>(); return _quantityParam; } }

        private Localize _decorLoc;
        public Localize DecorLoc { get { if (!_decorLoc && decorText) _decorLoc = decorText.GetComponent<Localize>(); return _decorLoc; } }

        public Action<UIIAPPackage> onPerformPurchase = delegate { };

        #region unity methods
#if UNITY_EDITOR
        void OnValidate()
        {
            if (Selection.activeGameObject == null || PrefabUtility.GetNearestPrefabInstanceRoot(Selection.activeGameObject) == null) return;

            if (packageId != -1)
            {
                IAPPackage iapPackage = IAPPackageAsset.Ins.Find(packageId);
                gameObject.name = "UIIAPPackage - " + iapPackage.productId;
            }
            else
            {
                gameObject.name = "UIIAPPackage";
            }
        }
#endif
        void Start()
        {
            if (purchaseButton) purchaseButton.OnClick.OnTrigger.Event.AddListener(OnPurchaseClicked);
            if (packageId != -1) SetData(UserStore.Ins.Find(packageId));
        }

        void OnEnable()
        {
            if (IAPManager.Ins.InitializationComplete)
            {
                if (packageId == -1) Subscribe();
            }
            else IAPManager.Ins.onInitialized.AddListener(OnIAPInit);
        }

        void OnDisable()
        {
            if (packageId == -1) UserStore.Ins.Unsubscribe(this);
        }

        void OnDestroy()
        {
            UserStore.Ins.Unsubscribe(this);
        }
        #endregion

        public override void SetData(InStorePackage package)
        {
            InStorePackage = package;
            Group = UserStore.Ins.FindGroup(package.GroupId);
            UpdatePromotion();
            if (IAPManager.Ins.InitializationComplete) Subscribe();
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            // update availability
            bool hide = (hideWhenUnavailable && !InStorePackage.Available) || (hideWhenOutStock && InStorePackage.OutStock);
            gameObject.SetActive(!hide);
            if (hide) return;

            // update ui
            UpdateUI();
        }

        protected virtual void UpdateUI()
        {
            if (InStorePackage.Detail.free)
            {
                UpdateUIFreePackage();
                return;
            }

            var inStorePackage = InStorePackage;
            var iapPackage = inStorePackage.Detail;

            var availableProduct = IAPManager.Ins.GetProduct(inStorePackage.AvailableProductId);
            var originProduct = IAPManager.Ins.GetProduct(InStorePackage.OriginProductId);
            bool inPromotion = inStorePackage.InPromotion;

            if (purchaseButton) purchaseButton.Interactable = availableProduct != null;

            // update iapPackage info
            if (icon) icon.sprite = iapPackage.icon;
            
            if (TitleLoc) TitleLoc.Term = inPromotion ? iapPackage.promotionTitle : iapPackage.displayName;
            else if(titleText) titleText.text = inPromotion ? iapPackage.promotionTitle : iapPackage.displayName;

            if (DescLoc) DescLoc.Term = inPromotion ? iapPackage.promotionDescription : iapPackage.description;
            else if (descriptionText) descriptionText.text = inPromotion ? iapPackage.promotionDescription : iapPackage.description;

            // update instore
            if (rewards) rewards.SetData(inStorePackage.RewardDetail);
            if (outStockNode) outStockNode.SetActive(inStorePackage.OutStock);
            if (QuantityParam) QuantityParam.SetParameterValue(quantityParamName, inStorePackage.Quantity.ToString());
            else if (quantityText) quantityText.text = inStorePackage.Quantity.ToString();

            // update product info
            if (priceText && availableProduct != null)
            {
                priceText.gameObject.SetActive(true);
                priceText.text = availableProduct.metadata.localizedPriceString;
            }
            if (oldPriceText && inPromotion && originProduct != null)
            {
                oldPriceText.gameObject.SetActive(true);
                oldPriceText.text = originProduct.metadata.localizedPriceString;
            }

            // show/hide game objects
            if (promotionNode) promotionNode.SetActive(inPromotion);
            if (promotionTime) promotionTime.gameObject.SetActive(inPromotion);
            if (promotionIcon) promotionIcon.gameObject.SetActive(inPromotion);
            if (promotionDescriptionText) promotionDescriptionText.gameObject.SetActive(inPromotion);
            if (x2Node) x2Node.SetActive(Group.Detail.x2FirstBuy && inStorePackage.purchaseCount == 0);
            if (freeNode) freeNode.SetActive(false);
            if (freeNotifyNode) freeNotifyNode.SetActive(false);

            // show/hide game objects
            if (decorNode) decorNode.SetActive(iapPackage.decorContent.Length > 0);
            if (decorBackground && iapPackage.decorBackground) decorBackground.sprite = iapPackage.decorBackground;
            if (DecorLoc) DecorLoc.Term = iapPackage.decorContent;
            else if (decorText) decorText.text = iapPackage.decorContent;

            // sale off
            if (saleOffNode)
            {
                saleOffNode.SetActive(iapPackage.saleOff > 0);
                saleOffText.gameObject.SetActive(iapPackage.saleOff > 0);
                saleOffText.text = iapPackage.saleOff + "%";
            }

            // update promotion
            if (inPromotion)
            {
                if (promotionTime) promotionTime.StartWithDuration((float)inStorePackage.PromotionLastSeconds);
                if (promotionIcon) promotionIcon.sprite = iapPackage.promotionIcon;
                if (PromoDescLoc) PromoDescLoc.Term = iapPackage.promotionDescription;
                else if (promotionDescriptionText) promotionDescriptionText.text = iapPackage.promotionDescription;
            }
        }

        protected virtual void UpdatePromotion()
        {
            if (InStorePackage.InPromotion)
            {
                TickInterval.enabled = true;
                TickInterval.onTick = () =>
                {
                    if (!InStorePackage.InPromotion)
                    {
                        TickInterval.enabled = false;
                        UpdateUI();
                    }
                };
            }
            else if (_tickInterval) _tickInterval.enabled = false;
        }

        protected virtual void OnPurchaseClicked()
        {
#if UNITY_WEBGL
            Debug.LogError("IAP not support on webGL");
#else
            if (InStorePackage.Detail.free)
            {
                // free package, consume without purchase processing
                IAPPackageProcess.ConsumePackage(InStorePackage);
            }
            else
            {
                IAPManager.Ins.DoPurchase(InStorePackage);
            }
#endif
            onPerformPurchase(this);
        }

        #region private methods
        void OnIAPInit()
        {
            IAPManager.Ins.onInitialized.RemoveListener(OnIAPInit);
            Subscribe();
        }

        void Subscribe()
        {
            if (InStorePackage == null) return;
            UserStore.Ins.Subscribe(this, InStorePackage.DetailId.ToString());
        }

        void UpdateUIFreePackage()
        {
            var inStorePackage = InStorePackage;
            var iapPackage = InStorePackage.Detail;

            // update iapPackage info
            if (icon) icon.sprite = iapPackage.icon;
            if (TitleLoc) TitleLoc.Term = iapPackage.displayName;
            if (DescLoc) DescLoc.Term = iapPackage.description;

            // update instore
            if (rewards) rewards.SetData(inStorePackage.RewardDetail);
            if (outStockNode) outStockNode.SetActive(inStorePackage.OutStock);
            if (QuantityParam) QuantityParam.SetParameterValue(quantityParamName, inStorePackage.Quantity.ToString());
            else if (quantityText) quantityText.text = inStorePackage.Quantity.ToString();

            // show/hide game objects
            if (freeNotifyNode) freeNotifyNode.SetActive(inStorePackage.Available);
            if (freeNode) freeNode.SetActive(true);
            if (decorNode) decorNode.SetActive(false);
            if (saleOffNode) saleOffNode.SetActive(false);
            if (priceText) priceText.gameObject.SetActive(false);
            if (oldPriceText) oldPriceText.gameObject.SetActive(false);
        }
        #endregion
    }

    [Serializable]
    public class OnPurchaseCompletedEvent : UnityEvent<Product> { }

    [Serializable]
    public class OnPurchaseFailedEvent : UnityEvent<Product, PurchaseFailureReason> { }
}