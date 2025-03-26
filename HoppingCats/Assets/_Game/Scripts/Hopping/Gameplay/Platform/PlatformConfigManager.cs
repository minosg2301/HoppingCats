using moonNest;
using System.Collections.Generic;

public class PlatformConfigManager : SingletonScriptObject<PlatformConfigManager>
{
    public List<PlatformConfig> platformConfigs;
    public List<PlatformConfig> platformRandomConfigs;
}