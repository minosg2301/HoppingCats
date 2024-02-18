using UnityEditor;
using UnityEngine;
using moonNest;

internal class InviteFriendTab : TabContent
{
    private readonly InviteRewardListDrawer inviteRewardListDrawer;

    internal InviteFriendTab()
    {
        inviteRewardListDrawer = new InviteRewardListDrawer();
    }

    public override void DoDraw()
    {
        Undo.RecordObject(RewardAsset.Ins, "Reward Config");
        
        Draw.BeginVertical(Draw.SubContentStyle);
        RewardAsset.Ins.testInviteReward = Draw.ToggleField(RewardAsset.Ins.testInviteReward, "Test Reward", 40);

        Draw.Space();
        inviteRewardListDrawer.DoDraw(RewardAsset.Ins.inviteRewards);

        Draw.EndVertical();
        if (GUI.changed) Draw.SetDirty(RewardAsset.Ins);
    }

    class InviteRewardListDrawer : ListTabDrawer<InviteReward>
    {
        RewardDrawer rewardDrawer;

        internal InviteRewardListDrawer()
        {
            rewardDrawer = new RewardDrawer();
        }

        protected override InviteReward CreateNewElement() => new();

        protected override string GetTabLabel(InviteReward element) => element.require.ToString();

        protected override void DoDrawContent(InviteReward element)
        {
            element.require = Draw.IntField("Require", element.require, 100);
            rewardDrawer.DoDraw(element.rewardDetail);
        }
    }
}