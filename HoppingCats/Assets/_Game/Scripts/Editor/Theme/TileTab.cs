using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
using UnityEditor;

public class TileTab : TabContent
{
    private TileAsset tileAsset;
    private readonly TileTableDrawer tileTable = new TileTableDrawer();

    public TileTab()
    {
        tileAsset = TileAsset.Ins;
        tileTable = new TileTableDrawer();
    }

    public override void DoDraw()
    {
        tileTable.DoDraw(tileAsset.tiles);
        Undo.RecordObject(TileAsset.Ins, "Tile Asset");
        if (GUI.changed) Draw.SetDirty(TileAsset.Ins);
    }
}

public class TileTableDrawer : TableDrawer<TileData>
{
    public TileTableDrawer()
    {
        AddCol("Name", 150, ele => ele.name = Draw.Text(ele.name, 150));
        elementCreator = () => new TileData("New Tile Type");
    }
}
