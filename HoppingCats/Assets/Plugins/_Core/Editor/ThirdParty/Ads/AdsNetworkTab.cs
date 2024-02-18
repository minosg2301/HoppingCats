using FullSerializer;
using System;

namespace moonNest.ads
{
    internal class AdsNetworkTab : TabContent
    {
        private AdsConfig adsConfig;
        private readonly TableDrawer<AdsNetworkConfig> networkTable;

        internal AdsNetworkTab()
        {
            networkTable = new TableDrawer<AdsNetworkConfig>();
            networkTable.AddCol("Enabled", 60, ele => ele.enabled = Draw.Toggle(ele.enabled, 60));
            networkTable.AddCol("Network", 120, ele => ele.id = Draw.Enum(ele.id, 120), false);
            networkTable.AddCol("ios SDK", 350, ele =>
            {
                Draw.BeginDisabledGroup(!ele.enableSDKKey);
                ele.ios.sdkKey = Draw.Text(ele.ios.sdkKey, 350);
                Draw.EndDisabledGroup();
            });
            networkTable.AddCol("Android SDK", 350, ele =>
            {
                Draw.BeginDisabledGroup(!ele.enableSDKKey);
                ele.android.sdkKey = Draw.Text(ele.android.sdkKey, 350);
                Draw.EndDisabledGroup();
            });
            networkTable.drawDeleteButton = false;
            networkTable.drawIndex = false;
            networkTable.drawControl = false;
            networkTable.drawOrder = false;

            CorrectAdsNetwork();
        }

        void CorrectAdsNetwork()
        {
            adsConfig = ThirdPartyConfig.Ins.adsConfig;
            if (adsConfig.networks.Count < Enum.GetNames(typeof(AdsNetworkID)).Length)
            {
                var values = Enum.GetValues(typeof(AdsNetworkID));
                foreach (var value in values)
                {
                    AdsNetworkID networkID = (AdsNetworkID)value;
                    if (adsConfig.FindAdsNetwork(networkID) == null)
                    {
                        var adsNetwork = new AdsNetworkConfig() { id = networkID, enableSDKKey = networkID != AdsNetworkID.ADMOB };

                        // hard code sdk keys
                        if (AdsNetworkID.APPLOVIN == adsNetwork.id)
                        {
                            adsNetwork.ios.sdkKey =
                            adsNetwork.android.sdkKey = "NTAl9ByXZ6-NKKJwT2AFeA4USE0KSAbrdnZozWKWdIuydCsAE1QuCUNekNzBTZ0BLazI5IpkahfCvWN8To0lHN";
                        }

                        adsConfig.networks.Add(adsNetwork);
                    }
                }
            }
        }

        public override void DoDraw()
        {
            networkTable.DoDraw(adsConfig.networks);

            Draw.Space();

            Draw.BeginHorizontal();

            Draw.BeginVertical();
            Draw.LabelBold("Ads Simulation");
            adsConfig.simulateAdsTest = Draw.ToggleField("Native Ads Test", adsConfig.simulateAdsTest, 80); 
            adsConfig.simulateShowFailed = Draw.ToggleField("Show Failed", adsConfig.simulateShowFailed, 80);
            adsConfig.simulateNoInterstitial = Draw.ToggleField("No Interstitial", adsConfig.simulateNoInterstitial, 80);
            adsConfig.simulateNoRewardAds = Draw.ToggleField("No Reward Video", adsConfig.simulateNoRewardAds, 80);
            adsConfig.simulateNoRewardInterAds = Draw.ToggleField("No Reward Inter", adsConfig.simulateNoRewardInterAds, 80);

            Draw.Space();
            Draw.LabelBold("Interstitial");
            adsConfig.skipAds = Draw.ToggleField("Skip Ads Rule", adsConfig.skipAds, 80);
            Draw.BeginDisabledGroup(!adsConfig.skipAds);
            adsConfig.skipAdsDuration = Draw.FloatField("Skip Ads Duration (s)", adsConfig.skipAdsDuration, 80);
            Draw.EndDisabledGroup();

            Draw.Space();
            Draw.LabelBold("Debugging");
            adsConfig.maxSDKDebug = Draw.ToggleField("MaxSDK Debug", adsConfig.maxSDKDebug, 80);
            Draw.EndVertical();

            Draw.Space();
            Draw.BeginVertical();
            Draw.LabelBold("Native Ads");
            adsConfig.nativeAds ??= new NativeAdsConfig();
            adsConfig.nativeAds.delayRefreshSeconds = Draw.IntField("Delay Seconds", adsConfig.nativeAds.delayRefreshSeconds, 80);
            adsConfig.nativeAds.noDelayRefresh = Draw.ToggleField("No Delay Refresh", adsConfig.nativeAds.noDelayRefresh, 80);
            Draw.EndVertical();

            Draw.FlexibleSpace();

            Draw.EndHorizontal();
        }
    }
}