using System;
using System.Collections.Generic;

namespace moonNest.ads
{
    internal partial class FBInstantAds
    {
        static readonly Dictionary<AdsType, Type> adsDisplayerCreators = new Dictionary<AdsType, Type>()
        {
#if USE_FB_INSTANT_ADS
            { AdsType.BANNER, typeof(FBInstantBanner)},
            { AdsType.INTERSTITIAL, typeof(FBInstantInterstitial)},
            { AdsType.REWARDED, typeof(FBInstantRewardAds)},
            { AdsType.RWD_INTER, typeof(FBInstantRewardInterAds)}
#endif
        };

        internal override AdsDisplayer CreateAdsDisplayer(AdsType type, DisplayConfig displayConfig)
        {
            if (adsDisplayerCreators.TryGetValue(type, out var classType))
            {
                var displayer = Activator.CreateInstance(classType, this, displayConfig) as AdsDisplayer;
                displayer.Init();
                return displayer;
            }
            throw new NullReferenceException("USE_FB_INSTANT_ADS not defined. Can not find creators of " + type.ToString());
        }
    }
}