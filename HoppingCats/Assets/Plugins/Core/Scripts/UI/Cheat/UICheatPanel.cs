#if ENABLE_CHEAT
using BayatGames.SaveGameFree;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using vgame;
using vgame.remotedata;
using static UnityEngine.UI.CanvasScaler;

public class UICheatPanel
{
    const int buttonSize = 30;
    const int clickCount = 1;

    private static int tlCount, trCount;
    private static GameObject tlBtn;
    private static GameObject trBtn;
    private static GameObject container;
    private static GameObject closeBtn;

    private static Button deleteRemoteDataBtn;

    internal static void Create()
    {
        var canvasGO = new GameObject("Cheat Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(DontDestroy));
        canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.GetComponent<CanvasScaler>().screenMatchMode = ScreenMatchMode.MatchWidthOrHeight;
        canvasGO.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.GetComponent<CanvasScaler>().referenceResolution = Screen.height > Screen.width ? new Vector2(750, 1334) : new Vector2(1334, 750);

        tlBtn = CreateButton("Top-Left Button", canvasGO, OnTopLeftClick);
        tlBtn.GetComponent<RectTransform>().TopLeft(Vector2.one * buttonSize);
        tlBtn.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        trBtn = CreateButton("Top-Right Button", canvasGO, OnTopRightClick);
        trBtn.GetComponent<RectTransform>().TopRight(Vector2.one * buttonSize);
        trBtn.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        container = new GameObject("Container", typeof(RectTransform), typeof(Image));
        container.transform.SetParent(canvasGO.transform);
        container.gameObject.SetActive(false);
        container.GetComponent<RectTransform>().FitParent();
        container.GetComponent<Image>().color = Color.gray;

        var deleteRemoteDataBtnGo = CreateButton("Delete Button", container, DeleteRemoteData, "Delete Remote Data");
        deleteRemoteDataBtnGo.GetComponent<RectTransform>().MidCenter(new Vector2(250, 50));
        deleteRemoteDataBtn = deleteRemoteDataBtnGo.GetComponent<Button>();
        deleteRemoteDataBtn.transition = Selectable.Transition.ColorTint;

        closeBtn = CreateButton("Close Button", container, Close, "Close");
        closeBtn.GetComponent<RectTransform>().BottomCenter(new Vector2(250, 50));
    }

    private static GameObject CreateButton(string name, GameObject canvasGO, UnityAction onClick, string title = default)
    {
        var btn = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        btn.transform.SetParent(canvasGO.transform);
        btn.GetComponent<Button>().transition = Selectable.Transition.None;
        btn.GetComponent<Button>().onClick.AddListener(onClick);
        btn.GetComponent<Image>().color = Color.white;

        if(!string.IsNullOrEmpty(title))
        {
            var text = new GameObject("Text", typeof(RectTransform), typeof(Text));
            text.transform.SetParent(btn.transform);
            text.GetComponent<RectTransform>().FitParent();
            text.GetComponent<Text>().text = title;
            text.GetComponent<Text>().fontSize = 24;
            text.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.GetComponent<Text>().color = Color.black;
            text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        }

        return btn;
    }

    static void OnTopLeftClick()
    {
        tlCount = Mathf.Min(clickCount, tlCount + 1);
        tlBtn.GetComponent<Image>().color = new Color(1, 1, 1, (float)tlCount / clickCount);
        if(tlCount == clickCount && trCount == clickCount)
            Show();
    }

    static void OnTopRightClick()
    {
        trCount = Mathf.Min(clickCount, trCount + 1);
        trBtn.GetComponent<Image>().color = new Color(1, 1, 1, (float)trCount / clickCount);
        if(tlCount == clickCount && trCount == clickCount)
            Show();
    }

    private static void Show()
    {
        trCount = tlCount = 0;
        tlBtn.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        trBtn.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        container.SetActive(true);
    }

    private static void Close()
    {
        container.SetActive(false);
    }

    private static async void DeleteRemoteData()
    {
        deleteRemoteDataBtn.interactable = false;
        await RemoteDataManager.DeleteAll();
        LocalData.allowSave = false;
        SaveGame.DeleteAll();
        PlayerPrefs.DeleteAll();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

#else
public class UICheatPanel
{
    internal static void Create(){}
}
#endif