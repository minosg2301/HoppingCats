using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator
{
    private const int limitIndex = 20;

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


        foreach (var step in prevCanSpawnPlatforms)
        {
            var leftIndex = step.index - 1;
            var rightIndex = step.index + 1;

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
        var safeJumpSteps = PlatformConfigManager.Ins.platformConfigs.FindAll(e => e.isSafe && e.platformType != PlatformType.First);
        Platform step = new(index, safeJumpSteps.Random());
        return step;
    }

    private static Platform GenerateRandomPlatform(int index)
    {
        var exceptFirstStep = PlatformConfigManager.Ins.platformConfigs.FindAll(e => e.platformType != PlatformType.First);
        return new Platform(index, exceptFirstStep.Random());
    }

    private static Platform GenerateNonePlatform(int index)
    {
        return new Platform(index, PlatformConfigManager.Ins.platformConfigs.Find(e => e.platformType == PlatformType.None));
    }

    private static Platform GenerateFirstPlatform(int index)
    {
        return new Platform(index, PlatformConfigManager.Ins.platformConfigs.Find(e => e.platformType == PlatformType.First));
    }
}
