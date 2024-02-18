using System;
using System.Collections.Generic;

namespace moonNest.ads
{
    internal abstract class AdsNetwork
    {
        public AdsNetworkID Id { get; }

        public AdsNetworkConfig Config { get; }
        public PlatformConfig PlatformConfig { get; }

        protected Dictionary<AdsType, AdsDisplayer> displayers = new Dictionary<AdsType, AdsDisplayer>();

        protected AdsNetwork(AdsNetworkConfig config)
        {
            Id = config.id;
            Config = config;
#if UNITY_ANDROID
            PlatformConfig = config.android;
#elif UNITY_IOS
            PlatformConfig = config.ios;
#elif UNITY_WEBGL
            PlatformConfig = config.web;
#else
            PlatformConfig = null;
#endif
        }

        internal abstract void Init();
        internal abstract AdsDisplayer CreateAdsDisplayer(AdsType type, DisplayConfig displayConfig);

        internal AdsDisplayer RegisterDisplayer(AdsType type, DisplayConfig displayConfig)
        {
            if(displayers.TryGetValue(type, out var displayer))
                return displayer;

            displayer = CreateAdsDisplayer(type, displayConfig);
            displayers[type] = displayer;
            return displayer;
        }
    }
}