using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace moonNest.ads
{
    [RequireComponent(typeof(UIButton))]
    public class UIRewardInterstitialButton : MonoBehaviour
    {
        #region ui creator
#if UNITY_EDITOR
        [MenuItem("GameObject/MoonNest/Ads/UIRewardInterstitialButton", false, 0)]
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
            go.name = "UIRewardInterstitialButton";

            GameObject readyNode = new GameObject("ReadyNode", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(readyNode, go);
            readyNode.GetComponent<RectTransform>().FitParent();

            GameObject notAvailableNode = new GameObject("NotAvailableNode", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(notAvailableNode, go);
            notAvailableNode.GetComponent<RectTransform>().FitParent();

            var rewardInterButton = go.AddComponent<UIRewardInterstitialButton>();
            rewardInterButton.readyNode = readyNode;
            rewardInterButton.notAvailableNode = notAvailableNode;

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
            gameObject.name = "UIRewardInterstitialButton";
            if (!button) button = GetComponent<UIButton>();
        }

        void Start()
        {
            button.OnClick.OnTrigger.Event.AddListener(OnButtonClicked);
        }

        void OnEnable()
        {
            Ads.Subscribe(AdsType.RWD_INTER, OnRewardInterstitialUpdated);
        }

        void OnDisable()
        {
            Ads.Unsubscribe(AdsType.RWD_INTER, OnRewardInterstitialUpdated);
        }

        void OnButtonClicked()
        {
            button.Interactable = false;
            Ads.ShowRewardInter();
            UpdateAvailabilityAds(Ads.IsAvailable(AdsType.RWD_INTER));
            showAdsCompleted.Invoke(true);
        }

        void OnRewardInterstitialUpdated(Ads ads)
        {
            UpdateAvailabilityAds(Ads.IsAvailable(AdsType.RWD_INTER));
        }

        protected virtual void UpdateAvailabilityAds(bool available)
        {
            readyNode.SetActive(available);
            notAvailableNode.SetActive(!available);
            button.Interactable = available;
        }
    }
}