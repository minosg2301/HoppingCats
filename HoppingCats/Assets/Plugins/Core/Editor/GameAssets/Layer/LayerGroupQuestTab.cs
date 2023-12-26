using System.Collections.Generic;
using moonNest;

public class LayerGroupQuestTab : TabContent
{
    private readonly LayerGroup layerGroup;
    private readonly LayerDetailTabDrawer layerTab;

    public LayerGroupQuestTab(LayerGroup group, int questGroupId)
    {
        this.layerGroup = group;

        layerTab = new LayerDetailTabDrawer(questGroupId);
        layerTab.DrawAddButton = false;
        layerTab.DrawControl = false;
        layerTab.HeaderType = HeaderType.Vertical;
    }

    public override void DoDraw()
    {
        layerTab.DoDraw(LayerAsset.Ins.FindByGroup(layerGroup.id));
    }

    private class LayerDetailTabDrawer : BaseLayerTabDrawer
    {
        private readonly int questGroupId;
        private readonly QuestListTabDrawer questTab;

        private LayerDetail lastLayer;
        private List<QuestLayer> questLayers;

        public LayerDetailTabDrawer(int questGroupId)
        {
            this.questGroupId = questGroupId;
            this.questTab = new QuestListTabDrawer();
        }

        protected override void DoDrawContent(LayerDetail layer)
        {
            if(layer != lastLayer)
            {
                lastLayer = layer;

                // update name from quest asset
                var quests = QuestAsset.Ins.FindByGroup(questGroupId);
                questLayers = layer.questGroups.Find(q => q.groupId == questGroupId).quests;
                questLayers.ForEach(questLayer =>
                {
                    var quest = quests.Find(q => q.id == questLayer.questId);
                    questLayer.name = quest.name;
                });
            }

            questTab.DoDraw(questLayers);
        }
    }

    private class QuestListTabDrawer : ListTabDrawer<QuestLayer>
    {
        readonly RewardDrawer rewardDrawer;

        public QuestListTabDrawer()
        {
            KeepCurrentTabIndex = true;
            DrawAddButton = false;
            rewardDrawer = new RewardDrawer();
        }

        protected override QuestLayer CreateNewElement() => null;

        protected override string GetTabLabel(QuestLayer questLayer) => questLayer.name;

        protected override void DoDrawContent(QuestLayer questLayer)
        {
            rewardDrawer.DoDraw(questLayer.reward);
        }
    }
}