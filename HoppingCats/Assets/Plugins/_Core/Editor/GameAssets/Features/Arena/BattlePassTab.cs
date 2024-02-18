using UnityEngine;
using moonNest;

public class BattlePassTab : TabContent
{
    readonly BattlePassLevelCardDrawer cardDrawer;
    readonly RewardDrawer finalLevelRewardDrawer;

    public BattlePassSeason Season { get; private set; }

    public BattlePassTab()
    {
        cardDrawer = new BattlePassLevelCardDrawer();
        cardDrawer.onElementAdded += OnLevelAdded;
        cardDrawer.onElementRemoved += OnLevelRemoved;
        finalLevelRewardDrawer = new RewardDrawer();
    }

    private void OnLevelRemoved(BattlePassLevel obj)
    {
        var arena = ArenaAsset.Ins;
        arena.finalLevel.level = arena.levels.Count + 1;
    }

    private void OnLevelAdded(BattlePassLevel obj)
    {
        var arena = ArenaAsset.Ins;
        arena.finalLevel.level = arena.levels.Count + 1;
    }

    public override void DoDraw()
    {
        var arena = ArenaAsset.Ins;

        Draw.BeginHorizontal();
        Draw.BeginVertical(600);

        arena.requireStatId = Draw.IntPopupField("Require Stat", arena.requireStatId, UserPropertyAsset.Ins.properties.stats, "name", "id", 150);
        arena.premiumStatId = Draw.IntPopupField("Premium Stat", arena.premiumStatId, UserPropertyAsset.Ins.properties.stats, "name", "id", 150);
        arena.requireDescription = Draw.TextField("Description Format", arena.requireDescription, 400);
        DrawLayerEnabled();

        Draw.SpaceAndLabelBoldBox("Final Level - " + arena.finalLevel.level, Color.yellow);
        finalLevelRewardDrawer.MaxWidth = Screen.width - 630;
        arena.finalLevel.requireValue = Draw.IntField("Require", arena.finalLevel.requireValue, 150);
        Draw.Space();
        finalLevelRewardDrawer.DoDraw(arena.finalLevel.reward);

        Draw.EndVertical();

        Draw.Space(30);
        Draw.BeginVertical();
        Draw.LabelBoldBox("Levels", Color.yellow);
        cardDrawer.Levels = arena.levels;
        cardDrawer.DoDraw(arena.levels);
        Draw.EndVertical();

        Draw.EndHorizontal();
    }

    void DrawLayerEnabled()
    {
        if(ArenaAsset.Ins.layerEnabled)
        {
            LayerGroup layerGroup = LayerAsset.Ins.FindGroup(ArenaAsset.Ins.layerGroupId);
            if(layerGroup)
                Draw.LabelBoldBox($"Battle Pass Rewards is overrided by Layer '{layerGroup.name}'", Color.blue);
            else
                Draw.LabelBoldBox("Select which Layer Group in LAYER TAB", Color.red);
        }
    }
}
