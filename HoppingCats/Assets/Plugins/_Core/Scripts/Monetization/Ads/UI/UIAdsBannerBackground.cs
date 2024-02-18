using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace moonNest.ads
{
    [RequireComponent(typeof(Image))]
    public class UIAdsBannerBackground : MonoBehaviour
    {
        #region ui creator
#if UNITY_EDITOR
        [MenuItem("GameObject/MoonNest/Ads/UIAdsBannerBackground", false, 0)]
        static void CreateUI(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if (selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UIAdsBannerBackground", typeof(RectTransform), typeof(UIAdsBannerBackground));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().BottomStretch(50);
            go.GetComponent<Image>().color = Color.gray;

            Selection.activeObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        }
#endif
        #endregion

        public SafeArea safeArea;

        private Canvas _canvas;
        public Canvas Canvas { get { if(!_canvas) _canvas = GetComponentInParent<Canvas>(); return _canvas; } }


        private RectTransform _rectTransform;
        public RectTransform RectTransform { get { if(!_rectTransform) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }


        void Start()
        {
            Ads.onBannerVisibilityChanged += UpdateBannerBackground;
            UpdateBannerBackground(Ads.IsShowing(AdsType.BANNER));

            if(safeArea) safeArea.onApplied += () => UpdateBannerBackground(Ads.IsShowing(AdsType.BANNER));
        }

        private void UpdateBannerBackground(bool visible)
        {
            gameObject.SetActive(visible);
            if (visible)
            {
#if UNITY_EDITOR
                float bannerSize = Ads.BannerSize;
#else
                float bannerSize = Ads.BannerSize / Canvas.scaleFactor;
#endif

                this.CallInNextFrame(() =>
                {
                    var fillSize = new Vector2(0, bannerSize);
                    if(safeArea && safeArea.ApplyingSafeArea)
                    {
                        var padding = new Vector2(safeArea.GetSafeArea().x, safeArea.GetSafeArea().y) / Canvas.scaleFactor;
                        fillSize = new Vector2(0, bannerSize + padding.y);
                    }
                    RectTransform.sizeDelta = fillSize;
                });
            }
        }
    }
}