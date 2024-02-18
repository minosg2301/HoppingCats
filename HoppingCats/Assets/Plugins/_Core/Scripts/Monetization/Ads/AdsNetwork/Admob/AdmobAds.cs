namespace moonNest.ads
{
    internal partial class AdmobAds : AdsNetwork
    {
        public bool PendingDisplayerInit { get; private set; }

        public AdmobAds(AdsNetworkConfig config) : base(config) { }

        internal override void Init()
        {
            PendingDisplayerInit = true;

            NativeAdmob.Ins.OnInitialized += OnInitialized;
            NativeAdmob.Ins.Initialize(Ads.adsConfig.nativeAds ?? new NativeAdsConfig());
        }

        void OnInitialized()
        {
            PendingDisplayerInit = false;

            foreach (var displayer in displayers.Values)
            {
                displayer.Init();
            }
        }

        public override string ToString() { return "admob"; }
    }
}