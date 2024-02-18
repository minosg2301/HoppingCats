using System;
using System.Collections.Generic;

namespace moonNest.ads
{
    internal partial class IronSourceAds
    {
        static readonly Dictionary<AdsType, Type> adsDisplayerCreators = new Dictionary<AdsType, Type>()
        {
#if USE_IRONSRC_ADS
            { AdsType.BANNER, typeof(IronSourceBanner)},
            { AdsType.INTERSTITIAL, typeof(IronSourceInterstitial)},
            { AdsType.REWARDED, typeof(IronSourceRewardAds)}
#endif // USE_IRONSRC_ADS
        };

        internal override AdsDisplayer CreateAdsDisplayer(AdsType type, DisplayConfig displayConfig)
        {
            if(adsDisplayerCreators.TryGetValue(type, out var classType))
            {
                var displayer = Activator.CreateInstance(classType, this, displayConfig) as AdsDisplayer;
                displayer.Init();
                return displayer;
            }
            throw new NullReferenceException("USE_IRONSRC_ADS not defined. Can not find creators of " + type.ToString());
        }
    }
}