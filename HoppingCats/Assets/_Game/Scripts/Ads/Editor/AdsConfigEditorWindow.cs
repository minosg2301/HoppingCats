using UnityEditor;
using UnityEngine;
using moonNest;
using Object = UnityEngine.Object;

public class AdsConfigEditorWindow : BaseEditorWindow
{
    #region static
    public static AdsConfigEditorWindow Ins { get; private set; }

    [MenuItem("EditorSettings/Ads Setting &a", false, 4)]
    private static void OnMenuItemClicked()
    {
        //GameConfig.CreateAsset(typeof(GameConfig), "GameConfig.asset");
        OpenWindow();
    }

    public static void OpenWindow()
    {
        Ins = (AdsConfigEditorWindow)GetWindow(typeof(AdsConfigEditorWindow), false, "Admod Setting");
        Ins.autoRepaintOnSceneChange = true;
    }
    #endregion

    #region template
    protected TabContainer tabContainer;
    protected AdsConfig admodConfig;

    protected override Object GetTarget() => AdsConfig.Ins;

    protected override void OnEnable()
    {
        base.OnEnable();
        Ins = this;
        admodConfig = AdsConfig.Ins;
        tabContainer = new TabContainer();
        _Init();
    }

    protected override void OnDraw()
    {
        Undo.RecordObject(admodConfig, "Admod Config");

        _OnDraw();

        if (GUI.changed)
            EditorUtility.SetDirty(AdsConfig.Ins);
    }
    #endregion

    private void _Init()
    {
        // create view here
        tabContainer = new TabContainer();
        tabContainer.AddTab("Admod", new AdmodTab());
    }

    private void _OnDraw()
    {
        tabContainer.DoDraw();

        // customize draw here
    }
}