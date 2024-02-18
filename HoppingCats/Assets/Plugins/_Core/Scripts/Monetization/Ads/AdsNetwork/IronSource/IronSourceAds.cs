#if USE_IRONSRC_ADS
using UnityEngine;
using vgame.tracking;

#if USE_AMAZON_SDK
using AmazonAds;
#endif

namespace vgame.ads
{
    internal partial class IronSourceAds : AdsNetwork
    {
        public IronSourceAds(AdsNetworkConfig config) : base(config)
        {
            Ads.onApplicationPause += OnApplicationPause;
        }

        internal override void Init()
        {
#if IS_USE_AMAZON_MEDIATION && USE_AMAZON_SDK && !UNITY_EDITOR
            var amazonAdsNetworkConfig = Ads.GetAdsNetworkConfig(AdsNetworkID.AMAZON);
            Amazon.Initialize(amazonAdsNetworkConfig.PlatformConfig.sdkKey);
            Amazon.EnableTesting(Ads.DebugMode);
            Amazon.EnableLogging(Ads.DebugMode);
            Amazon.UseGeoLocation(true);
            Amazon.IsLocationEnabled();
            Amazon.SetMRAIDPolicy(Amazon.MRAIDPolicy.CUSTOM);
            Amazon.SetMRAIDSupportedVersions(new string[] { "1.0", "2.0", "3.0" });
            Amazon.SetAdNetworkInfo(new AdNetworkInfo(DTBAdNetwork.IRON_SOURCE));

            IronSource.Agent.setManualLoadRewardedVideo(true);
#endif

            IronSource.Agent.setMetaData("is_child_directed", "false");
            IronSource.Agent.shouldTrackNetworkState(true);
#if !UNITY_EDITOR && (UNITY_ANDROID)
            IronSource.Agent.setConsent(Ads.GDPRConsent);
#endif

            IronSource.Agent.init(PlatformConfig.sdkKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.BANNER);

//#if !USE_IRONSRC_720
            if (Ads.DebugMode)
            {
                IronSource.Agent.validateIntegration();
                IronSource.Agent.setAdaptersDebug(true);

            }
//#else
            //if (Ads.DebugMode)
            //{
            //    IronSource.Agent.validateIntegration();
            //    IronSource.Agent.setAdaptersDebug(true);

            //    AdQualitySdkInit adQualitySdkInit = new();
            //    ISAdQualityConfig adQualityConfig = new()
            //    {
            //        TestMode = true,
            //        AdQualityInitCallback = adQualitySdkInit
            //    };
            //    IronSourceAdQuality.Initialize(PlatformConfig.sdkKey, adQualityConfig);
            //}
            //else
            //{
            //    IronSourceAdQuality.Initialize(PlatformConfig.sdkKey);
            //}
//#endif

            IronSourceEvents.onImpressionDataReadyEvent += IronSourceEvents_onImpressionSuccessEvent;
        }

        private void IronSourceEvents_onImpressionSuccessEvent(IronSourceImpressionData obj)
        {
            string adNetwork = ToString();
            TrackingManager.OnAdsImpress("ironSource", adNetwork, obj.adNetwork, obj.instanceName, obj.adUnit, "USD", (double)(obj.revenue.HasValue ? obj.revenue : 0));

#if IS_USE_AMAZON_MEDIATION && USE_AMAZON_SDK && !UNITY_EDITOR
            if(obj.adUnit == "banner")
            {
                (Ads.GetAdsController(AdsType.BANNER) as BannerController).IronSourceBanner.LoadAPSBanner();
            }
#endif
        }

        void OnApplicationPause(bool pause)
        {
            IronSource.Agent.onApplicationPause(pause);
        }

        public override string ToString() { return "ironsrc"; }
    }

//#if USE_IRONSRC_720
//    public class AdQualitySdkInit : ISAdQualityInitCallback
//    {
//        public void adQualitySdkInitSuccess()
//        {
//            Debug.Log("unity: adQualitySdkInitSuccess");
//        }
//        public void adQualitySdkInitFailed(ISAdQualityInitError adQualitySdkInitError, string errorMessage)
//        {
//            Debug.Log("unity: adQualitySdkInitFailed " + adQualitySdkInitError + " message: " + errorMessage);
//        }
//    }
//#endif

}
#else
    namespace moonNest.ads
{
    internal partial class IronSourceAds : AdsNetwork
    {
        public IronSourceAds(AdsNetworkConfig config) : base(config) { }
        internal override void Init() { }
        public override string ToString() { return "ironsrc"; }
    }
}
#endif // USE_IRONSRC_ADS