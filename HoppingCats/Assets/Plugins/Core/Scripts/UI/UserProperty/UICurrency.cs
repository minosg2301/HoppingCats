using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public class UICurrency : MonoBehaviour, IObserver
    {
        public CurrencyId currencyId;
        public Image icon;
        public TextMeshProUGUI valueText;
        public int digit = 8;
        public UICountDownTime growCountdown;

        public Currency Currency => UserCurrency.Get(currencyId);

        public bool CollectHandlerListening { get; internal set; } = false;

        void OnValidate()
        {
            CurrencyDefinition currency = GameDefinitionAsset.Ins.FindCurrency(currencyId);
            if (currency != null)
            {
                gameObject.name = "UICurrency - " + currency.name;
                if (icon) icon.sprite = currency.icon;
            }
        }

        protected virtual void OnEnable()
        {
            UpdateUI();
            Currency.Subscribe(this, true);
        }

        protected virtual void OnDisable()
        {
            Currency?.Unsubscribe(this);
        }

        protected virtual void UpdateUI()
        {
            var value = Currency.Value;
            if (icon) icon.sprite = Currency.Icon;
            if (valueText) valueText.text = value.ToShortString(digit, 0);

            if (growCountdown)
            {
                growCountdown.gameObject.SetActive(Currency.GrowOverTime && value < Currency.GrowMaxValue);
                growCountdown.StartWithDuration((float)Currency.LastSeconds);
            }
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            if (CollectingManager.Ins.AllowCollect && CollectHandlerListening && Currency.Modifier > 0) return;

            UpdateUI();
        }

        public virtual void ShowLastValue()
        {
            valueText.text = Currency.LastValue.ToShortString(digit, 0);
        }

        public virtual void ShowLastValueWithPercent(float percent)
        {
            valueText.text = Math.Max(0, Currency.LastValue + Currency.Modifier * percent).ToShortString(digit, 0);
        }
    }
}