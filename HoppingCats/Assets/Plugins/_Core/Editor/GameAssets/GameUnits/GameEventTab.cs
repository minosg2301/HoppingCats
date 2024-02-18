
namespace moonNest.editor
{
    internal class GameEventTab : TabContent
    {
        readonly TableDrawer<GameEvent> tableDrawer;

        public GameEventTab()
        {
            tableDrawer = new TableDrawer<GameEvent>("Navigation Path");
            tableDrawer.AddCol("Name", 250, ele => ele.name = Draw.Text(ele.name, 250));
            tableDrawer.elementCreator = () => new GameEvent("New game event");

        }


        public override void DoDraw()
        {
            tableDrawer.DoDraw(GameDefinitionAsset.Ins.events);
        }
    }
}
