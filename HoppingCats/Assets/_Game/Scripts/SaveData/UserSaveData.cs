using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
public class UserSaveData : BaseUserData
{
    public static UserSaveData Ins => LocalData.Get<UserSaveData>();

    public int topScore;
    public int star;

    protected override void OnInit()
    {
        base.OnInit();
    }

    public void SetTopScore(int topScore)
    {
        this.topScore = topScore;
        Save();
    }

    public void AddStar(int amount)
    {
        star += amount;
        star = star > 0 ? star : 0;
        Save();
    }
}
    