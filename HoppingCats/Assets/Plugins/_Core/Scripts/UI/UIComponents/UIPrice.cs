using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public class UIPrice : MonoBehaviour
    {
        public CurrencyId currencyId = -1;
        public Image icon;
        public TextMeshProUGUI priceText;
        public Color color = Color.white;
        public Color cantPayColor = Color.red;
        public int digit = 6;
        public int precision = 0;

        public PriceConfig PriceConfig { get; private set; }
        public Currency Currency => UserCurrency.Get(currencyId);
        public bool UserCanPay => Currency.Value >= PriceConfig.value;

        private Localize _priceLoc;
        public Localize PriceLoc { get { if(!_priceLoc && priceText) _priceLoc = priceText.GetComponent<Localize>(); return _priceLoc; } }

        void Reset()
        {
            if(!priceText) priceText = GetComponentInChildren<TextMeshProUGUI>();
            if(!icon) icon = GetComponentInChildren<Image>();
        }

        void OnValidate()
        {
            CurrencyDefinition currency = GameDefinitionAsset.Ins.FindCurrency(currencyId);
            if(currency && icon) icon.sprite = currency.icon;
        }

        public void SetPrice(PriceConfig price)
        {
            PriceConfig = price;
            currencyId = price.currencyId;
            if(Currency != null && icon) icon.sprite = Currency.Icon;

            UpdateUI();
        }

        public void SetPriceValue(long priceValue)
        {
            if(PriceConfig == null) PriceConfig = new PriceConfig() { currencyId = currencyId };
            PriceConfig.value = priceValue;
            UpdateUI();
        }

        public void UpdateUI()
        {
            if(PriceConfig != null && Currency != null)
            {
                priceText.color = UserCanPay ? color : cantPayColor;
                bool free = PriceConfig.value == 0;
                if(free)
                {
                    if(PriceLoc)
                    {
                        PriceLoc.enabled = true;
                        PriceLoc.Term = "TXT_FREE";
                    }
                    else if(priceText)
                    {
                        priceText.text = "Free";
                    }
                }
                else
                {
                    if(PriceLoc) PriceLoc.enabled = false;
                    priceText.text = PriceConfig.value.ToShortString(digit, precision);
                }

                icon.gameObject.SetActive(!free);
            }
        }
    }
}