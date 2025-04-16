using moonNest;
using System.Collections.Generic;
using System.Linq;

public class LevelConfigs : SingletonScriptObject<LevelConfigs>
{
    public List<LevelConfig> levelConfigs = new();

    public LevelConfig GetLevelConfig(int floor)
    {
        return levelConfigs.Find(l => l.IsInRange(floor)) ?? levelConfigs.Last();
    }
}
