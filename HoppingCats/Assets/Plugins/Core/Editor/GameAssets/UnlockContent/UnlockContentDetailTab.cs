using moonNest;

public class UnlockContentDetailTab : TabContent
{
    private TableDrawer<UnlockContentDetail> table;

    public UnlockContentDetailTab()
    {
        var typeFilter = new FilterCell<UnlockContentDetail, UnlockContentType>(
            UnlockContentType.Item,
            (type, w) => Draw.Enum(type, w),
            (ucd, type) => type == UnlockContentType.None || ucd.type == type);

        var conditionFilter = new FilterCell<UnlockContentDetail, int>(
            -1,
            (id, w) => Draw.IntPopup(id, UnlockContentAsset.Ins.Datas, "name", "id", w),
            (ucd, id) => ucd.conditionId == id);

        var itemTypeFilter = new FilterCell<UnlockContentDetail, int>(
            -1,
            (id, w) => Draw.IntPopup(id, ItemAsset.Ins.definitions, "name", "id", w),
            (ucd, id) =>
            {
                if(ucd.type != UnlockContentType.Item) return false;
                return ucd.itemDefinitionId == id;
            });

        table = new TableDrawer<UnlockContentDetail>();
        table.AddCol("Name", 200, ele => ele.name = Draw.Text(ele.name, 200));
        table.AddCol("Condition", 120, ele => DrawUnlockCondition(ele, 120)).AddFilter(conditionFilter);
        table.AddCol("Type", 60, ele => ele.type = Draw.Enum(ele.type, 60), false).AddFilter(typeFilter);
        table.AddCol("Content", 120, ele => DrawContent(ele, 120), false);
        table.AddCol("Item Type", 120, ele => DrawSubContent(ele, 120), false).AddFilter(itemTypeFilter);
        table.drawFilter = true;
        table.drawOrder = false;
        table.drawDeleteButton = false;
    }

    private void DrawUnlockCondition(UnlockContentDetail unlockContent, float maxWidth = -1)
    {
        Draw.BeginChangeCheck();
        unlockContent.conditionId = Draw.IntPopup(unlockContent.conditionId, UnlockContentAsset.Ins.Datas, "name", "id", maxWidth);
        if(Draw.EndChangeCheck())
        {
            if(unlockContent.type == UnlockContentType.Feature)
            {
                var feature = GameDefinitionAsset.Ins.FindFeature(unlockContent.contentId);
                if(feature)
                {
                    feature.unlockConditionId = unlockContent.conditionId;
                }
            }
            unlockContent.UnlockCondition = null;
            UnlockContentAsset.Ins.UpdateLinkedProgress(unlockContent);
        }
    }

    private void DrawContent(UnlockContentDetail unlockContent, float maxWidth = -1)
    {
        if(unlockContent.type == UnlockContentType.Item)
        {
            unlockContent.contentId = Draw.IntPopup(unlockContent.contentId, ItemAsset.Ins.FindByDefinition(unlockContent.itemDefinitionId), "name", "id", maxWidth);
        }
        else if(unlockContent.type == UnlockContentType.Feature)
        {
            unlockContent.contentId = Draw.IntPopup(unlockContent.contentId, GameDefinitionAsset.Ins.features, "name", "id", maxWidth);
        }
    }

    private void DrawSubContent(UnlockContentDetail unlockContent, float maxWidth = -1)
    {
        if(unlockContent.type == UnlockContentType.Item)
        {
            unlockContent.itemDefinitionId = Draw.IntPopup(unlockContent.itemDefinitionId, ItemAsset.Ins.definitions, "name", "id", maxWidth);
        }
        else
        {
            Draw.Label("", maxWidth);
        }
    }

    public override void DoDraw()
    {
        table.DoDraw(UnlockContentAsset.Ins.unlockContents);
    }
}