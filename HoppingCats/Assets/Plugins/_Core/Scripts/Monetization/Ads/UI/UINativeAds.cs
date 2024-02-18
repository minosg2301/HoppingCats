using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace moonNest.ads
{
    public class UINativeAds : MonoBehaviour
    {
        #region ui creator
#if UNITY_EDITOR
        private static void CreateUINativeAds(MenuCommand menuCommand, NativeAdsType adsType, Vector2 size)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if (selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UINativeAds", typeof(RectTransform), typeof(UINativeAds));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().MidCenter(size);

            GameObject backgroundGo = new GameObject("Background", typeof(RectTransform), typeof(Image));
            GameObjectUtility.SetParentAndAlign(backgroundGo, go);
            backgroundGo.GetComponent<RectTransform>().FitParent();
            backgroundGo.GetComponent<Image>().raycastTarget = false;
            backgroundGo.GetComponent<Image>().color = new(0f, 0f, 0f, 0.98f);

            GameObject contentGo = new GameObject("Content", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(contentGo, go);
            contentGo.GetComponent<RectTransform>().FitParent();

            UINativeAds nativeAds = go.GetComponent<UINativeAds>();
            nativeAds.adsType = adsType;
            nativeAds.background = backgroundGo;
            nativeAds.content = contentGo.GetComponent<RectTransform>();

            Selection.activeObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        }

        [MenuItem("GameObject/MoonNest/Ads/UINativeAds/Small", false, 10)]
        static void CreateUINativeAdsSmall(MenuCommand menuCommand)
        {
            CreateUINativeAds(menuCommand, NativeAdsType.SMALL, new Vector2(690, 220));
        }

        [MenuItem("GameObject/MoonNest/Ads/UINativeAds/Medium", false, 10)]
        static void CreateUINativeAdsMedium(MenuCommand menuCommand)
        {
            CreateUINativeAds(menuCommand, NativeAdsType.MEDIUM, new Vector2(650, 580));
        }
#endif
        #endregion

        public NativeAdsType adsType = NativeAdsType.MEDIUM;
        public GameObject background;
        public RectTransform content;
        public Color textColor = Color.white;
        public Color actionColor = Color.white;
        public UnityEvent<bool> showEvent;
        public UnityEvent<bool> dismissEvent;

        bool initialized = false;
        bool waitAdsForShow = false;
        bool adsShown = false;

        private Canvas canvas;
        public Canvas Canvas { get { if (!canvas) canvas = GetComponentInParent<UICanvas>().Canvas; return canvas; } }

        void Start()
        {
            if (!GlobalConfig.Ins.adsEnabled)
            {
                background.SetActive(false);
                content.gameObject.SetActive(false);
                return;
            }

            initialized = true;
            UpdateShowingAds();
        }

        void OnEnable()
        {
            adsShown = false;
            Ads.Subscribe(AdsType.NATIVE, OnNativeAdsUpdated);
            if (initialized) UpdateShowingAds();
        }

        void OnDisable()
        {
            Ads.Unsubscribe(AdsType.NATIVE, OnNativeAdsUpdated);
            if (initialized)
            {
                Ads.HideNative(new NativeAdsData(adsType));
            }
            waitAdsForShow = false;
        }

        void OnNativeAdsUpdated(Ads ads)
        {
            if (adsShown) return;

            bool canShowAds = Ads.IsNativeAdsReady(adsType) && Ads.IsDisplayable(AdsType.NATIVE) && Ads.IsDisplayable();
            if (canShowAds && waitAdsForShow)
            {
                waitAdsForShow = false;
                ShowAds();
            }
            UpdateVisibility(canShowAds);
        }

        void UpdateShowingAds()
        {
            bool adReady = Ads.IsNativeAdsReady(adsType);
            bool displayable = Ads.IsDisplayable(AdsType.NATIVE) && Ads.IsDisplayable();
            if (displayable)
            {
                if (adReady) ShowAds();
                else
                {
                    waitAdsForShow = true;
                }
            }
            UpdateVisibility(adReady && displayable);
        }

        void UpdateVisibility(bool show)
        {
            background.SetActive(show);
            showEvent.Invoke(show);
            dismissEvent.Invoke(!show);
        }

        void ShowAds()
        {
            adsShown = true;

            // get four corners of content rect
            var corners = new Vector3[4]; content.GetWorldCorners(corners);

            // get left-top corner position
            var position = RectTransformUtility.WorldToScreenPoint(Canvas.worldCamera, corners[1]);

            // create ads rect in screen space
#if !UNITY_EDITOR && UNITY_IOS
            var x = position.x / Screen.width;
            var y = 1f - position.y / Screen.height;
            var width = (content.rect.width * Canvas.scaleFactor) / Screen.width;
            var height = (content.rect.height * Canvas.scaleFactor) / Screen.height;
#else
            var x = position.x / CoreHandler.ScaleFactor;
            var y = position.y / CoreHandler.ScaleFactor;
            var width = (content.rect.width * Canvas.scaleFactor) / CoreHandler.ScaleFactor;
            var height = (content.rect.height * Canvas.scaleFactor) / CoreHandler.ScaleFactor;
            var trueScreenH = Screen.height / CoreHandler.ScaleFactor;

            y = Mathf.Clamp(trueScreenH - y, 0, trueScreenH);
#endif

            var adsRect = new AdsRect(x, y, width, height);
            //Debug.Log($"trueScreenH: {trueScreenH}");
            //Debug.Log($"{position.x} - {position.y} - {content.rect.width} - {content.rect.height}");
            //Debug.Log($"ScaleFactor: {CoreHandler.ScaleFactor}, Canvas.scaleFactor: {Canvas.scaleFactor}");
            //Debug.Log($"rect - " + adsRect.ToString());

            // show native ads
            Ads.ShowNative(new NativeAdsData(adsType, adsRect, textColor, actionColor));
        }
    }
}