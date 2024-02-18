using System;
using moonNest.ads;
using moonNest.tracking;

namespace moonNest
{
    public class ThirdPartyConfig : SingletonScriptObject<ThirdPartyConfig>
    {
        public SymbolDefined mobileDefine = new SymbolDefined();
        public SymbolDefined webGLDefine = new SymbolDefined();

        public TrackingConfig trackingConfig = new TrackingConfig();
        public AdsConfig adsConfig = new AdsConfig();
        public PushNotificationConfig pushNotiConfig = new PushNotificationConfig();

        public FirbaseConfig firbaseConfig = new FirbaseConfig();
    }

    [Serializable]
    public class FirbaseConfig
    {
        public string dynamicLinkDomain = "";
        public string dynamicLinkPrefix = "";
    }

    [Serializable]
    public class SymbolDefined
    {
        public bool useGoogleMobileAds = false;
        public bool useIronSrcAds = false;
        public bool useUnityAds = false;
        public bool useNativeAdmob = false;
        public bool useAppFlyer = false;
        public bool useAdjust = false;
        public bool useMaxSDK = false;
        public bool useFacebookSDK = false;
        public bool useFBInstant = false;
        public bool useFBInstantAds = false;
        public bool useLionSDK = false;
        public bool useAmazonSDK = false;
        public bool useGameAnalytic = false;
        public bool isUseAmazonMediation = false;
    }
}