using Doozy.Engine.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest.editor
{
    [CustomEditor(typeof(UITab))]
    [CanEditMultipleObjects]
    public class UITabEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UITab tabContainer = target as UITab;
            if(Draw.FitButton("Add New Tab Item"))
                AddNewTabItem(tabContainer);
        }

        public static void AddNewTabItem(UITab tabContainer)
        {
            GameObject contentGo = new GameObject("Tab Content", typeof(RectTransform), typeof(UITabContent));
            GameObjectUtility.SetParentAndAlign(contentGo, tabContainer.contentContainer.gameObject);
            contentGo.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            contentGo.GetComponent<RectTransform>().anchorMax = Vector2.one;
            contentGo.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            GameObject tabItemGo = new GameObject("Tab Item", typeof(RectTransform), typeof(UITabItem), typeof(UISwitcher));
            GameObjectUtility.SetParentAndAlign(tabItemGo, tabContainer.labelContainer.gameObject);
            tabItemGo.GetComponent<UITabItem>().tabContent = contentGo.GetComponent<UITabContent>();
            tabItemGo.GetComponent<UITabItem>().switcher = tabItemGo.GetComponent<UISwitcher>();
            
            GameObject buttonGo = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(UIButton), typeof(UISwitchColorTarget));
            GameObjectUtility.SetParentAndAlign(buttonGo, tabItemGo);
            buttonGo.GetComponent<RectTransform>().FitParent();
            buttonGo.GetComponent<Image>().sprite = UICreator.WhiteSprite;
            buttonGo.GetComponent<Image>().type = Image.Type.Sliced;
            tabItemGo.GetComponent<UITabItem>().button = buttonGo.GetComponent<UIButton>();

            GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            GameObjectUtility.SetParentAndAlign(textGo, buttonGo);
            textGo.GetComponent<TextMeshProUGUI>().verticalAlignment = VerticalAlignmentOptions.Middle;
            textGo.GetComponent<TextMeshProUGUI>().horizontalAlignment = HorizontalAlignmentOptions.Center;
            textGo.GetComponent<TextMeshProUGUI>().text = "Tab Title";
            textGo.GetComponent<TextMeshProUGUI>().color = Color.black;
            textGo.GetComponent<RectTransform>().FitParent();

            tabItemGo.GetComponent<UITabItem>().Selected = false;
            tabItemGo.GetComponent<UISwitcher>().UpdateTargets();
        }
    }

    [CustomEditor(typeof(UITabItem))]
    [CanEditMultipleObjects]
    public class UITabItemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Draw.Space(6);
            if(Draw.FitButton("Delete Tab"))
            {
                UITabItem tabLabel = target as UITabItem;
                DestroyImmediate(tabLabel.tabContent.gameObject);
                DestroyImmediate(tabLabel.gameObject);
            }
        }
    }
}