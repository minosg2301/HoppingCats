namespace moonNest.ads
{
    internal abstract class NativeAdsDisplayer : AdsDisplayer
    {
        internal NativeAdsData adsData;

        internal NativeAdsDisplayer(AdsType type, AdsNetwork network, DisplayConfig display)
            : base(type, network, display)
        {
        }

        internal abstract bool IsAdsReady(NativeAdsType adsType);
        internal abstract void Load(NativeAdsType adsType);
    }
}