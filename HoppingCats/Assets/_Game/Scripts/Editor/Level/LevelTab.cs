using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
using UnityEditor;
using System.Linq;

public class LevelTab : TabContent
{
    private LevelConfigs configs;
    private readonly LevelTableDrawer levelTable = new LevelTableDrawer();

    public LevelTab()
    {
        configs = LevelConfigs.Ins;
        levelTable = new();
    }

    public override void DoDraw()
    {
        levelTable.DoDraw(configs.levelConfigs);
        Undo.RecordObject(LevelConfigs.Ins, "Level Configs");
        if (GUI.changed) Draw.SetDirty(LevelConfigs.Ins);
    }
}

public class LevelTableDrawer : ListTabDrawer<LevelConfig>
{
    private readonly PlatformRandomTable platformRandomTable = new PlatformRandomTable();

    public LevelTableDrawer()
    {
        HeaderType = HeaderType.Vertical;
        platformRandomTable = new();
    }

    protected override LevelConfig CreateNewElement()
    {
        var newElement = new LevelConfig();
        newElement.level = LevelConfigs.Ins.levelConfigs.Count + 1;
        newElement.platformRandomDatas = new();
        return newElement;
    }

    protected override void DoDrawContent(LevelConfig element)
    {
        var lastWidth = Draw.kLabelWidth;
        Draw.kLabelWidth = 50;
        element.level = Draw.IntField("Level", element.level, 100);
        element.name = Draw.TextField("Name", element.name, 100);
        element.fromFloor = Draw.IntField("From", element.fromFloor, 100);
        element.toFloor = Draw.IntField("To", element.toFloor, 100);
        Draw.kLabelWidth = lastWidth;
        platformRandomTable.DoDraw(element.platformRandomDatas);
    }

    protected override string GetTabLabel(LevelConfig element)
    {
        return "level " + element.level;
    }
}

public class PlatformRandomTable : TableDrawer<PlatformRandomData>
{
    public PlatformRandomTable()
    {
        AddCol("Type", 100, ele => ele.type = Draw.Enum(ele.type, 100));
        AddCol("IsSafe", 100, ele => ele.isSafe = Draw.Toggle(ele.isSafe, 100));
        AddCol("Weight", 300, ele => ele.weight = Draw.IntSlider(ele.weight, 0, 9999, 300));
        AddCol("Probability", 100, ele => Draw.Label(ele.probability.ToString("00.0000%"), 100));
        elementCreator = () => new PlatformRandomData();
    }

    public override void DoDraw(List<PlatformRandomData> list)
    {
        base.DoDraw(list);
        if (Draw.EndChangeCheck())
        {
            CalculateProbability(list);
        }
    }

    private void CalculateProbability(List<PlatformRandomData> randomDetails)
    {
        float totalPoint = randomDetails.Sum(_ => _.weight);
        randomDetails.ForEach(_ => _.probability = totalPoint == 0 ? 0 : _.weight / totalPoint);
    }

}
