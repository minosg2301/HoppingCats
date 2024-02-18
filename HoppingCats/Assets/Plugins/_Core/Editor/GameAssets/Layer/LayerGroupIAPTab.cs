using System.Collections.Generic;
using moonNest;

public class LayerGroupIAPTab : TabContent
{
    private readonly LayerGroup layerGroup;
    private readonly LayerDetailTabDrawer layerTab;

    public LayerGroupIAPTab(LayerGroup group, int iapPackageGroupId)
    {
        layerGroup = group;

        layerTab = new LayerDetailTabDrawer(iapPackageGroupId);
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
        readonly int groupId;
        readonly IAPPackageListTabDrawer iapPackageListDrawer;

        LayerDetail lastLayer;
        IAPPackageGroupLayer iapGroupLayer;
        List<LayerDetail> LinkableLayers;
        List<IAPPackageLayer> iapPackages;

        public LayerDetailTabDrawer(int iapPackageGroupId)
        {
            groupId = iapPackageGroupId;
            iapPackageListDrawer = new IAPPackageListTabDrawer();
        }

        protected override void DoDrawContent(LayerDetail layer)
        {
            if(layer != lastLayer)
            {
                lastLayer = layer;
                iapGroupLayer = lastLayer.iapPackageGroups.Find(g => g.groupId == groupId);

                // update LinkableLayers
                LinkableLayers = List.FindAll(l => l.id != lastLayer.id && iapGroupLayer.iapLinkedLayer == -1);

                // update name from IAPPackageAsset
                List<IAPPackage> originIAPPackages = IAPPackageAsset.Ins.FindByGroup(groupId);
                iapPackages = layer.iapPackageGroups.Find(q => q.groupId == groupId).packages;
                iapPackages.ForEach(iapPackageLayer =>
                {
                    IAPPackage iapPackage = originIAPPackages.Find(q => q.id == iapPackageLayer.iapPackageId);
                    iapPackageLayer.name = !iapPackage.free ? iapPackage.productId : TextPack.FreePackage;
                });
            }

            Draw.BeginChangeCheck();
            int lastLinkedLayer = iapGroupLayer.iapLinkedLayer;
            iapGroupLayer.iapLinkedLayer = Draw.IntPopupField("Linked With Layer", iapGroupLayer.iapLinkedLayer, LinkableLayers, "name", "id", 120);
            if(Draw.EndChangeCheck())
            {
                if(iapGroupLayer.iapLinkedLayer == -1 && lastLinkedLayer != -1)
                {
                    var linkedLayer = LinkableLayers.Find(l => l.id == lastLinkedLayer);
                    if(linkedLayer)
                    {
                        var linkedIAPGroupLayer = linkedLayer.iapPackageGroups.Find(g => g.groupId == groupId);
                        iapGroupLayer.packages = linkedIAPGroupLayer.packages.Map(o => o.Clone() as IAPPackageLayer);
                    }
                }
                else if(layer.arenaLinkedLayer != -1)
                {
                    iapGroupLayer.packages.Clear();
                }
            }

            if(iapGroupLayer.iapLinkedLayer == -1)
                iapPackageListDrawer.DoDraw(iapPackages);
        }

        protected override int GetLabelFontSize(LayerDetail layer)
        {
            var groupLayer = layer.iapPackageGroups.Find(g => g.groupId == groupId);
            return groupLayer.iapLinkedLayer == -1 ? base.GetLabelFontSize(layer) : 8;
        }
    }

    private class IAPPackageListTabDrawer : ListTabDrawer<IAPPackageLayer>
    {
        readonly RewardDrawer rewardDrawer;
        readonly RewardListDrawer rewardListDrawer;

        public IAPPackageListTabDrawer()
        {
            KeepCurrentTabIndex = true;
            DrawAddButton = false;
            rewardDrawer = new RewardDrawer();
            rewardListDrawer = new RewardListDrawer();
        }

        protected override IAPPackageLayer CreateNewElement() => null;

        protected override string GetTabLabel(IAPPackageLayer iapPackageLayer) => iapPackageLayer.name;

        protected override void DoDrawContent(IAPPackageLayer iapPackageLayer)
        {
            rewardListDrawer.DoDraw(iapPackageLayer.rewards);
        }
    }
}