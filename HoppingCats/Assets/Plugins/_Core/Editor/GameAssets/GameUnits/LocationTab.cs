namespace moonNest.editor
{
    internal class LocationTab : TabContent
    {
        readonly TableDrawer<LocationDefinition> tableDrawer;

        public LocationTab()
        {
            tableDrawer = new TableDrawer<LocationDefinition>("Location Definition");
            tableDrawer.AddCol("Name", 250, ele => ele.name = Draw.Text(ele.name, 250));
            tableDrawer.elementCreator = () => new LocationDefinition("New Location");
        }

        public override void DoDraw()
        {
            tableDrawer.DoDraw(GameDefinitionAsset.Ins.locations);
        }
    }
}