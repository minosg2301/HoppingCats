namespace moonNest.ads
{
    public enum AdsNetworkID : byte
    {
        APPLOVIN,
        UNITY,
        IRONSRC,
        ADMOB,
        FB_INSTANT,
        AMAZON
    }

    public enum AdsType : byte
    {
        BANNER,
        INTERSTITIAL,
        REWARDED,
        NATIVE,
        RWD_INTER,
        APP_OPEN
    }

    public enum NativeAdsType : byte
    {
        SMALL, MEDIUM
    }
}