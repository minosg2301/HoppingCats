using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    private const int firstGenerateCount = 5;

    private Dictionary<int, List<Platform>> platformsByRow;

    private int rowIndex;

    public Transform container;
    public GameObject firstStep;

    public void Clear()
    {
        container.RemoveAllChildren();
        platformsByRow = new();
        rowIndex = 0;
    }

    public void InitPlatforms()
    {
        AddInitPlatform();
        foreach (var platform in platformsByRow)
        {
            InstantiatePlatform(platform.Value, platform.Key * 2);
        }
    }

    private void InstantiatePlatform(List<Platform> platforms, int row)
    {
        foreach (var platform in platforms)
        {
            var platformIns = Instantiate(platform.config.platformPrefab, container);
            platformIns.SetData(platform);
            var pos = new Vector2(platform.index, row - 4);
            platformIns.transform.SetLocalPositionAndRotation(pos, Quaternion.identity);
        }
    }

    private void AddInitPlatform()
    {
        platformsByRow.Add(rowIndex++, PlatformGenerator.GenerateFirstPlatforms());
        for (int i = 0; i < firstGenerateCount; i++)
        {
            platformsByRow.Add(rowIndex++, PlatformGenerator.GeneratePlatforms(platformsByRow[i], i % 2 == 0));
        }
    }

    public void SpawnNextPlatforms(bool moveLeft)
    {
        var listKeys = platformsByRow.Keys;
        var lastRow = listKeys.MaxBy(e => e);
        var lastJumpStepsByRow = platformsByRow[lastRow];
        platformsByRow.Add(rowIndex++, PlatformGenerator.GeneratePlatforms(lastJumpStepsByRow, moveLeft));
        platformsByRow = platformsByRow.OrderBy(e => e.Key).ToDictionary(obj => obj.Key, obj => obj.Value);
        container.RemoveAllChildren();

        foreach (var step in platformsByRow)
        {
            InstantiatePlatform(step.Value, step.Key * 2);
        }
    }

    public void RemoveFirstPlatform()
    {
        var listSteps = platformsByRow.Keys.ToList();
        var fistStepIdx = listSteps.Shift();
        platformsByRow.Remove(fistStepIdx);
    }
}

public class Platform
{
    public PlatformConfig config;
    public int index;

    public Platform(int index, PlatformConfig config)
    {
        this.config = config;
        this.index = index;
    }
}
