using System;
using System.Collections.Generic;

namespace moonNest.ads
{
    internal partial class AdmobAds
    {
        static readonly Dictionary<AdsType, Type> adsDisplayerCreators = new Dictionary<AdsType, Type>()
        {
#if USE_GOOGLE_MOBILE_ADS
            { AdsType.BANNER, typeof(AdmobBanner)},
            { AdsType.INTERSTITIAL, typeof(AdmobInterstitial)},
            { AdsType.REWARDED, typeof(AdmobRewardAds)},
#endif
#if USE_NATIVE_ADMOB
            { AdsType.RWD_INTER, typeof(AdmobRewardInter)},
            { AdsType.NATIVE, typeof(AdmobNative)},
            { AdsType.APP_OPEN, typeof(AppOpenAd)}
#endif
        };

        internal override AdsDisplayer CreateAdsDisplayer(AdsType type, DisplayConfig displayConfig)
        {
            if (adsDisplayerCreators.TryGetValue(type, out var classType))
            {
                var displayer = Activator.CreateInstance(classType, this, displayConfig) as AdsDisplayer;
                if (!PendingDisplayerInit) displayer.Init();
                return displayer;
            }
            if (AdsType.RWD_INTER == type
                || AdsType.NATIVE == type
                || AdsType.APP_OPEN == type)
            {
                throw new NullReferenceException("USE_NATIVE_ADMOB not defined. Can not find creators of " + type.ToString());
            }
            else
            {
                throw new NullReferenceException("USE_GOOGLE_MOBILE_ADS not defined. Can not find creators of " + type.ToString());
            }
        }
    }
}