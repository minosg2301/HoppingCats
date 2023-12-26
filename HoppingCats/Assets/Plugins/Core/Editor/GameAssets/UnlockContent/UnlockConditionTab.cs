using moonNest;

public class UnlockConditionTab : TabContent
{
    readonly UnlockConditionGroupTab unlockConditionGroupTab = new UnlockConditionGroupTab();

    public UnlockConditionTab()
    {
        unlockConditionGroupTab.onElementRemoved = OnGroupDeleted;
    }

    private void OnGroupDeleted(UnlockConditionGroup group)
    {
        UnlockContentAsset.Ins.RemoveGroup(group.id);
    }

    public override void DoDraw()
    {
        unlockConditionGroupTab.DoDraw(UnlockContentAsset.Ins.Groups);
    }
}