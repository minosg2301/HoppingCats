using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public class UIGameSelectionInfo : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI storeText;
        public RawImage gameIcon;
        public RawImage gameImage;
        public UIButton openButton;
        public UIButton policyButton;
        public UIGameRankPoint rankPoint;
        public GameObject androidNode;
        public GameObject iOSNode;

        IGPGameInfo gameInfo;

        void Start()
        {
            if (openButton) openButton.OnClick.OnTrigger.Event.AddListener(OnClick);
            if (policyButton) policyButton.OnClick.OnTrigger.Event.AddListener(OnPolicy);

#if UNITY_ANDROID
            if (storeText) storeText.text = "Google Play";
            if (androidNode) androidNode.SetActive(true);
            if (iOSNode) iOSNode.SetActive(false);
#elif UNITY_IOS
            if (storeText) storeText.text = "App Store";
            if (androidNode) androidNode.SetActive(false);
            if (iOSNode) iOSNode.SetActive(true);
#endif
        }

        void OnClick()
        {
            IGP.OpenStore(gameInfo);
        }

        void OnPolicy()
        {
            Application.OpenURL(gameInfo.PolicyLink);
        }

        public void SetData(IGPGameInfo gameInfo)
        {
            this.gameInfo = gameInfo;
            nameText.text = gameInfo.Name;
            UpdateGameImage(gameInfo);
            UpdateGameIcon(gameInfo);
#if UNITY_ANDROID
            if (rankPoint) rankPoint.SetValue(gameInfo.AndroidRankPoint);
#elif UNITY_IOS
            if (rankPoint) rankPoint.SetValue(gameInfo.IosRankPoint);
#endif
        }

        async void UpdateGameIcon(IGPGameInfo gameInfo)
        {
            gameIcon.texture = await IGP.GetGameIconAsync(gameInfo);
        }

        async void UpdateGameImage(IGPGameInfo gameInfo)
        {
            gameImage.texture = await IGP.GetGameImageAsync(gameInfo);
        }
    }
}