using System.Collections.Generic;
using moonNest;

public class LayerOnlineRewardTab : TabContent
{
    private readonly LayerGroup layerGroup;
    private readonly LayerDetailTabDrawer tabDrawer;

    public LayerOnlineRewardTab(LayerGroup group)
    {
        layerGroup = group;
        tabDrawer = new LayerDetailTabDrawer();
        tabDrawer.DrawAddButton = false;
        tabDrawer.DrawControl = false;
        tabDrawer.HeaderType = HeaderType.Vertical;
    }

    public override void DoDraw()
    {
        tabDrawer.DoDraw(LayerAsset.Ins.FindByGroup(layerGroup.id));
    }

    class LayerDetailTabDrawer : BaseLayerTabDrawer
    {
        readonly List<OnlineRewardDetail> onlineRewards;
        readonly OnlineRewardLayerTabDrawer tabDrawer;

        Dictionary<int, OnlineRewardDetail> onlineRewardMap = new Dictionary<int, OnlineRewardDetail>();
        LayerDetail lastLayer;
        List<LayerDetail> Layers;

        public LayerDetailTabDrawer()
        {
            onlineRewards = OnlineRewardAsset.Ins.onlineRewards;
            tabDrawer = new OnlineRewardLayerTabDrawer();
        }

        protected override void DoDrawContent(LayerDetail layer)
        {
            if(layer != lastLayer)
            {
                Layers = List.FindAll(l => l.id != layer.id);
                lastLayer = layer;
                onlineRewardMap = onlineRewards.ToMap(o => o.id);
                UpdateOnlineRewardName(layer);
            }
            /*
            Draw.BeginChangeCheck();
            int lastLinkedLayer = layer.onlineRewardLinkedLayer;
            layer.onlineRewardLinkedLayer = Draw.IntPopupField("Linked With Layer", layer.onlineRewardLinkedLayer, Layers, "name", "id", 120);
            if(Draw.EndChangeCheck())
            {
                if(layer.onlineRewardLinkedLayer == -1 && lastLinkedLayer != -1)
                {
                    LayerDetail linkedLayer = Layers.Find(l => l.id == lastLinkedLayer);
                    if(linkedLayer)
                    {
                        layer.onlineRewards = linkedLayer.onlineRewards.Map(o => o.Clone() as OnlineRewardLayer);
                        UpdateOnlineRewardName(layer);
                    }
                }
                else if(layer.onlineRewardLinkedLayer != -1)
                {
                    layer.onlineRewards.Clear();
                }
            }
            */
            if(layer.onlineRewardLinkedLayer == -1)
            {
                tabDrawer.DoDraw(layer.onlineRewards);
            }
        }

        void UpdateOnlineRewardName(LayerDetail layer)
        {
            foreach(var onlineRewardLayer in layer.onlineRewards)
            {
                var originReward = onlineRewardMap[onlineRewardLayer.onlineRewardId];
                onlineRewardLayer.Name = "Minutes " + originReward.minutes;
            }
        }
    }

    public class OnlineRewardLayerTabDrawer : ListTabDrawer<OnlineRewardLayer>
    {
        readonly OnlineRewardDrawer onlineRewardDrawer;

        public OnlineRewardLayerTabDrawer()
        {
            onlineRewardDrawer = new OnlineRewardDrawer();
            DrawAddButton = false;
            KeepCurrentTabIndex = true;
        }

        protected override string GetTabLabel(OnlineRewardLayer element) => element.Name;

        protected override OnlineRewardLayer CreateNewElement() => null;

        protected override void DoDrawContent(OnlineRewardLayer element)
        {
            onlineRewardDrawer.DoDraw(element.rewards);
        }
    }
}