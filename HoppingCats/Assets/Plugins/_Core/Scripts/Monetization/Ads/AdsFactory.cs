using System;
using System.Collections.Generic;

namespace moonNest.ads
{
    internal class AdsFactory
    {
        static readonly Dictionary<AdsNetworkID, Func<AdsNetworkConfig, AdsNetwork>> adsNetworkCreators = new Dictionary<AdsNetworkID, Func<AdsNetworkConfig, AdsNetwork>>()
        {
            { AdsNetworkID.IRONSRC, c => new IronSourceAds(c)},
            { AdsNetworkID.ADMOB, c => new AdmobAds(c)},
            { AdsNetworkID.APPLOVIN, c => new AppLovinAds(c)},
            { AdsNetworkID.UNITY, c => new UnityAds(c)},
            { AdsNetworkID.FB_INSTANT, c => new FBInstantAds(c)}
        };

        static readonly Dictionary<AdsType, Func<AdsController>> adsFormatCreators = new Dictionary<AdsType, Func<AdsController>>()
        {
            { AdsType.BANNER, () => new BannerController()},
            { AdsType.INTERSTITIAL, () => new InterstitialController()},
            { AdsType.REWARDED, () => new RewardAdsController()},
            { AdsType.NATIVE, () => new NativeAdsController()},
            { AdsType.RWD_INTER, () => new RewardInterController()},
            { AdsType.APP_OPEN, () => new AppOpenAdsController()}
        };

        internal static AdsNetwork CreateAdsNetwork(AdsNetworkConfig config)
        {
            if(adsNetworkCreators.TryGetValue(config.id, out var creator))
            {
                var adsNetwork = creator.Invoke(config);

                var baseType = typeof(AdsNetwork);
                var type = adsNetwork.GetType();
                if(!type.IsSubclassOf(baseType))
                    throw new InvalidCastException($"{type} must be a sub class of {baseType}");

                return adsNetwork;
            }
            throw new KeyNotFoundException("Can not find creator of " + config.id.ToString());
        }

        internal static AdsController CreateAdsController(AdsType adsType)
        {
            if(adsFormatCreators.TryGetValue(adsType, out var creator))
            {
                var controller = creator.Invoke();

                var baseType = typeof(AdsController);
                var type = controller.GetType();
                if(controller == null || !type.IsSubclassOf(baseType))
                    throw new InvalidCastException($"{type} must be a sub class of {baseType}");

                controller.Init();

                return controller;
            }
            throw new KeyNotFoundException("Can not find creator of " + adsType.ToString());
        }
    }
}