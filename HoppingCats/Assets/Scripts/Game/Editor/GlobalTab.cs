using moonNest;

internal class GlobalTab : TabContent
{
    private TabContainer tabContainer;

    public GlobalTab()
    {
        tabContainer = new TabContainer();
    }

    public override void DoDraw()
    {
        tabContainer.DoDraw();
    }
}

