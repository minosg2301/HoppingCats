using System;

namespace moonNest
{
    [Serializable]
    public class PriceConfig
    {
        public int currencyId = -1;
        public long value = 10;

        /// <summary>
        /// Shot cut for get currency definition
        /// </summary>
        [NonSerialized] private CurrencyDefinition _currency;
        private CurrencyDefinition Currency { get { if(!_currency) _currency = GameDefinitionAsset.Ins.FindCurrency(currencyId); return _currency; } }

        /// <summary>
        /// Get currency name
        /// </summary>
        public string CurrencyName => Currency.name;

        /// <summary>
        /// Operator of multiply price
        /// </summary>
        /// <param name="config"></param>
        /// <param name="multiply"></param>
        /// <returns></returns>
        public static PriceConfig operator *(PriceConfig config, float multiply)
        {
            PriceConfig newConfig = new PriceConfig() { currencyId = config.currencyId, value = config.value };
            newConfig.value = (long)Math.Round(config.value * multiply);
            return newConfig;
        }

        /// <summary>
        /// ToString method
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Currency.name} - {value}";
    }
}