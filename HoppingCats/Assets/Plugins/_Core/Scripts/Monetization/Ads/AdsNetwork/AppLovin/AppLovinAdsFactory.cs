using System;
using System.Collections.Generic;

namespace moonNest.ads
{
    internal partial class AppLovinAds
    {
        static readonly Dictionary<AdsType, Type> adsDisplayerCreators = new Dictionary<AdsType, Type>()
        {
#if USE_MAX_SDK
            { AdsType.BANNER, typeof(AppLovinBanner)},
            { AdsType.INTERSTITIAL, typeof(AppLovinInterstitial)},
            { AdsType.REWARDED, typeof(AppLovinRewardAds)}
#endif
        };

        internal override AdsDisplayer CreateAdsDisplayer(AdsType type, DisplayConfig displayConfig)
        {
            if (adsDisplayerCreators.TryGetValue(type, out var classType))
            {
                var displayer = Activator.CreateInstance(classType, this, displayConfig) as AdsDisplayer;
                if (!PendingInit) displayer.Init();
                return displayer;
            }
            throw new NullReferenceException("USE_MAX_SDK not defined. Can not find creators of " + type.ToString());
        }
    }
}