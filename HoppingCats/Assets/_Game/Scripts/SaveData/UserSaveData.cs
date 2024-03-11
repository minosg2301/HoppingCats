using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
public class UserSaveData : BaseUserData
{
    public static UserSaveData Ins => LocalData.Get<UserSaveData>();

    public int topScore;

    protected override void OnInit()
    {
        base.OnInit();
        topScore = 0;
    }

    public void SetTopScore(int topScore)
    {
        this.topScore = topScore;
        Save();
    }
}
    