using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    private const int firstGenerateCount = 5;

    private Dictionary<int, List<Platform>> platformsByRow = new();
    private List<UIPlatform> uiPlatforms = new();

    private int rowIndex;

    public Transform container;
    public GameObject firstStep;

    public void Clear()
    {
        foreach(var uiPlatform in uiPlatforms)
        {
            Destroy(uiPlatform.gameObject);
        }
        uiPlatforms.Clear();

        rowIndex = 0;
        platformsByRow.Clear();
    }

    public void InitPlatforms()
    {
        platformsByRow.Add(rowIndex, PlatformGenerator.GenerateFirstPlatforms());

        for (int i = 0; i < firstGenerateCount; i++)
        {
            rowIndex++;
            platformsByRow.Add(rowIndex, PlatformGenerator.GeneratePlatforms(platformsByRow[i], i % 2 == 0));
        }

        foreach (var platform in platformsByRow)
        {
            InstantiatePlatform(platform.Value, platform.Key);
        }
    }

    private void InstantiatePlatform(List<Platform> platforms, int row)
    {
        foreach (var platform in platforms)
        {
            var platformIns = Instantiate(platform.config.platformPrefab, container);
            uiPlatforms.Add(platformIns);
            platformIns.SetData(platform, row);

            var pos = new Vector2(platform.index, row * 2 - 4);
            platformIns.transform.SetLocalPositionAndRotation(pos, Quaternion.identity);
        }
    }

    public void SpawnNextPlatforms(bool moveLeft)
    {
        var listKeys = platformsByRow.Keys;
        var lastRow = listKeys.MaxBy(e => e);
        var lastPlatformsByRow = platformsByRow[lastRow];

        var nextPlatfoms = PlatformGenerator.GeneratePlatforms(lastPlatformsByRow, moveLeft);

        rowIndex++;
        platformsByRow.Add(rowIndex, nextPlatfoms);

        platformsByRow = platformsByRow.OrderBy(e => e.Key).ToDictionary(obj => obj.Key, obj => obj.Value);

        InstantiatePlatform(nextPlatfoms, rowIndex);
    }

    public void RemovePlatforms()
    {
        if (platformsByRow.Count < 8) return;

        var listPlatformRow = platformsByRow.Keys.ToList();
        var platformRowRemoved = listPlatformRow.Shift();
        platformsByRow.Remove(platformRowRemoved);
        var platformRemoveds = uiPlatforms.FindAll(e => e.RowIndex == platformRowRemoved);
        uiPlatforms.RemoveAll(platformRemoveds);

        foreach (var uiPlatform in platformRemoveds)
        {
            Destroy(uiPlatform.gameObject);
        }
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
