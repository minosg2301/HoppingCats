#if USE_MAX_SDK
using UnityEngine;
namespace vgame.ads
{
    internal partial class AppLovinAds : AdsNetwork
    {
        public bool PendingInit { get; private set; }

        public AppLovinAds(AdsNetworkConfig config) : base(config)
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += OnSDKInitialized;
        }

        void OnSDKInitialized(MaxSdkBase.SdkConfiguration obj)
        {
            MaxSdkCallbacks.OnSdkInitializedEvent -= OnSDKInitialized;

            // off flag
            PendingInit = false;

            // init displayer here
            displayers.Values.ForEach(displayer => displayer.Init());

            if (Ads.DebugMode && ThirdPartyConfig.Ins.adsConfig.maxSDKDebug)
                MaxSdk.ShowMediationDebugger();
        }

        internal override void Init()
        {
            // wait for max sdk init
            PendingInit = true;

            // init max sdk
            MaxSdk.SetSdkKey(PlatformConfig.sdkKey);
            MaxSdk.InitializeSdk();
        }

        public override string ToString() { return "applovin"; }
    }
}

#else // USE_MAX_SDK
namespace moonNest.ads
{
    internal partial class AppLovinAds : AdsNetwork
    {
        public bool PendingInit { get; private set; }

        public AppLovinAds(AdsNetworkConfig config) : base(config) { }

        internal override void Init() { }
    }
}
#endif