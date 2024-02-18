#if USE_FB_INSTANT_ADS
using UnityEngine.Scripting;
using vgame.tracking;

namespace vgame.ads
{
    internal class FBInstantRewardInterAds : AutoHideDisplayer
    {
        [Preserve]
        public FBInstantRewardInterAds(AdsNetwork network, DisplayConfig display)
            : base(AdsType.RWD_INTER, network, display)
        {
        }

        internal override void Init()
        {
            FBInstantPlugin.OnRewardInterReadyEvent += OnAdsReady;
            FBInstantPlugin.OnRewardInterLoadFailedEvent += OnAdsLoadFailed;
            FBInstantPlugin.OnRewardInterShowSucceededEvent += OnAdsShowSucceeded;
            FBInstantPlugin.OnRewardInterShowFailedEvent += OnAdsShowFailed;

            Load();
        }

        internal override void Load()
        {
            FBInstantPlugin.LoadRewardInterAd(Placement);
        }

        internal override void Show()
        {
            Ads.PauseGame();
            FBInstantPlugin.ShowRewardInterAd();
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