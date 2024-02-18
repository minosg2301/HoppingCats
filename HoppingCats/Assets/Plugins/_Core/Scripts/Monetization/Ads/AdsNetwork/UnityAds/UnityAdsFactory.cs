using System;
using System.Collections.Generic;

namespace moonNest.ads
{
    internal partial class UnityAds
    {
        static readonly Dictionary<AdsType, Type> adsDisplayerCreators = new Dictionary<AdsType, Type>()
        {
#if USE_UNITY_ADS
            //{ AdsType.BANNER, typeof(IronSourceBanner)},
            { AdsType.INTERSTITIAL, typeof(UnityInterstitial)},
            { AdsType.REWARDED, typeof(UnityRewardAds)}
#endif
        };

        internal override AdsDisplayer CreateAdsDisplayer(AdsType type, DisplayConfig displayConfig)
        {
            if(adsDisplayerCreators.TryGetValue(type, out var classType))
            {
                var displayer = Activator.CreateInstance(classType, this, displayConfig) as AdsDisplayer;
                displayer.Init();
                return displayer;
            }
            throw new NullReferenceException("USE_UNITY_ADS not defined. Can not find creators of " + type.ToString());
        }
    }
}