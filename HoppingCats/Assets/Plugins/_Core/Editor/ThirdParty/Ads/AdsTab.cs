namespace moonNest.ads
{
    internal class AdsTab : TabContent
    {
        private readonly TabContainer tabContainer;

        public AdsTab()
        {
            tabContainer = new TabContainer();
            tabContainer.AddTab("Networks", new AdsNetworkTab());
            tabContainer.AddTab("IOS", new AdsFormatTab(ThirdPartyConfig.Ins.adsConfig.ios));
            tabContainer.AddTab("ANDROID", new AdsFormatTab(ThirdPartyConfig.Ins.adsConfig.android));
            tabContainer.AddTab("Web", new AdsFormatTab(ThirdPartyConfig.Ins.adsConfig.web));
            tabContainer.AddTab("HUAWEI", new AdsFormatTab(ThirdPartyConfig.Ins.adsConfig.huawei));
        }

        public override void DoDraw()
        {
            tabContainer.DoDraw();
        }
    }
}