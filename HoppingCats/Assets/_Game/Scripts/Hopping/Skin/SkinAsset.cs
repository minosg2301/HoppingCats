using moonNest;
using System.Collections.Generic;

public class SkinAsset : SingletonScriptObject<SkinAsset>
{
    public List<SkinConfig> skinConfigs = new();
}
