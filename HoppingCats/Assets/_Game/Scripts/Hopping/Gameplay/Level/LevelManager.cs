using moonNest;
using System.Linq;
using UnityEngine;

public class LevelManager : SingletonMono<LevelManager>
{
    public LevelConfig currentLevelConfig;
    public int currentFloor = 0;
    public bool isLastLevelConfig;

    public void InitLevel()
    {
        isLastLevelConfig = false;
        currentFloor = 0;
        currentLevelConfig = LevelConfigs.Ins.GetLevelConfig(currentFloor);
    }

    public void UpdateLevel()
    {
        currentFloor++;
        if (!isLastLevelConfig && !currentLevelConfig.IsInRange(currentFloor))
        {
            var nextLevel = LevelConfigs.Ins.GetLevelConfig(currentFloor);
            if(currentLevelConfig.id != nextLevel.id)
            {
                currentLevelConfig = nextLevel;
            }
            else
            {
                isLastLevelConfig = true;
            }
        }
    }

    public PlatformConfig GetPlatformConfig(PlatformType type)
    {
        return SkinManager.Ins.GetPlatformConfig(type);
    }

    public PlatformConfig RandomPlatform(bool onlySafe = false)
    {
        var sortList = currentLevelConfig.platformRandomDatas.Where(data =>
        {
            if (onlySafe)
            {
                if (data.isSafe) return true;
                else return false;
            }
            else return true;

        }).ToList();

        if (onlySafe)
        {
            sortList = currentLevelConfig.platformRandomDatas.FindAll(p => p.isSafe);
        }
        else
        {
            sortList = currentLevelConfig.platformRandomDatas;
        }

        if (sortList.Count > 0)
        {
            sortList.SortAsc(_ => _.weight);
            int sum = sortList.Sum(_ => _.weight);
            float ran = Random.Range(0f, sum);
            float k = 0;

            foreach (PlatformRandomData randomContent in sortList)
            {
                k += randomContent.weight;

                if (ran < k)
                {
                    return SkinManager.Ins.GetPlatformConfig(randomContent.type);
                }
            }

            return SkinManager.Ins.GetPlatformConfig(sortList[sortList.Count - 1].type);
        }

        return null;
    }
}
