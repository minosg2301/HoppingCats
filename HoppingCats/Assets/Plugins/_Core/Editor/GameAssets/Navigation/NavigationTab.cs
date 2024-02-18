using System;
using UnityEditor;
using UnityEngine;
using moonNest;

public class NavigationTab : TabContent
{
    ListCardDrawer<NavigationData> cardDrawer;
    TableDrawer<NavigationPath> tableDrawer;


    public NavigationTab()
    {
        cardDrawer = new ListCardDrawer<NavigationData>();
        cardDrawer.onDrawElement = OnDrawEnum;
        cardDrawer.onDrawEditElement = OnDrawEditEnum;
        cardDrawer.elementCreator = () => new NavigationData("");
        cardDrawer.EditBeforeAdd = true;
        cardDrawer.onElementAdded = OnEnumAdded;

        tableDrawer = new TableDrawer<NavigationPath>();
        tableDrawer.onElementAdded = OnElementAdded;
        tableDrawer.AddCol("Navigation Path", 120, ele => ele.gameEventId = Draw.IntPopup(ele.gameEventId, GameDefinitionAsset.Ins.events, "name", "id", 120));
        tableDrawer.AddCol("Delay Time", 80, ele => ele.delayTime = Draw.Float(ele.delayTime, 80));
        tableDrawer.elementCreator = () => new NavigationPath("");

    }

    private void OnElementAdded(NavigationPath navDef)
    {
        navDef.name = navDef.name.RemoveSpace();
    }

    private void OnEnumAdded(NavigationData navData)
    {
        navData.name = navData.name.RemoveSpace();
    }

    private bool OnDrawEditEnum(NavigationData navData)
    {
        Draw.LabelBoldBox("Enter Navigation Name");

        navData.name = Draw.TextField("Name", navData.name, 120);

        string finalName = navData.name.RemoveSpace();
        return finalName.Length > 2 && GameDefinitionAsset.Ins.enums.Find(@enum => @enum.name == finalName) == null;
    }

    private void OnDrawEnum(NavigationData navData)
    {
        Draw.LabelBoldBox(navData.name, Color.blue);

        Draw.Space();
        tableDrawer.DoDraw(navData.paths);
    }

    public override void DoDraw()
    {
        Undo.RecordObject(NavigationAsset.Ins, "Navigation");
        cardDrawer.MaxWidth = Screen.width - 500;
        cardDrawer.DoDraw(NavigationAsset.Ins.navigationDatas);
        if (GUI.changed) Draw.SetDirty(NavigationAsset.Ins);
    }
}
