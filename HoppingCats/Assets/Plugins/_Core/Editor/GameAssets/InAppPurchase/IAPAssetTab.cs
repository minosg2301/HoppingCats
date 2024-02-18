using UnityEditor;
using UnityEngine;
using moonNest;

public class IAPAssetTab : TabContent
{
    private TabContainer tab;

    public IAPAssetTab()
    {
        tab = new TabContainer();
        tab.AddTab("IAP Offers", new IAPOfferTab());
        tab.AddTab("Triggers", new IAPTriggerTab());
        tab.HeaderType = HeaderType.Vertical;
    }

    public override void DoDraw()
    {
        Undo.RecordObject(IAPPackageAsset.Ins, "IAP Packages");
        tab.DoDraw();
        if(GUI.changed) Draw.SetDirty(IAPPackageAsset.Ins);
    }
}