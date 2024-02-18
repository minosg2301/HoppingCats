namespace moonNest.editor
{
    internal class ShopDefinitionTab : TabContent
    {
        private TableDrawer<ShopDefinition> table;

        public ShopDefinitionTab()
        {
            table = new TableDrawer<ShopDefinition>();
            table.AddCol("Name", 150, ele =>
            {
                if(table.drawingInlineAdd) ele.name = Draw.Text(ele.name, 150);
                else Draw.LabelBold(ele.name, 150);
            });
            table.AddCol("Type", 150, ele => ele.type = Draw.Enum(ele.type, 150), false);
            table.inlineAdd = true;
            table.drawOrder = false;
            table.drawControl = false;
            table.elementCreator = () => new ShopDefinition("New Shop");
            table.onElementAdded = ele => ShopAsset.Ins.AddShop(new ShopDetail(ele));
            table.onElementDeleted = ele => ShopAsset.Ins.RemoveShop(ele);
            table.askBeforeDelete = ele => $"Delete {ele.name} Shop will lose all {ele.name}'s Shop Items";
        }

        public override void DoDraw()
        {
            table.DoDraw(ShopAsset.Ins.shopDefinitions);
        }
    }
}