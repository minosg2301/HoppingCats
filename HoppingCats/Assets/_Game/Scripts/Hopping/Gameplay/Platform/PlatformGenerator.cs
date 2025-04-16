using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator
{
    private const int limitIndex = 10;

    public static List<Platform> GenerateFirstPlatforms()
    {
        List<Platform> platforms = new();
        for (int i = -limitIndex; i <= limitIndex; i += 2)
        {
            if (i == 0) platforms.Add(GenerateFirstPlatform(i));
            else
            {
                platforms.Add(GenerateNonePlatform(i));
            }
        }
        return platforms;
    }

    public static List<Platform> GeneratePlatforms(List<Platform> prevPlatform, bool spawnLeft)
    {
        List<Platform> platforms = new();

        var prevCanSpawnPlatforms = prevPlatform.FindAll(e => e.config.isSafe);

        var limitLeftIndex = prevPlatform.MinBy(e => e.index).index + (spawnLeft ? -1 : 1);
        var limitRightIndex = prevPlatform.MaxBy(e => e.index).index + (spawnLeft ? -1 : 1);


        foreach (var platform in prevCanSpawnPlatforms)
        {
            var leftIndex = platform.index - 1;
            var rightIndex = platform.index + 1;

            bool canSpawnLeft = !platforms.Exists(e => e.index == leftIndex) && leftIndex >= limitLeftIndex;
            bool canSpawnRight = !platforms.Exists(e => e.index == rightIndex) && rightIndex <= limitRightIndex;
            bool haveLeftSafe = leftIndex < limitLeftIndex ? true : (!canSpawnLeft ? platforms.Find(e => e.index == leftIndex).config.isSafe : false);
            bool haveRightSafe = rightIndex > limitRightIndex ? true : (!canSpawnRight ? platforms.Find(e => e.index == rightIndex).config.isSafe : false);
            bool haveSafeStep = haveLeftSafe || haveRightSafe;

            if (haveSafeStep)
            {
                if (canSpawnLeft) platforms.Add(GenerateRandomPlatform(leftIndex));
                if (canSpawnRight) platforms.Add(GenerateRandomPlatform(rightIndex));
            }
            else
            {
                if (canSpawnLeft && canSpawnRight)
                {
                    var spawnSafeLeft = Random.Range(0, 2) == 0;
                    platforms.Add(GenerateSafePlatform(spawnSafeLeft ? leftIndex : rightIndex));
                    platforms.Add(GenerateRandomPlatform(spawnSafeLeft ? rightIndex : leftIndex));
                }
                else if (canSpawnLeft)
                {
                    platforms.Add(GenerateSafePlatform(leftIndex));
                }
                else if (canSpawnRight)
                {
                    platforms.Add(GenerateSafePlatform(rightIndex));
                }
            }
        }

        for (int i = limitLeftIndex; i <= limitRightIndex; i += 2)
        {
            if (!platforms.Exists(e => e.index == i))
                platforms.Add(GenerateRandomPlatform(i));
        }

        return platforms;
    }

    private static Platform GenerateSafePlatform(int index)
    {
        var safePlatform = LevelManager.Ins.GetPlatformConfig(PlatformType.Normal);
        Platform platform = new(index, safePlatform);
        return platform;
    }

    private static Platform GenerateRandomPlatform(int index)
    {
        var randomPlatformConfig = LevelManager.Ins.RandomPlatform();
        return new Platform(index, randomPlatformConfig);
    }

    private static Platform GenerateNonePlatform(int index)
    {
        return new Platform(index, LevelManager.Ins.GetPlatformConfig(PlatformType.None));
    }

    private static Platform GenerateFirstPlatform(int index)
    {
        return new Platform(index, LevelManager.Ins.GetPlatformConfig(PlatformType.First));
    }
}
