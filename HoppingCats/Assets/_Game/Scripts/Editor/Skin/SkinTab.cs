using UnityEngine;
using moonNest;
using UnityEditor;
using System.Collections.Generic;

public class SkinTab : TabContent
{
    private readonly SkinAsset skinAsset;
    private readonly SkinTabDrawer skinTable;

    public SkinTab()
    {
        skinAsset = SkinAsset.Ins;
        skinTable = new();
    }

    public override void DoDraw()
    {
        skinTable.DoDraw(skinAsset.skinConfigs);
        Undo.RecordObject(SkinAsset.Ins, "Skin Configs");
        if (GUI.changed) Draw.SetDirty(SkinAsset.Ins);
    }
}

public class SkinTabDrawer : ListTabDrawer<SkinConfig>
{
    private readonly PlatformTable platformTable;

    public SkinTabDrawer()
    {
        HeaderType = HeaderType.Vertical;
        platformTable = new();
    }

    protected override SkinConfig CreateNewElement()
    {
        var newElement = new SkinConfig();
        newElement.id = Random.Range(0, int.MaxValue);
        newElement.name = "new";
        return newElement;
    }

    protected override void DoDrawContent(SkinConfig element)
    {
        var lastWidth = Draw.kLabelWidth;
        Draw.kLabelWidth = 120;
        element.name = Draw.TextField("Name", element.name, 100);
        element.isFree = Draw.ToggleField("Free", element.isFree, 100);
        if (!element.isFree)
        {
            element.price = Draw.IntField("Price", element.price, 100);
        }

        element.bgSprite = Draw.SpriteField("Background", element.bgSprite, false, 200, 200);
        element.catPrefab = Draw.ObjectField("Cat Prefab", element.catPrefab, 200);

        platformTable.DoDraw(element.platforms);
        Draw.kLabelWidth = lastWidth;
    }

    protected override string GetTabLabel(SkinConfig element)
    {
        return "SKin " + element.name;
    }
}

public class PlatformTable : TableDrawer<PlatformConfig>
{
    public PlatformTable()
    {
        AddCol("Type", 100, ele => ele.platformType = Draw.Enum(ele.platformType, 100));
        AddCol("IsSafe", 100, ele => ele.isSafe = Draw.Toggle(ele.isSafe, 100));
        AddCol("Prefab", 100, ele => ele.platformPrefab = Draw.Object(ele.platformPrefab, 100));
        elementCreator = () => new PlatformConfig();
    }

    public override void DoDraw(List<PlatformConfig> list)
    {
        base.DoDraw(list);
    }
}

