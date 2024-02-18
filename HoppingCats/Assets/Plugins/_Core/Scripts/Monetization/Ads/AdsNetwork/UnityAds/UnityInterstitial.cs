#if USE_UNITY_ADS
using UnityEngine.Advertisements;
using tracking;

namespace vgame.ads
{
    internal class UnityInterstitial : AutoHideDisplayer, IUnityAdsShowListener
    {
        [Preserve]
        public UnityInterstitial(AdsNetwork adsNetwork, DisplayConfig displayConfig) : base(AdsType.INTERSTITIAL, adsNetwork, displayConfig)
        {
        }

        internal override bool Available => Advertisement.IsReady(Placement);

        internal override void Init()
        {
        }

        internal override void Load()
        {
        }

        internal override void Show()
        {
            Advertisement.Show(Placement, this);
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            onAdsShow?.Invoke();
            Ads.PauseGame();
        }

        public void OnUnityAdsShowClick(string placementId)
        {
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            onAdsHide?.Invoke();
            Ads.ResumeGame();
            Ads.UpdateShowAdsComplete(Type, true);
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            onAdsHide();
            Ads.ResumeGame();
            Ads.UpdateShowAdsComplete(Type, false);
            TrackingManager.OnInterstitialShow(AdsNetwork.ToString(), Placement);
        }
    }
}
#endif // USE_UNITY_ADS