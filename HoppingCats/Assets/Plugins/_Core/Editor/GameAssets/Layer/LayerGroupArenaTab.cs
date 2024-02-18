using System.Collections.Generic;
using moonNest;

public class LayerGroupArenaTab : TabContent
{
    private readonly LayerGroup layerGroup;
    private readonly LayerDetailTabDrawer layerTab;

    public LayerGroupArenaTab(LayerGroup group)
    {
        layerGroup = group;

        layerTab = new LayerDetailTabDrawer();
        layerTab.DrawAddButton = false;
        layerTab.DrawControl = false;
        layerTab.HeaderType = HeaderType.Vertical;
    }

    public override void DoDraw()
    {
        layerTab.DoDraw(LayerAsset.Ins.FindByGroup(layerGroup.id));
    }

    class LayerDetailTabDrawer : BaseLayerTabDrawer
    {
        readonly LevelListTabDrawer levelListTabDrawer;
        LayerDetail lastLayer;
        List<LayerDetail> LinkableLayers;

        public LayerDetailTabDrawer()
        {
            levelListTabDrawer = new LevelListTabDrawer();
        }

        protected override void DoDrawContent(LayerDetail layer)
        {
            if(layer != lastLayer)
            {
                lastLayer = layer;

                // update LinkableLayers
                LinkableLayers = List.FindAll(l => l.id != lastLayer.id && l.arenaLinkedLayer == -1);
            }

            Draw.BeginChangeCheck();
            int lastLinkedLayer = layer.arenaLinkedLayer;
            layer.arenaLinkedLayer = Draw.IntPopupField("Linked With Layer", layer.arenaLinkedLayer, LinkableLayers, "name", "id", 120);
            if(Draw.EndChangeCheck())
            {
                if(layer.arenaLinkedLayer == -1 && lastLinkedLayer != -1)
                {
                    LayerDetail linkedLayer = LinkableLayers.Find(l => l.id == lastLinkedLayer);
                    if(linkedLayer)
                    {
                        layer.battlePassLevels = linkedLayer.battlePassLevels.Map(o => o.Clone() as BattlePassLevelLayer);
                    }
                }
                else if(layer.arenaLinkedLayer != -1)
                {
                    layer.battlePassLevels.Clear();
                }
            }

            if(layer.arenaLinkedLayer == -1)
                levelListTabDrawer.DoDraw(lastLayer.battlePassLevels);
        }

        protected override int GetLabelFontSize(LayerDetail layer)
            => layer.arenaLinkedLayer == -1 ? base.GetLabelFontSize(layer) : 8;
    }

    class LevelListTabDrawer : ListTabDrawer<BattlePassLevelLayer>
    {
        readonly RewardDrawer rewardDrawer = new RewardDrawer("Reward");
        readonly RewardDrawer premiumRewardDrawer = new RewardDrawer("Premium Reward");

        public LevelListTabDrawer()
        {
            rewardDrawer.DrawOnce = true;
            premiumRewardDrawer.DrawOnce = true;
            KeepCurrentTabIndex = true;
        }

        protected override BattlePassLevelLayer CreateNewElement() => null;

        protected override string GetTabLabel(BattlePassLevelLayer element) => element.level.ToString();

        protected override void DoDrawContent(BattlePassLevelLayer element)
        {
            Draw.BeginHorizontal();
            {
                Draw.BeginVertical(Draw.BoxStyle);
                rewardDrawer.DoDraw(element.reward);
                Draw.EndVertical();

                Draw.BeginVertical(Draw.BoxStyle);
                premiumRewardDrawer.DoDraw(element.premiumReward);
                Draw.EndVertical();

                Draw.FlexibleSpace();
            }
            Draw.EndHorizontal();
        }
    }
}