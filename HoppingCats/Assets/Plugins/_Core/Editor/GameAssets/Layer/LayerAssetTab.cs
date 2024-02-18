using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{

    public class LayerAssetTab : TabContent
    {
        const string Layers = "Layers";

        TabContainer tab;
        List<LayerGroupTab> layerGroupTabs = new List<LayerGroupTab>();

        private bool
            haveOverrideOnlineReward,
            haveQuest,
            haveShop,
            haveChest,
            haveIAPOffer,
            haveBattlePass;

        public LayerAssetTab()
        {
            LayerAsset.Ins.onOverrideUpdated += OnOverrideUpdated;
        }

        public override void OnFocused()
        {
            base.OnFocused();

            tab = new TabContainer();
            foreach(var group in LayerAsset.Ins.Groups)
            {
                LayerGroupTab layerGroupTab = new LayerGroupTab(group);
                layerGroupTabs.Add(layerGroupTab);
                tab.AddTab(group.name, layerGroupTab);
            }

            UpdateOverride();
        }

        public override void DoDraw()
        {
            Undo.RecordObject(LayerAsset.Ins, Layers);
            tab.DoDraw();
            if(GUI.changed) Draw.SetDirty(LayerAsset.Ins);
        }

        private void OnOverrideUpdated(LayerGroup layerGroup)
        {
            UpdateOverride();
        }

        private void UpdateOverride()
        {
            haveOverrideOnlineReward = haveChest = haveQuest = haveShop = haveIAPOffer = haveBattlePass = false;
            LayerAsset.Ins.Groups.ForEach(group =>
            {
                haveOverrideOnlineReward |= group.overrideOnlineReward;
                haveChest |= group.overrideChest;
                haveQuest |= group.overrideQuest;
                haveShop |= group.overrideShop;
                haveIAPOffer |= group.overrideIAPOffer;
                haveBattlePass |= group.overrideBattlePass;
            });
            foreach(var groupTab in layerGroupTabs)
            {
                groupTab.settingTab.disableOnlineReward = haveOverrideOnlineReward && !groupTab.layerGroup.overrideOnlineReward;
                groupTab.settingTab.disableChest = haveChest && !groupTab.layerGroup.overrideChest;
                groupTab.settingTab.disableQuest = haveQuest && !groupTab.layerGroup.overrideQuest;
                groupTab.settingTab.disableShop = haveShop && !groupTab.layerGroup.overrideShop;
                groupTab.settingTab.disableIAPOffer = haveIAPOffer && !groupTab.layerGroup.overrideIAPOffer;
                groupTab.settingTab.disableBattlePass = haveBattlePass && !groupTab.layerGroup.overrideBattlePass;
            }
        }
    }
}