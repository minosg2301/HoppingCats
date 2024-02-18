#if USE_UNITY_ADS
using UnityEngine;
using UnityEngine.Advertisements;

namespace vgame.ads
{
    internal partial class UnityAds : AdsNetwork
    {
        public UnityAds(AdsNetworkConfig config) : base(config)
        {
        }

        internal override void Init()
        {
            Advertisement.Initialize(PlatformConfig.sdkKey, false);
        }

        public override string ToString() { return "unityAds"; }
    }
}
#else // USE_UNITY_ADS 
namespace moonNest.ads
{
    internal partial class UnityAds : AdsNetwork
    {
        public UnityAds(AdsNetworkConfig config) : base(config) { }
        internal override void Init() { }
    }
}
#endif // USE_UNITY_ADS