#if USE_FB_INSTANT_ADS
using UnityEngine.Scripting;
using vgame.tracking;

namespace vgame.ads
{
    internal class FBInstantRewardAds : AutoHideDisplayer
    {
        [Preserve]
        public FBInstantRewardAds(AdsNetwork network, DisplayConfig display)
            : base(AdsType.REWARDED, network, display)
        {
        }

        internal override void Init()
        {
            FBInstantPlugin.OnRewardedAdReadyEvent += OnAdsReady;
            FBInstantPlugin.OnRewardedAdLoadFailedEvent += OnAdsLoadFailed;
            FBInstantPlugin.OnRewardedAdShowSucceededEvent += OnAdsShowSucceeded;
            FBInstantPlugin.OnRewardedAdShowFailedEvent += OnAdsShowFailed;

            Load();
        }

        internal override void Load()
        {
            FBInstantPlugin.LoadRewardAd(Placement);
        }

        internal override void Show()
        {
            Ads.PauseGame();
            FBInstantPlugin.ShowRewardAd();
            onAdsShow();

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
            onAdsHide();
            Ads.ResumeGame();
            Ads.UpdateShowAdsComplete(Type, false);
            Load();
        }

        void OnAdsShowSucceeded()
        {
            IsAdReady = false;
            onAdsShowCompleted();            
            onAdsHide();
            Ads.ResumeGame();
            Ads.UpdateShowAdsComplete(Type, true);
            Load();
        }
    }
}
#endif