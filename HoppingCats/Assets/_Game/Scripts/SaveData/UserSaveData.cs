using moonNest;
using System.Collections.Generic;

public class UserSaveData : BaseUserData
{
    public static UserSaveData Ins => LocalData.Get<UserSaveData>();

    public int topScore;
    public int star;

    public List<int> userSkinIds = new();
    public int currentUserSkinId = -1;

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

    //Skin
    public SkinConfig GetCurrentSkinConfig()
    {
        if(currentUserSkinId == -1)
        {
            var freeSkinConfig = SkinAsset.Ins.skinConfigs.Find(s => s.isFree == true);
            Save();
            return freeSkinConfig;
        }
        else
        {
            return SkinAsset.Ins.skinConfigs.Find(s => s.id == currentUserSkinId);
        }
    }

    public SkinConfig ChangeSkin(int skinId)
    {
        var skinConfig = SkinAsset.Ins.skinConfigs.Find(s => s.id == skinId);
        if (skinConfig)
        {
            currentUserSkinId = skinId;
            Save();
        }

        return skinConfig;
    }
}
    