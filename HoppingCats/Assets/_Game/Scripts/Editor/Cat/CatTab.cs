using moonNest;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CatTab : TabContent
{
    private CatAsset catAsset;
    private readonly CatDrawer catTab = new CatDrawer();

    public CatTab()
    {
        catAsset = CatAsset.Ins;
    }

    public override void DoDraw()
    {
        Undo.RecordObject(CatAsset.Ins, "Cats");
        catTab.DoDraw(CatAsset.Ins.cats);
        if (GUI.changed) Draw.SetDirty(CatAsset.Ins);
    }

    public override bool DoDrawWindow() => catTab.DoDrawWindow();
}

public class CatDrawer : ListTabDrawer<CatData>
{
    private readonly CatTableDrawer catTable = new CatTableDrawer();

    public CatDrawer()
    {
        HeaderType = HeaderType.Vertical;
        onSwapPerformed += OnSwapPerformed;
        askBeforeDelete = ele => $"Consider delete {ele.name}";

        catTable = new CatTableDrawer();
    }

    private void OnSwapPerformed(CatData arg1, CatData arg2)
    {
        CatData temp = arg1;
        arg1 = arg2;
        arg2 = temp;
    }

    protected override CatData CreateNewElement() => new CatData("New Cat");
    protected override string GetTabLabel(CatData element) => element.name;

    protected override void DoDrawContent(CatData element)
    {
        DrawInfo(element);
        Draw.Space();
        DrawBodyParts(element);
        Draw.Space();
    }

    private void DrawInfo(CatData element)
    {
        Draw.LabelBoldBox("Info", Color.yellow);
        Draw.Space();
        element.name = Draw.TextField("Name", element.name);
        element.theme = Draw.StringPopupField("Theme", element.theme, ThemeAsset.Ins.themes, "name");
    }

    private void DrawBodyParts(CatData element)
    {
        Draw.LabelBoldBox("Body Parts", Color.yellow);
        Draw.Space();
        catTable.DoDraw(element.catParts);
    }

    public override bool DoDrawWindow()
    {
        base.DoDrawWindow();
        return catTable.DoDrawWindow();
    }
}

public class CatTableDrawer : TableDrawer<CatPart>
{
    public CatTableDrawer()
    {
        AddCol("Sprite", 120, ele => { DrawUtils.DrawAddressableSprite(ele.sprite, 120, 120); });
        AddCol("Type", 120, ele => ele.type = Draw.StringPopup(ele.type, BodyAsset.Ins.parts, "name"));
        AddCol("Offset", 120, ele => ele.offset = Draw.Vector2(ele.offset, 120));
        AddCol("Scale", 120, ele => ele.scale = Draw.Vector2(ele.scale, 120));
        elementCreator = () => new CatPart();
    }
}