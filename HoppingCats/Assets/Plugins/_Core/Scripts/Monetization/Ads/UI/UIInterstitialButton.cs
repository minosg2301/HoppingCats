using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace moonNest.ads
{
    [RequireComponent(typeof(UIButton))]
    public class UIInterstitialButton : MonoBehaviour
    {
        #region ui creator
#if UNITY_EDITOR
        [MenuItem("GameObject/MoonNest/Ads/UIInterstitialButton", false, 0)]
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
            go.name = "UIInterstitialButton";

            GameObject readyNode = new GameObject("ReadyNode", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(readyNode, go);
            readyNode.GetComponent<RectTransform>().FitParent();

            GameObject notAvailableNode = new GameObject("NotAvailableNode", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(notAvailableNode, go);
            notAvailableNode.GetComponent<RectTransform>().FitParent();

            var interstitialButton = go.AddComponent<UIInterstitialButton>();
            interstitialButton.readyNode = readyNode;
            interstitialButton.notAvailableNode = notAvailableNode;

            Selection.activeObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        }
#endif
        #endregion

        public UIButton button;
        public GameObject readyNode;
        public GameObject notAvailableNode;
        public UnityEvent<bool> showAdsCompleted;

        void Reset()
        {
            gameObject.name = "UIInterstitialButton";
            if (!button) button = GetComponent<UIButton>();
        }

        void Start()
        {
            button.OnClick.OnTrigger.Event.AddListener(OnButtonClicked);
        }

        void OnEnable()
        {
            Ads.Subscribe(AdsType.INTERSTITIAL, OnInterstitialUpdated);
        }

        void OnDisable()
        {
            Ads.Unsubscribe(AdsType.INTERSTITIAL, OnInterstitialUpdated);
        }

        void OnButtonClicked()
        {
            button.Interactable = false;
            Ads.ShowInterstitial();
            UpdateAvailabilityAds(Ads.IsAvailable(AdsType.INTERSTITIAL));
            showAdsCompleted.Invoke(true);
        }

        void OnInterstitialUpdated(Ads ads)
        {
            UpdateAvailabilityAds(Ads.IsAvailable(AdsType.INTERSTITIAL));
        }

        protected virtual void UpdateAvailabilityAds(bool available)
        {
            readyNode.SetActive(available);
            notAvailableNode.SetActive(!available);
            button.Interactable = available;
        }
    }
}