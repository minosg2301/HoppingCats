using UnityEditor;
using UnityEngine;
using moonNest;

public class LockFeatureTab : TabContent
{
    private readonly TableDrawer<FeatureConfig> table;

    public LockFeatureTab()
    {
        table = new TableDrawer<FeatureConfig>();
        table.AddCol("Name", 120, ele =>
        {
            Draw.BeginDisabledGroup(!ele.canEditName);
            ele.name = Draw.Text(ele.name, 120);
            Draw.EndDisabledGroup();
        });
        table.AddCol("Display Name", 200, ele => ele.displayName = Draw.Text(ele.displayName, 200));
        table.AddCol("Locked", 60, ele =>
        {
            Draw.BeginChangeCheck();
            ele.locked = Draw.Toggle(ele.locked, 60);
            if (Draw.EndChangeCheck()) HandleLockChanged(ele);
        });
        table.AddCol("Unlock Condition", 120, ele =>
        {
            Draw.BeginDisabledGroup(!ele.locked);
            Draw.BeginChangeCheck();
            ele.unlockConditionId = Draw.IntPopup(ele.unlockConditionId, UnlockContentAsset.Ins.Datas, "name", "id", 120);
            if (Draw.EndChangeCheck()) HandleUnlockConditionChanged(ele);
            Draw.EndDisabledGroup();
        });
        table.AddCol("Icon", 120, ele => ele.icon = Draw.Sprite(ele.icon, 120));
        table.AddCol("uiPrefab", 120, ele => ele.uiPrefab = Draw.Object(ele.uiPrefab, 120));
        table.AddCol("Description", 200, ele => ele.description = Draw.Text(ele.description, 200));
        table.elementCreator = () => new FeatureConfig("New Feature");
        table.drawControl = false;
        table.drawIndex = false;
    }

    public override Color HeaderBackgroundColor => Color.grey;

    private static void HandleLockChanged(FeatureConfig feature)
    {
        var featureId = feature.id;
        if (feature.locked)
        {
            var unlockContent = UnlockContentAsset.Ins.FindContent(featureId);
            if (!unlockContent)
            {
                UnlockContentAsset.Ins.AddContent(new UnlockContentDetail(feature.name)
                {
                    type = UnlockContentType.Feature,
                    id = featureId,
                    contentId = featureId,
                    conditionId = feature.unlockConditionId
                });
            }
            else
            {

            }
        }
        else
        {
            UnlockContentAsset.Ins.RemoveContent(featureId);
        }
    }

    private static void HandleUnlockConditionChanged(FeatureConfig feature)
    {
        var featureId = feature.id;
        var unlockCondition = UnlockContentAsset.Ins.FindCondition(feature.unlockConditionId);
        if (unlockCondition)
        {
            var unlockContent = UnlockContentAsset.Ins.FindContent(featureId);
            unlockContent.conditionId = feature.unlockConditionId;
        }
    }

    public override void DoDraw()
    {
        Undo.RecordObject(GameDefinitionAsset.Ins, "Game Defines");
        table.DoDraw(GameDefinitionAsset.Ins.features);
        if (GUI.changed) EditorUtility.SetDirty(GameDefinitionAsset.Ins);
    }
}