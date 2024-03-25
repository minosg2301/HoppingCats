using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
using UnityEditor;

public class BodyTab : TabContent
{
    private BodyAsset bodyAsset;
    private readonly BodyTableDrawer bodyTable = new BodyTableDrawer();

    public BodyTab()
    {
        bodyAsset = BodyAsset.Ins;
        bodyTable = new BodyTableDrawer();
    }

    public override void DoDraw()
    {
        bodyTable.DoDraw(bodyAsset.parts);
        Undo.RecordObject(BodyAsset.Ins, "Body Asset");
        if (GUI.changed) Draw.SetDirty(BodyAsset.Ins);
    }
}

public class BodyTableDrawer : TableDrawer<BodyPart>
{
    public BodyTableDrawer()
    {
        AddCol("Name", 150, ele => ele.name = Draw.Text(ele.name, 150));
        AddCol("Sorting Order", 150, ele => ele.sortingOrder = Draw.Int(ele.sortingOrder, 150));
        elementCreator = () => new BodyPart("New Body Part");
    }
}
