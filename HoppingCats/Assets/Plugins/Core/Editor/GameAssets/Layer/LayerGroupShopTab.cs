using System.Collections.Generic;

namespace moonNest.editor
{
    public class LayerGroupShopTab : TabContent
    {
        private readonly LayerGroup layerGroup;
        private readonly LayerDetailTabDrawer layerTab;

        public LayerGroupShopTab(LayerGroup group, int shopId)
        {
            layerGroup = group;
            layerTab = new LayerDetailTabDrawer(shopId);
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
            private readonly int shopId;
            private readonly ShopItemTableDrawer shopItemTable;

            private LayerDetail lastLayer;
            private ShopLayer shopLayer;

            private List<QuestLayer> questLayers;

            public LayerDetailTabDrawer(int shopId)
            {
                this.shopId = shopId;
                shopItemTable = new ShopItemTableDrawer("Shop");
            }

            protected override void DoDrawContent(LayerDetail layer)
            {
                if(layer != lastLayer)
                {
                    lastLayer = layer;
                    shopLayer = layer.shops.Find(q => q.shopId == shopId);
                    shopItemTable.Shop = ShopAsset.Ins.FindShop(shopId);
                }

                shopItemTable.DoDraw(shopLayer.shopItems);
            }
        }
    }
}