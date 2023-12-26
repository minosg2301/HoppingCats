using System.Collections.Generic;
using UnityEngine;
using moonNest;

public class RewardListDrawer
{
    public List<RewardDrawer> rewardDrawers = new List<RewardDrawer>();

    public void DoDraw(List<RewardDetail> list)
    {
        DrawRewards(list);
        Draw.Space();
        if(Draw.FitButton("Add New Reward", Color.yellow))
            list.Add(new RewardDetail("New Reward"));
    }

    void DrawRewards(List<RewardDetail> list)
    {
        while(list.Count > rewardDrawers.Count)
            rewardDrawers.Add(new RewardDrawer() { DrawHeader = false });

        for(int i = 0; i < list.Count; i++)
        {
            Draw.BeginHorizontal();
            if(Draw.FitButton("X", Color.gray))
            {
                list.Remove(list[i]);
                break;
            }
            Draw.LabelBoldBox("Reward " + (i + 1), Color.cyan);
            Draw.EndHorizontal();
            rewardDrawers[i].DoDraw(list[i]);
        }
    }
}