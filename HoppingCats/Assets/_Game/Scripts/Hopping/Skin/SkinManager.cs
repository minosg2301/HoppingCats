using moonNest;
using System.Collections.Generic;

public class SkinManager : SingletonMono<SkinManager>
{
    private SkinConfig currentSkinConfig;
    public SkinConfig CurrentSkinConfig 
    { 
        get 
        { 
            if(!currentSkinConfig) currentSkinConfig = UserSaveData.Ins.GetCurrentSkinConfig();
            return currentSkinConfig;
        } 
    }

    public List<SkinConfig> GetAllSkinConfigs()
    {
        return SkinAsset.Ins.skinConfigs;
    }

    public List<PlatformConfig> GetAllPlatformConfigs()
    {
        return CurrentSkinConfig.platforms;
    }

    public PlatformConfig GetPlatformConfig(PlatformType type)
    {
        return CurrentSkinConfig.platforms.Find(p => p.platformType == type);
    }

    public PlatformConfig GetSafePlatformConfig()
    {
        return CurrentSkinConfig.platforms.Find(p => p.isSafe);
    }

    public void ChangeSkin(int skinId)
    {
        var skinChanged = UserSaveData.Ins.ChangeSkin(skinId);
        if (skinChanged)
        {
            currentSkinConfig = UserSaveData.Ins.GetCurrentSkinConfig();
            GameEventManager.Ins.OnSkinUpdate(skinChanged);
        }
    }


}
