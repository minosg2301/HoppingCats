using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    public class GameCoreWindow : BaseEditorWindow
    {
        public static GameCoreWindow Ins { get; private set; }

        private TabContainer tabContainer;

        [MenuItem("EditorSettings/Core Setting &c", false, 1)]
        private static void Init()
        {
            OpenWindow();
        }

        public static void OpenWindow()
        {
            Ins = (GameCoreWindow)GetWindow(typeof(GameCoreWindow), false, "Core Setting");
            Ins.autoRepaintOnSceneChange = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Ins = this;

            tabContainer = new TabContainer();
            tabContainer.AddTab("Global Config", new GlobalConfigTab());
            tabContainer.AddTab("User Property", new UserPropertyAssetTab());
            tabContainer.AddTab("Unlock Content", new UnlockContentTab());
            tabContainer.AddTab("Unit", new GameUnitTab());
            tabContainer.AddTab("Item", new ItemAssetTab());
            tabContainer.AddTab("Shop", new ShopAssetTab());
            tabContainer.AddTab("Chest", new ChestTab());
            tabContainer.AddTab("Layer", new LayerAssetTab());
            tabContainer.AddTab("Features", new FeatureTab());
            tabContainer.AddTab("Arena", new ArenaAssetTab());
            tabContainer.AddTab("IAP Offer", new IAPAssetTab());
            tabContainer.AddTab("Tutorial", new TutorialTab());
            tabContainer.AddTab("Invite Friend", new InviteFriendTab());
        }

        protected override Object GetTarget() => null;

        protected override void OnDraw()
        {
            tabContainer.SelectedItem.DoDrawContent();
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