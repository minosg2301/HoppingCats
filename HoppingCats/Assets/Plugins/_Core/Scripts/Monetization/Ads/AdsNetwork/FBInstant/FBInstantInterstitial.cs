#if USE_FB_INSTANT_ADS
using System;
using UnityEngine.Scripting;

namespace vgame.ads
{
    internal class FBInstantInterstitial : AutoHideDisplayer
    {
        [Preserve]
        public FBInstantInterstitial(AdsNetwork network, DisplayConfig display)
            : base(AdsType.INTERSTITIAL, network, display)
        {
        }

        internal override void Init()
        {
            FBInstantPlugin.OnInterstitialAdReadyEvent += OnAdsReady;
            FBInstantPlugin.OnInterstitialAdLoadFailedEvent += OnAdsLoadFailed;
            FBInstantPlugin.OnInterstitialAdShowSucceededEvent += OnAdsShowSucceeded;
            FBInstantPlugin.OnInterstitialAdShowFailedEvent += OnAdsShowFailed;

            Load();
        }

        internal override void Load()
        {
            FBInstantPlugin.LoadInterstitial(Placement);
        }

        internal override void Show()
        {
            Ads.PauseGame();
            FBInstantPlugin.ShowInterstitial();

#if UNITY_EDITOR
            if (Ads.adsConfig.simulateShowFailed)
            {
                Ads.CallDelay(0.2f, OnAdsShowFailed);
            }
            else
            {
                // simulate ad display success
                Ads.CallDelay(0.2f, OnAdsShowSucceeded);
            }
#endif
        }

        void OnAdsReady()
        {
            IsAdReady = true;
            onAdsLoaded();
        }

        void OnAdsLoadFailed()
        {
            Ads.CallDelay(Ads.ReloadInterval, Load);
        }

        void OnAdsShowFailed()
        {
            IsAdReady = false;
            Ads.ResumeGame();
            Load();
        }

        void OnAdsShowSucceeded()
        {
            IsAdReady = false;
            onAdsHide();
            Ads.ResumeGame();
            Load();
        }
    }
}
#endif