using moonNest;

public abstract class BaseLayerTabDrawer : ListTabDrawer<LayerDetail>
{
    public BaseLayerTabDrawer()
    {
        HeaderType = HeaderType.Vertical;
    }

    protected override LayerDetail CreateNewElement() => null;

    protected override string GetTabLabel(LayerDetail element) => element.name;
}