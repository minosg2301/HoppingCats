using moonNest;

public class IAPOfferTab : TabContent
{
    IAPGroupDrawer groupDrawer;

    public IAPOfferTab()
    {
        groupDrawer = new IAPGroupDrawer();
    }

    public override void DoDraw()
    {
        groupDrawer.DoDraw(IAPPackageAsset.Ins.Groups);
    }

    public override bool DoDrawWindow()
    {
        return groupDrawer.DoDrawWindow();
    }
}