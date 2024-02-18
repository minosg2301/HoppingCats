namespace moonNest.ads
{
    public class AdsValue
    {
        public AdsValue(string adUnit, string adsFormat, string adsSourceName,
            double valueMicros, string currencyCode, int precisionType)
        {
            AdUnit = adUnit;
            AdsFormat = adsFormat;
            AdsSourceName = adsSourceName;
            ValueMicros = valueMicros;
            CurrencyCode = currencyCode;
            PrecisionType = precisionType;
        }

        public string AdUnit { get; }
        public string AdsFormat { get; }
        public string AdsSourceName { get; }
        public double ValueMicros { get; }
        public string CurrencyCode { get; }
        public int PrecisionType { get; }
    }
}