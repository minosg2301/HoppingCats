using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    [CustomEditor(typeof(StatProgressAsset))]
    public class StatProgressAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) StatProgressAsset.Reload();
        }
    }

    [CustomEditor(typeof(UnlockContentAsset))]
    public class UnlockContentAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) UnlockContentAsset.Reload();
        }
    }

    [CustomEditor(typeof(ArenaAsset))]
    public class BattlePassAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) ArenaAsset.Reload();
        }
    }

    [CustomEditor(typeof(IAPPackageAsset))]
    public class IAPPackageAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) IAPPackageAsset.Reload();
        }
    }

    [CustomEditor(typeof(AchievementAsset))]
    public class AchievementAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) AchievementAsset.Reload();
        }
    }

    [CustomEditor(typeof(ChestAsset))]
    public class ChestAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) ChestAsset.Reload();
        }
    }

    [CustomEditor(typeof(ShopAsset))]
    public class ShopAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) ShopAsset.Reload();
        }
    }

    [CustomEditor(typeof(OnlineRewardAsset))]
    public class OnlineRewardAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) OnlineRewardAsset.Reload();
        }
    }

    [CustomEditor(typeof(GlobalConfig))]
    public class GlobalConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) GlobalConfig.Reload();
        }
    }

    [CustomEditor(typeof(Profiles))]
    public class ProfilesEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if (GUILayout.Button("Reload")) Profiles.Reload();
        }
    }

    [CustomEditor(typeof(ItemAsset))]
    public class InventoryItemAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) ItemAsset.Reload();
        }
    }

    [CustomEditor(typeof(QuestAsset))]
    public class QuestAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) QuestAsset.Reload();
        }
    }

    [CustomEditor(typeof(GameDefinitionAsset))]
    public class GameDefinitionAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) GameDefinitionAsset.Reload();
        }
    }

    [CustomEditor(typeof(UserPropertyAsset))]
    public class UserPropertyAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) UserPropertyAsset.Reload();
        }
    }

    [CustomEditor(typeof(LayerAsset))]
    public class LayerAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) LayerAsset.Reload();
        }
    }

    [CustomEditor(typeof(GatchaAsset))]
    public class GatchaAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")) GameCoreWindow.OpenWindow();
            if(GUILayout.Button("Reload")) LayerAsset.Reload();
        }
    }
}