using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGenerator
{
    public static List<JumpStep> GenerateJumpsStep(List<JumpStep> prevJumpsStep, bool spawnLeft)
    {
        List<JumpStep> jumpSteps = new();

        var prevCanSpawnJumpSteps = prevJumpsStep.FindAll(e => e.config.safeJumpType);

        var limitLeftIndex = prevJumpsStep.MinBy(e => e.index).index + (spawnLeft ? -1 : 1);
        var limitRightIndex = prevJumpsStep.MaxBy(e => e.index).index + (spawnLeft ? -1 : 1);

        foreach (var step in prevCanSpawnJumpSteps)
        {
            var leftIndex = step.index - 1;
            var rightIndex = step.index + 1;
            if (!jumpSteps.Exists(e => e.index == leftIndex)) jumpSteps.Add(GenerateSafeJumpStep(leftIndex));
            if (!jumpSteps.Exists(e => e.index == rightIndex)) jumpSteps.Add(GenerateSafeJumpStep(rightIndex));
        }

        for (int i = limitLeftIndex; i <= limitRightIndex; i++)
        {
            if (!jumpSteps.Exists(e => e.index == i))
                jumpSteps.Add(GenerateRandomJumpStep(i));
        }

        return jumpSteps;
    }

    private static JumpStep GenerateSafeJumpStep(int index)
    {
        var safeJumpSteps =  JumpManagerConfig.Ins.jumpSteepConfigs.FindAll(e => e.safeJumpType);
        JumpStep step = new(index, safeJumpSteps.Random());
        return step;
    }

    private static JumpStep GenerateRandomJumpStep(int index)
    {
        return new JumpStep(index, JumpManagerConfig.Ins.jumpSteepConfigs.Random());
    }
}
