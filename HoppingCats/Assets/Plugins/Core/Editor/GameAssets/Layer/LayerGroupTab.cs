using UnityEngine;

namespace moonNest.editor
{
    public class LayerGroupTab : TabContent
    {
        const string layerGroupTitle = "LAYER GROUP require value of '{0}' USER PROPERTY";

        private TabContainer tabContainer;

        public LayerGroup layerGroup;
        public LayerGroupSettingTab settingTab;

        public LayerGroupTab(LayerGroup group)
        {
            layerGroup = group;
            settingTab = new LayerGroupSettingTab(group);

            UpdateTabContainer();

            LayerAsset.Ins.onOverrideUpdated += OnOverrideUpdated;
        }

        private void OnOverrideUpdated(LayerGroup updatedGroup)
        {
            if(layerGroup == updatedGroup)
            {
                UpdateTabContainer();
            }
        }

        public void UpdateTabContainer()
        {
            tabContainer = new TabContainer();
            tabContainer.AddTab("Setting", settingTab);
            if(layerGroup.overrideChest) tabContainer.AddTab("Chest", new LayerGroupChestTab(layerGroup));
            if(layerGroup.overrideOnlineReward) tabContainer.AddTab("Online Reward", new LayerOnlineRewardTab(layerGroup));
            if(layerGroup.overrideShop)
            {
                layerGroup.shopIds.ForEach(shopId =>
                {
                    ShopDetail shop = ShopAsset.Ins.FindGroup(shopId);
                    if(shop) tabContainer.AddTab(shop.name, new LayerGroupShopTab(layerGroup, shopId));
                });
            }
            if(layerGroup.overrideQuest)
            {
                layerGroup.questGroupIds.ForEach(questGroupId =>
                {
                    QuestGroupDetail questGroup = QuestAsset.Ins.FindGroup(questGroupId);
                    if(questGroup) tabContainer.AddTab(questGroup.name, new LayerGroupQuestTab(layerGroup, questGroupId));
                });
            }
            if(layerGroup.overrideIAPOffer)
            {
                layerGroup.packageGroupIds.ForEach(groupId =>
                {
                    IAPPackageGroup packageGroup = IAPPackageAsset.Ins.FindGroup(groupId);
                    if(packageGroup) tabContainer.AddTab(packageGroup.name, new LayerGroupIAPTab(layerGroup, groupId));
                });
            }
            if(layerGroup.overrideBattlePass)
            {
                tabContainer.AddTab("Battle Pass", new LayerGroupArenaTab(layerGroup));
            }
        }

        public override void DoDraw()
        {
            Draw.LabelBoldBox(string.Format(layerGroupTitle, layerGroup.name), Color.blue);
            tabContainer.DoDraw();
        }
    }
}