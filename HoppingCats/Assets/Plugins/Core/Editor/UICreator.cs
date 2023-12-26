using Doozy.Engine.UI;
using I2.Loc;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest.editor
{
    public class UICreator
    {
        public const string kStandardSpritePath = "UI/Skin/UISprite.psd";
        public const string kWhiteSprite = "Assets/Plugins/Extensions/Doozy/Examples/Textures/Shapes/SquareFull@32px.png";
        public const string kLockSprite = "Assets/Plugins/Extensions/Doozy/Examples/Icons/Awesome/lock-alt.png";

        public static Sprite WhiteSprite => AssetDatabase.LoadAssetAtPath<Sprite>(kWhiteSprite);
        public static Sprite LockSprite => AssetDatabase.LoadAssetAtPath<Sprite>(kLockSprite);

        private static void SetActiveAndUndo(GameObject go)
        {
            Selection.activeObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        }

        public static UIButton CreateUIButton(GameObject parent, string name, string text = "Button")
        {
            UIButton uiButton = UIButton.CreateUIButton(parent);
            GameObject buttonGo = uiButton.gameObject;
            buttonGo.name = name;
#if dUI_TextMeshPro
            uiButton.TextMeshProLabel.text = text;
#endif
            return uiButton;
        }

        public static Image CreateImage(GameObject parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            GameObjectUtility.SetParentAndAlign(go, parent);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40f);
            go.GetComponent<Image>().sprite = WhiteSprite;
            go.GetComponent<Image>().raycastTarget = false;

            return go.GetComponent<Image>();
        }

        private static TextMeshProUGUI CreateTextMeshProUGUI(GameObject parent, string name = "Text (TMP)", string text = "Text")
        {
            GameObject textGo = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            GameObjectUtility.SetParentAndAlign(textGo, parent);
            textGo.GetComponent<RectTransform>().FitParent();
            textGo.GetComponent<TextMeshProUGUI>().text = text;
            textGo.GetComponent<TextMeshProUGUI>().fontSize = 24;
            textGo.GetComponent<TextMeshProUGUI>().raycastTarget = false;
            textGo.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Midline;

            return textGo.GetComponent<TextMeshProUGUI>();
        }

        private static Text CreateText(GameObject parent, string name = "Text", string text = "Text")
        {
            GameObject textGo = new GameObject(name, typeof(RectTransform), typeof(Text));
            GameObjectUtility.SetParentAndAlign(textGo, parent);
            textGo.GetComponent<RectTransform>().FitParent();
            textGo.GetComponent<Text>().text = text;
            textGo.GetComponent<Text>().fontSize = 18;
            textGo.GetComponent<Text>().raycastTarget = false;
            textGo.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            return textGo.GetComponent<Text>();
        }

        private static UIPrice CreateUIPrice(GameObject parent)
        {
            GameObject priceGo = new GameObject("UIPrice", typeof(RectTransform), typeof(UIPrice));
            GameObjectUtility.SetParentAndAlign(priceGo, parent);
            priceGo.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 40);

            Image icon = CreateImage(priceGo, "Icon");
            icon.GetComponent<RectTransform>().MidLeft(new Vector2(40, 40));
            icon.raycastTarget = false;
            icon.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);

            TextMeshProUGUI priceText = CreateTextMeshProUGUI(priceGo, "Price Text", "999");
            priceText.GetComponent<RectTransform>().MidRight(new Vector2(60, 40));

            priceGo.GetComponent<UIPrice>().priceText = priceText;
            priceGo.GetComponent<UIPrice>().icon = icon;
            return priceGo.GetComponent<UIPrice>();
        }

        private static UIReward CreateUIReward(GameObject parent)
        {
            GameObject rewardGo = new GameObject("UIReward", typeof(RectTransform), typeof(UIReward));
            GameObjectUtility.SetParentAndAlign(rewardGo, parent);
            rewardGo.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);

            GameObject iconGo = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            GameObjectUtility.SetParentAndAlign(iconGo, rewardGo);
            iconGo.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
            iconGo.GetComponent<Image>().sprite = WhiteSprite;
            iconGo.GetComponent<Image>().raycastTarget = false;

            TextMeshProUGUI amountText = CreateTextMeshProUGUI(rewardGo, "Amount Text", "99");
            amountText.GetComponent<RectTransform>().BottomCenter(new Vector2(40, 15));
            amountText.fontStyle = FontStyles.Bold;
            amountText.fontSize = 18;
            amountText.alignment = TextAlignmentOptions.Right;
            amountText.color = Color.black;

            rewardGo.GetComponent<UIReward>().amountText = amountText;
            rewardGo.GetComponent<UIReward>().icon = iconGo.GetComponent<Image>();
            return rewardGo.GetComponent<UIReward>();
        }

        private static UIProgressBar CreateUIProgressBar(GameObject parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(UIProgressBar));
            GameObjectUtility.SetParentAndAlign(go, parent);
            go.GetComponent<Image>().raycastTarget = false;
            go.GetComponent<Image>().sprite = WhiteSprite;
            go.GetComponent<Image>().color = ColorExt.Parse("#545454");

            Image imageGo = CreateImage(go, "Image");
            imageGo.GetComponent<RectTransform>().FitParent();
            imageGo.GetComponent<Image>().raycastTarget = false;
            imageGo.GetComponent<Image>().sprite = WhiteSprite;
            imageGo.GetComponent<Image>().type = Image.Type.Filled;
            imageGo.GetComponent<Image>().fillMethod = Image.FillMethod.Horizontal;
            imageGo.GetComponent<Image>().fillAmount = 0.8f;
            imageGo.GetComponent<Image>().color = Color.yellow;

            var progressBar = go.GetComponent<UIProgressBar>();
            progressBar.image = imageGo.GetComponent<Image>();

            return progressBar;
        }

        private static UIProgress CreateUIProgress(GameObject parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(UIProgress));
            GameObjectUtility.SetParentAndAlign(go, parent);

            var progressBar = CreateUIProgressBar(go, "Progress Bar");
            progressBar.GetComponent<RectTransform>().FitParent();

            var progressText = CreateTextMeshProUGUI(go, "Progress Text", "00/99");
            progressText.color = ColorExt.Parse("#737373");

            var questProgress = go.GetComponent<UIProgress>();
            questProgress.progressText = progressText;
            questProgress.progressBar = progressBar;

            return go.GetComponent<UIProgress>();
        }

        private static UIRewardDetail CreateUIRewardDetail(GameObject parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(UIRewardDetail));
            GameObjectUtility.SetParentAndAlign(go, parent);

            CreateUIReward(go);

            return go.GetComponent<UIRewardDetail>();
        }

        [MenuItem("GameObject/VGames/UIAchievement", false, 0)]
        private static void CreateAchievement(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UIAchievementItem", typeof(RectTransform), typeof(UIAchievement));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().MidCenter(new Vector2(560f, 200f));

            TextMeshProUGUI titleText = CreateTextMeshProUGUI(go, "Title Text", "Title");
            titleText.GetComponent<RectTransform>().TopLeft(new Vector2(400, 50));

            TextMeshProUGUI descText = CreateTextMeshProUGUI(go, "Description Text", "Description");
            descText.GetComponent<RectTransform>().MidLeft(new Vector2(400, 50f));

            Image icon = CreateImage(go, "Icon");
            icon.GetComponent<RectTransform>().MidLeft();

            UIButton claimButton = CreateUIButton(go, "Claim Button", "Claim");
            claimButton.GetComponent<RectTransform>().MidRight();

            var rewardContainer = CreateUIRewardDetail(go, "Reward Container");
            rewardContainer.GetComponent<RectTransform>().BottomRight(new Vector2(200, 50));

            var progress = CreateUIProgress(go, "Progress");
            progress.GetComponent<RectTransform>().BottomLeft(new Vector2(400, 50));

            go.GetComponent<UIAchievement>().titleText = titleText;
            go.GetComponent<UIAchievement>().descriptionText = descText;
            go.GetComponent<UIAchievement>().icon = icon;
            go.GetComponent<UIAchievement>().claimedText = claimButton.GetComponent<TextMeshProUGUI>();
            go.GetComponent<UIAchievement>().progress = progress;
            go.GetComponent<UIAchievement>().claimButton = claimButton;
            go.GetComponent<UIAchievement>().rewardContainer = rewardContainer;

            SetActiveAndUndo(go);
        }

        [MenuItem("GameObject/VGames/UIBattlePassLevel", false, 0)]
        private static void CreateBattle(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UIBattlePassLevel", typeof(RectTransform), typeof(UIBattlePassLevel));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().MidCenter(new Vector2(600, 160));

            UIBattlePassReward reward = CreateBattlePassReward(go, "Reward");
            reward.GetComponent<RectTransform>().MidCenter(new Vector2(300, 160), new Vector2(-150, 0));

            UIBattlePassReward premiumReward = CreateBattlePassReward(go, "Premium Reward");
            premiumReward.GetComponent<RectTransform>().MidCenter(new Vector2(300, 160), new Vector2(150, 0));

            TextMeshProUGUI levelText = CreateTextMeshProUGUI(go, "Level Text", "1");
            levelText.rectTransform.MidCenter(new Vector2(30, 30));

            UIBattlePassLevel battlePassLevel = go.GetComponent<UIBattlePassLevel>();
            battlePassLevel.levelText = levelText;
            battlePassLevel.reward = reward;
            battlePassLevel.premiumReward = premiumReward;

            SetActiveAndUndo(go);
        }

        private static UIBattlePassReward CreateBattlePassReward(GameObject go, string name)
        {
            UIBattlePassReward battlePassReward = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(UIBattlePassReward)).GetComponent<UIBattlePassReward>();
            GameObjectUtility.SetParentAndAlign(battlePassReward.gameObject, go);

            UIButton claimbutton = CreateUIButton(battlePassReward.gameObject, "Claim Button", "Claim");
            claimbutton.RectTransform.FitParent();

            UIReward reward = CreateUIReward(battlePassReward.gameObject);

            Image lockIcon = CreateImage(battlePassReward.gameObject, "Lock");
            lockIcon.rectTransform.MidCenter(Vector2.one * 30, new Vector2(0, -50));
            lockIcon.sprite = LockSprite;

            return battlePassReward;
        }

        [MenuItem("GameObject/VGames/UICurrency", false, 0)]
        private static void CreateCurrency(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UICurrency", typeof(RectTransform), typeof(UICurrency));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().MidCenter(new Vector2(100, 40));

            GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            GameObjectUtility.SetParentAndAlign(iconObj, go);
            iconObj.GetComponent<RectTransform>().MidLeft(new Vector2(40, 40));
            iconObj.GetComponent<Image>().sprite = WhiteSprite;
            iconObj.GetComponent<Image>().type = Image.Type.Simple;
            iconObj.GetComponent<Image>().raycastTarget = false;

            TextMeshProUGUI valueText = CreateTextMeshProUGUI(go, "Value Text", "9999");
            valueText.GetComponent<RectTransform>().MidRight(new Vector2(60, 40));
            valueText.text = "9999";
            valueText.alignment = TextAlignmentOptions.Midline;

            UICurrency uiCurrency = go.GetComponent<UICurrency>();
            uiCurrency.icon = iconObj.GetComponent<Image>();
            uiCurrency.valueText = valueText.GetComponent<TextMeshProUGUI>();

            SetActiveAndUndo(go);
        }

        [MenuItem("GameObject/VGames/UIIAPPackage", false, 0)]
        private static void CreateIAPPackage(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UIIAPPackage", typeof(RectTransform), typeof(Image), typeof(Button), typeof(UIButton), typeof(UIIAPPackage));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().MidCenter(new Vector2(180, 220));
            go.GetComponent<Image>().sprite = WhiteSprite;
            go.GetComponent<Image>().color = ColorExt.Parse("#009844");

            Image icon = CreateImage(go, "Icon");
            icon.GetComponent<RectTransform>().TopCenter(Vector2.one * 120, new Vector2(0, -20));

            TextMeshProUGUI priceText = CreateTextMeshProUGUI(go, "Price Text", "$0.99");
            priceText.GetComponent<RectTransform>().BottomStretch(30);

            GameObject rewardDetailGo = new GameObject("Rewards", typeof(RectTransform), typeof(UIRewardDetail));
            GameObjectUtility.SetParentAndAlign(rewardDetailGo, go);
            rewardDetailGo.GetComponent<RectTransform>().MidStretch(30, -50);

            UIReward reward = CreateUIReward(rewardDetailGo);
            reward.GetComponent<RectTransform>().MidStretch(30);
            reward.icon.rectTransform.MidCenter(Vector2.one * 30, new Vector3(-36, 0));
            reward.amountText.rectTransform.FitParent();
            reward.amountText.fontSize = 24;
            reward.amountText.alignment = TextAlignmentOptions.Midline;

            go.GetComponent<UIIAPPackage>().icon = icon;
            go.GetComponent<UIIAPPackage>().rewards = rewardDetailGo.GetComponent<UIRewardDetail>();
            go.GetComponent<UIIAPPackage>().purchaseButton = go.GetComponent<UIButton>();

            SetActiveAndUndo(go);
        }

        [MenuItem("GameObject/VGames/UILuckyBox", false, 0)]
        private static void CreateLuckyBox(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UILuckyBox", typeof(RectTransform), typeof(UILuckyBox));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().MidCenter(new Vector2(180, 220));

            GameObject closeNode = new GameObject("Close Node", typeof(RectTransform), typeof(Image));
            GameObjectUtility.SetParentAndAlign(closeNode, go);
            closeNode.GetComponent<RectTransform>().FitParent();
            closeNode.GetComponent<Image>().color = ColorExt.Parse("#C2C2C2");

            UIButton openButton = CreateUIButton(closeNode, "Open Button");
            openButton.GetComponent<RectTransform>().FitParent();

            GameObject openedNode = new GameObject("Opened Node", typeof(RectTransform), typeof(Image));
            GameObjectUtility.SetParentAndAlign(openedNode, go);
            openedNode.GetComponent<RectTransform>().FitParent();

            UIRewardDetail reward = CreateUIRewardDetail(openedNode, "Reward Container");
            reward.GetComponent<RectTransform>().MidCenter(new Vector2(100, 100));

            UILuckyBox luckyBox = go.GetComponent<UILuckyBox>();
            luckyBox.openButton = openButton;
            luckyBox.reward = reward;
            luckyBox.openedNode = openedNode;
            luckyBox.closeNode = closeNode;
            SetActiveAndUndo(go);
        }

        [MenuItem("GameObject/VGames/UIOnlineReward", false, 0)]
        private static void CreateOnlineReward(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UIOnlineReward", typeof(RectTransform), typeof(Image), typeof(UIOnlineReward));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().MidCenter(new Vector2(120, 70));
            go.GetComponent<Image>().color = ColorExt.Parse("#2D4082");

            TextMeshProUGUI minuteText = CreateTextMeshProUGUI(go, "Minute Text", "00");
            minuteText.GetComponent<RectTransform>().TopCenter(new Vector2(120, 30));
            minuteText.fontSize = 18;

            UIReward reward = CreateUIReward(go);
            reward.GetComponent<RectTransform>().BottomCenter(new Vector2(120, 40));

            go.GetComponent<UIOnlineReward>().minuteText = minuteText;
            go.GetComponent<UIOnlineReward>().reward = reward;

            SetActiveAndUndo(go);
        }

        [MenuItem("GameObject/VGames/UIPages", false, 0)]
        private static void CreateUIPages(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UIPages", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(UIPages));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().FitParent();
            go.GetComponent<RectTransform>().offsetMax = new Vector2(0f, -250f);
            go.GetComponent<RectTransform>().offsetMin = Vector2.zero;

            UIPages pages = go.GetComponent<UIPages>();
            GameObject labelContaingerGo = new GameObject("Labels", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(labelContaingerGo, go);
            labelContaingerGo.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, 80f);
            labelContaingerGo.AddComponent<HorizontalLayoutGroup>();
            labelContaingerGo.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
            labelContaingerGo.GetComponent<HorizontalLayoutGroup>().childControlHeight = true;
            labelContaingerGo.AddComponent<LayoutElement>();
            labelContaingerGo.GetComponent<LayoutElement>().preferredHeight = 80f;
            labelContaingerGo.GetComponent<LayoutElement>().flexibleHeight = 0f;

            GameObject contentContaingerGo = new GameObject("Contents", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(contentContaingerGo, go);
            contentContaingerGo.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, 540f);
            contentContaingerGo.AddComponent<HorizontalLayoutGroup>();
            contentContaingerGo.AddComponent<LayoutElement>();
            contentContaingerGo.GetComponent<LayoutElement>().preferredHeight = 540f;
            contentContaingerGo.GetComponent<LayoutElement>().flexibleHeight = 1f;

            //pages.pageSwiper = contentContaingerGo.GetComponent<UIPageSwiper>();
            pages.labelContainer = labelContaingerGo.GetComponent<RectTransform>();
            pages.contentContainer = contentContaingerGo.GetComponent<RectTransform>();

            SetActiveAndUndo(go);
        }

        [MenuItem("GameObject/VGames/UIPrice", false, 0)]
        private static void CreatePrice(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            UIPrice uiPrice = CreateUIPrice(targetParent);

            Selection.activeObject = uiPrice;
        }

        [MenuItem("GameObject/VGames/UIProgressBar", false, 0)]
        private static void CreateProgressBar(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            UIProgressBar uiProgressBar = CreateUIProgressBar(targetParent, "Progress Bar");
            uiProgressBar.GetComponent<RectTransform>().MidCenter(new Vector2(100, 20));

            Selection.activeObject = uiProgressBar;
        }

        [MenuItem("GameObject/VGames/UIQuest", false, 0)]
        private static void CreateQuest(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UIQuest", typeof(RectTransform), typeof(Image), typeof(UIQuest));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().MidCenter(new Vector2(550, 120));
            go.GetComponent<Image>().color = ColorExt.Parse("#2D4082");

            TextMeshProUGUI descText = CreateTextMeshProUGUI(go, "Description Text", "Description");
            descText.GetComponent<RectTransform>().TopCenter(new Vector2(400, 30), new Vector2(-65, -5));
            descText.alignment = TextAlignmentOptions.Left;

            //UIProgress questProgress = CreateUIProgress(go, "Quest Progress");
            //questProgress.GetComponent<RectTransform>().MidCenter(new Vector2(400, 30), new Vector2(-65, 5));

            UIRewardDetail rewardContainer = CreateUIRewardDetail(go, "Reward Container");
            rewardContainer.GetComponent<RectTransform>().BottomCenter(new Vector2(400, 40), new Vector2(-65, 5));

            UIButton claimButton = CreateUIButton(go, "Claim Button", "Claim");
            claimButton.GetComponent<RectTransform>().MidRight(new Vector2(120, 50), new Vector2(-5, 0));
            claimButton.Button.image.color = ColorExt.Parse("#BD75FF");
            claimButton.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

            go.GetComponent<UIQuest>().descriptionText = descText;
            //go.GetComponent<UIQuest>().progress = questProgress;
            go.GetComponent<UIQuest>().rewardsContainer = rewardContainer;
            go.GetComponent<UIQuest>().claimButton = claimButton;

            SetActiveAndUndo(go);
        }

        [MenuItem("GameObject/VGames/UIReward", false, 0)]
        private static void CreateReward(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            UIReward uiReward = CreateUIReward(targetParent);

            Selection.activeObject = uiReward;
        }

        [MenuItem("GameObject/VGames/UIShopItem", false, 0)]
        private static void CreateShopItem(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UIShopItem", typeof(RectTransform), typeof(UIShopItem));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().MidCenter(new Vector2(180, 220));

            TextMeshProUGUI nameText = CreateTextMeshProUGUI(go, "Name Text", "Item name");
            nameText.GetComponent<RectTransform>().TopStretch(30);

            Image icon = CreateImage(go, "Icon");
            icon.GetComponent<RectTransform>().TopCenter(Vector2.one * 120, new Vector2(0, -30));

            TextMeshProUGUI valueText = CreateTextMeshProUGUI(icon.gameObject, "Value Text", "x10");
            valueText.GetComponent<RectTransform>().BottomRight(new Vector2(60, 20));
            valueText.fontSize = 18;
            valueText.color = Color.black;
            valueText.alignment = TextAlignmentOptions.BottomRight;

            UIButton buyButton = CreateUIButton(go, "Buy Button");
#if dUI_TextMeshPro
            buyButton.TextMeshProLabel.Destroy();
#endif
            buyButton.RectTransform.BottomCenter(new Vector2(160, 50), new Vector2(0, 10));

            UIPrice uiPrice = CreateUIPrice(buyButton.gameObject);
            uiPrice.GetComponent<RectTransform>().MidCenter();
            uiPrice.priceText.color = Color.black;

            go.GetComponent<UIShopItem>().price = uiPrice;
            go.GetComponent<UIShopItem>().nameText = nameText;
            go.GetComponent<UIShopItem>().valueText = valueText;
            go.GetComponent<UIShopItem>().icon = icon;
            go.GetComponent<UIShopItem>().button = buyButton;

            SetActiveAndUndo(go);
        }

        [MenuItem("GameObject/VGames/UISpinItem", false, 0)]
        private static void CreateUISpinItem(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UISpinItem", typeof(RectTransform), typeof(UISpinItem));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().MidCenter();

            UIRewardDetail reward = CreateUIRewardDetail(go, "Rewards");

            UISpinItem spinItem = go.GetComponent<UISpinItem>();
            spinItem.reward = reward;
            SetActiveAndUndo(go);
        }

        [MenuItem("GameObject/VGames/UITab", false, 0)]
        private static void CreateTab(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UITab", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(UITab));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().FitParent();
            go.GetComponent<RectTransform>().offsetMax = new Vector2(0f, -250f);
            go.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            go.GetComponent<VerticalLayoutGroup>().childControlWidth = true;
            go.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
            go.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = false;

            UITab tab = go.GetComponent<UITab>();
            GameObject labelContaingerGo = new GameObject("Labels", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(labelContaingerGo, go);
            labelContaingerGo.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, 80f);
            labelContaingerGo.AddComponent<HorizontalLayoutGroup>();
            labelContaingerGo.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
            labelContaingerGo.GetComponent<HorizontalLayoutGroup>().childControlHeight = true;
            labelContaingerGo.AddComponent<LayoutElement>();
            labelContaingerGo.GetComponent<LayoutElement>().preferredHeight = 80f;
            labelContaingerGo.GetComponent<LayoutElement>().flexibleHeight = 0f;

            GameObject contentContaingerGo = new GameObject("Contents", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(contentContaingerGo, go);
            contentContaingerGo.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, 540f);
            contentContaingerGo.AddComponent<LayoutElement>();
            contentContaingerGo.GetComponent<LayoutElement>().preferredHeight = 540f;
            contentContaingerGo.GetComponent<LayoutElement>().flexibleHeight = 1f;

            tab.labelContainer = labelContaingerGo.GetComponent<RectTransform>();
            tab.contentContainer = contentContaingerGo.GetComponent<RectTransform>();

            SetActiveAndUndo(go);
        }

        #region UI Sfx/Music
        [MenuItem("GameObject/VGames/UIMusicToggle", false, 1)]
        private static void CreateMusicToggle(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UIMusicToggle", typeof(RectTransform), typeof(Image), typeof(UIMusicToggle));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().MidCenter(new Vector2(50f, 50f));
            go.GetComponent<Image>().SetSpriteDefault();
            go.GetComponent<Toggle>().targetGraphic = go.GetComponent<Image>();

            SetActiveAndUndo(go);
        }

        [MenuItem("GameObject/VGames/UISoundToggle", false, 1)]
        private static void CreateSoundToggle(MenuCommand menuCommand)
        {
            GameObject targetParent = null;
            GameObject selectedGO = menuCommand.context as GameObject;
            if(selectedGO != null) targetParent = selectedGO;

            GameObject go = new GameObject("UISoundToggle", typeof(RectTransform), typeof(Image), typeof(UISoundToggle));
            GameObjectUtility.SetParentAndAlign(go, targetParent);
            go.GetComponent<RectTransform>().MidCenter(new Vector2(50f, 50f));
            go.GetComponent<Image>().SetSpriteDefault();
            go.GetComponent<Toggle>().targetGraphic = go.GetComponent<Image>();

            SetActiveAndUndo(go);
        }
        #endregion
    }
}