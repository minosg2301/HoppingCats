using UnityEngine;
using moonNest;

public class LayerGroupChestTab : TabContent
{
    private readonly LayerGroup layerGroup;
    private readonly LayerDetailTabDrawer layerTab;

    public LayerGroupChestTab(LayerGroup group)
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
        const string EmptyChest = "Specific Chest for overriding at Chest Tab";
        readonly ChestListTabDrawer chestListTabDrawer;

        private LayerDetail lastLayer;

        public LayerDetailTabDrawer()
        {
            chestListTabDrawer = new ChestListTabDrawer();
        }

        protected override void DoDrawContent(LayerDetail layer)
        {
            if(layer != lastLayer)
            {
                lastLayer = layer;

                // update name from chest asset
                var chests = ChestAsset.Ins.chests;
                layer.chests.ForEach(chestLayer => chestLayer.Name = chests.Find(c => c.id == chestLayer.chestId).name);
            }

            if(layer.chests.Count == 0)
                Draw.LabelBoldBox(EmptyChest, Color.red);
            else
                chestListTabDrawer.DoDraw(layer.chests);
        }
    }

    class ChestListTabDrawer : ListTabDrawer<ChestLayer>
    {
        private readonly ListCardDrawer<CurrencyReward> currencyDrawer;
        private readonly ListCardDrawer<ItemReward> itemDrawer;

        public ChestListTabDrawer()
        {
            KeepCurrentTabIndex = true;
            DrawAddButton = false;

            currencyDrawer = new ListCardDrawer<CurrencyReward>();
            currencyDrawer.onDrawElement = ele => RewardDrawer.DrawCurrency(ele);
            currencyDrawer.elementCreator = () => new CurrencyReward();
            currencyDrawer.onElementAdded = ele => ele.contentId = GameDefinitionAsset.Ins.currencies[0].id;

            itemDrawer = new ListCardDrawer<ItemReward>();
            itemDrawer.onDrawElement = ele => RewardDrawer.DrawItem(ele);
            itemDrawer.elementCreator = () => new ItemReward();
            itemDrawer.onElementAdded = ele => ele.contentId = ItemAsset.Ins.items[0].id;
            itemDrawer.CardHeight = 130;
        }

        protected override ChestLayer CreateNewElement() => null;

        protected override string GetTabLabel(ChestLayer chest) => chest.Name;

        protected override void DoDrawContent(ChestLayer chest)
        {
            Draw.Space();
            Draw.LabelBoldBox("Currencies", Color.blue);
            currencyDrawer.DoDraw(chest.content.currencies);

            Draw.Space();
            Draw.LabelBoldBox("Items", Color.blue);
            itemDrawer.DoDraw(chest.content.items);
        }
    }
}