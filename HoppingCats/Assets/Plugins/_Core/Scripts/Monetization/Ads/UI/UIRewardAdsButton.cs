using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.Events;
using JetBrains.Annotations;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace moonNest.ads
{
    [RequireComponent(typeof(UIButton))]
    public class UIRewardAdsButton : MonoBehaviour
    {
        #region ui creator
#if UNITY_EDITOR
        [MenuItem("GameObject/MoonNest/Ads/UIRewardAdsButton", false, 0)]
        static void CreateUI(MenuCommand menuCommand)
        {
            Vector3 size = new Vector3(160, 30);
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if (selectedGO != null) targetParent = selectedGO;

            UIButton uiButton = UIButton.CreateUIButton(targetParent);
#if dUI_TextMeshPro
            uiButton.TextMeshProLabel.text = "Show Ads";
#endif

            GameObject go = uiButton.gameObject;
            go.GetComponent<RectTransform>().MidCenter(size);
            go.name = "UIRewardAdsButton";

            GameObject readyNode = new GameObject("ReadyNode", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(readyNode, go);
            readyNode.GetComponent<RectTransform>().FitParent();

            GameObject notAvailableNode = new GameObject("NotAvailableNode", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(notAvailableNode, go);
            notAvailableNode.GetComponent<RectTransform>().FitParent();

            var rewardAdsButton = go.AddComponent<UIRewardAdsButton>();
            rewardAdsButton.readyNode = readyNode;
            rewardAdsButton.notAvailableNode = notAvailableNode;

            Selection.activeObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        }
#endif
        #endregion

        public UIButton button;
        public GameObject readyNode;
        public GameObject notAvailableNode;

        [System.Obsolete("DONT USE THIS! Keep for compat, this event will be removed in future. Use 2 Unity Event below")]
        public UnityEvent<bool> showAdsCompleted;

        /// <summary>
        /// Called when rewarded video is completely watched
        /// </summary>
        public UnityEvent onAdsRewarded;

        /// <summary>
        /// Called when rewarded video is canceled
        /// </summary>
        public UnityEvent onAdsCanceled;

        public string action = "default";

        void Reset()
        {
            gameObject.name = "UIRewardAdsButton";
            if (!button) button = GetComponent<UIButton>();
        }

        void Start()
        {
            button.OnClick.OnTrigger.Event.AddListener(OnButtonClicked);
        }

        void OnEnable()
        {
            Ads.Subscribe(OnRewardAdsUpdated, false);
            Ads.Subscribe(AdsType.REWARDED, OnRewardAdsUpdated);
        }

        void OnDisable()
        {
            Ads.Unsubscribe(OnRewardAdsUpdated);
            Ads.Unsubscribe(AdsType.REWARDED, OnRewardAdsUpdated);
        }

        void OnButtonClicked()
        {
            button.Interactable = false;
            Ads.ShowRewardAds(action, UpdateShowAdsCompleted);
        }

        void UpdateShowAdsCompleted(bool completed)
        {
            UpdateAvailabilityAds();
            showAdsCompleted.Invoke(completed);

            if (completed) onAdsRewarded.Invoke();
            else onAdsCanceled.Invoke();
        }

        void OnRewardAdsUpdated(Ads ads)
        {
            UpdateAvailabilityAds();
        }

        protected virtual void UpdateAvailabilityAds()
        {
            gameObject.SetActive(Ads.IsDisplayable() && Ads.IsDisplayable(AdsType.REWARDED));

            bool adsAvaialble = Ads.IsAvailable(AdsType.REWARDED);
            readyNode.SetActive(adsAvaialble);
            notAvailableNode.SetActive(!adsAvaialble);
            button.Interactable = adsAvaialble;
        }
    }
}