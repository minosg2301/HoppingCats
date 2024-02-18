#if USE_FB_INSTANT_ADS
namespace vgame.ads
{
    internal partial class FBInstantAds : AdsNetwork
    {
        public FBInstantAds(AdsNetworkConfig config) : base(config)
        {
        }

        internal override void Init()
        {
        }

        public override string ToString() { return "unityAds"; }
    }
}
#else // USE_FB_INSTANT_ADS 
namespace moonNest.ads
{
    internal partial class FBInstantAds : AdsNetwork
    {
        public FBInstantAds(AdsNetworkConfig config) : base(config) { }
        internal override void Init() { }
    }
}
#endif // USE_FB_INSTANT_ADS