using UnityEngine;
using moonNest;


public class OnlineRewardTabDrawer : ListTabDrawer<OnlineRewardDetail>
{
    readonly OnlineRewardDrawer onlineRewardDrawer;
    const string Minutes = " Minutes";

    public OnlineRewardTabDrawer()
    {
        onlineRewardDrawer = new OnlineRewardDrawer();
    }

    protected override string GetTabLabel(OnlineRewardDetail element) => element.minutes + Minutes;

    protected override OnlineRewardDetail CreateNewElement() => new OnlineRewardDetail("");

    protected override void DoDrawContent(OnlineRewardDetail element)
    {
        element.minutes = Draw.IntField("Minutes", element.minutes);
        element.serverId = Draw.IntField("ServerId", element.serverId);
        Draw.Space();
        onlineRewardDrawer.DoDraw(element.rewards);
    }
}