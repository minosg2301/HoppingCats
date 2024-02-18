using UnityEditor;
using UnityEngine;

namespace moonNest.ads
{
    [CustomEditor(typeof(ThirdPartyConfig))]
    public class ThirdPartySettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor")) ThirdPartyWindow.OpenWindow();
            if (GUILayout.Button("Reload")) ThirdPartyConfig.Reload();
        }
    }

    public class ThirdPartyWindow : BaseEditorWindow
    {
        public static ThirdPartyWindow Ins { get; private set; }

        private TabContainer tabContainer;

        [MenuItem("EditorSettings/Third Party &T", false, 2)]
        private static void Init()
        {
            OpenWindow();
        }

        public static void OpenWindow()
        {
            Ins = (ThirdPartyWindow)GetWindow(typeof(ThirdPartyWindow), false, "Third Party");
            Ins.autoRepaintOnSceneChange = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Ins = this;

            tabContainer = new TabContainer();
            tabContainer.AddTab("Ads", new AdsTab());
            tabContainer.AddTab("Tracking", new TrackingTab());
            tabContainer.AddTab("Firebase", new FirebaseTab());
            tabContainer.AddTab("Notifications", new PushNotificationTab());
            tabContainer.AddTab("Define Symbols", new DefineSymbolsTab());
        }

        protected override Object GetTarget() => null;

        protected override void OnDraw()
        {
            Undo.RecordObject(ThirdPartyConfig.Ins, "Third Party");
            tabContainer.SelectedItem.DoDrawContent();
            if (GUI.changed) Draw.SetDirty(ThirdPartyConfig.Ins);
        }

        protected override bool OnDrawWindow() => tabContainer.DoDrawWindow();

        protected override void OnDrawToolbar()
        {
            float oldValue = EditorStyles.toolbar.fixedHeight;
            EditorStyles.toolbar.fixedHeight = 32;
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            tabContainer.DoDrawAsToolbar();
            GUILayout.EndHorizontal();
            EditorStyles.toolbar.fixedHeight = oldValue;
        }
    }
}