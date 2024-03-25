using moonNest;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ThemeTab : TabContent
{
    private ThemeAsset themeAsset;
    private readonly ThemeDrawer themeTab = new ThemeDrawer();

    public ThemeTab() 
    {
        themeAsset = ThemeAsset.Ins;
    }

    public override void DoDraw()
    {
        Undo.RecordObject(ThemeAsset.Ins, "Themes");
        themeTab.DoDraw(themeAsset.themes);
        if (GUI.changed) Draw.SetDirty(ThemeAsset.Ins);
    }

    public override bool DoDrawWindow() => themeTab.DoDrawWindow();
}

public class ThemeDrawer : ListTabDrawer<ThemeData>
{
    private readonly ThemeTileTable tileTable = new ThemeTileTable();

    public ThemeDrawer()
    {
        HeaderType = HeaderType.Vertical;
        onSwapPerformed += OnSwapPerformed;
        askBeforeDelete = ele => $"Consider delete {ele.name}";

        tileTable = new ThemeTileTable();
    }

    private void OnSwapPerformed(ThemeData arg1, ThemeData arg2)
    {
        ThemeData temp = arg1;
        arg1 = arg2;
        arg2 = temp;
    }

    protected override ThemeData CreateNewElement() => new ThemeData("New Theme");
    protected override string GetTabLabel(ThemeData element) => element.name;

    protected override void DoDrawContent(ThemeData element)
    {
        DrawInfo(element);
        Draw.Space();
        DrawThemeTiles(element);
        Draw.Space();
    }

    private void DrawInfo(ThemeData element)
    {
        Draw.LabelBoldBox("Info", Color.yellow);
        Draw.Space();
        element.name = Draw.TextField("Name", element.name);  
    }

    private void DrawThemeTiles(ThemeData element)
    {
        Draw.LabelBoldBox("Body Parts", Color.yellow);
        Draw.Space();

        foreach(var t in element.tiles.ToArray())
        {
            if (TileAsset.Ins.Find(t.name) == null)
            {
                element.tiles.Remove(t);
            }
        }

        foreach(var t in TileAsset.Ins.tiles)
        {
            if(!element.tiles.Contains(t.name))
            {
                element.tiles.Add(new ThemeTile(t.name));
            }
        }

        tileTable.DoDraw(element.tiles);
    }

    public override bool DoDrawWindow()
    {
        base.DoDrawWindow();
        return tileTable.DoDrawWindow();
    }
}

public class ThemeTileTable : TableDrawer<ThemeTile>
{
    public ThemeTileTable()
    {
        AddCol("Type", 120, ele => Draw.Label(ele.type, 120));
        AddCol("Sprite", 120, ele => { DrawUtils.DrawAddressableSprite(ele.sprite, 120, 120); });
    }
}
