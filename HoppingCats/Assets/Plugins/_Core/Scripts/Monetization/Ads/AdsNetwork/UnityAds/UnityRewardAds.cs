#if USE_UNITY_ADS
using UnityEngine.Advertisements;
using vgame.tracking;

namespace vgame.ads
{
    internal class UnityRewardAds : AutoHideDisplayer, IUnityAdsShowListener
    {
        [Preserve]
        public UnityRewardAds(AdsNetwork adsNetwork, DisplayConfig displayConfig) : base(AdsType.REWARDED, adsNetwork, displayConfig)
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
            if(videoCompleted) TrackingManager.OnRewardAdsComplete(AdsNetwork.ToString(), Placement);
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            onAdsHide();
            Ads.ResumeGame();
            Ads.UpdateShowAdsComplete(Type, false);
        }
    }
}
#endif // USE_UNITY_ADS