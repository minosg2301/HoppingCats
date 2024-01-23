using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGenerator
{
    private const int limitIndex = 20;

    public static List<JumpStepData> GenerateFirstJumpSteps()
    {
        List<JumpStepData> jumpSteps = new();
        for (int i = -limitIndex; i <= limitIndex; i += 2)
        {
            if (i == 0) jumpSteps.Add(GenerateFirstJumpStep(i));
            else
            {
                jumpSteps.Add(GenerateNoneJumpStep(i));
            }
        }
        return jumpSteps;
    }

    public static List<JumpStepData> GenerateJumpsStep(List<JumpStepData> prevJumpsStep, bool spawnLeft)
    {
        List<JumpStepData> jumpSteps = new();

        var prevCanSpawnJumpSteps = prevJumpsStep.FindAll(e => e.config.safeJumpType);

        var limitLeftIndex = prevJumpsStep.MinBy(e => e.index).index + (spawnLeft ? -1 : 1);
        var limitRightIndex = prevJumpsStep.MaxBy(e => e.index).index + (spawnLeft ? -1 : 1);

        foreach (var step in prevCanSpawnJumpSteps)
        {
            var leftIndex = step.index - 1;
            var rightIndex = step.index + 1;
            if ((leftIndex >= limitLeftIndex) && !jumpSteps.Exists(e => e.index == leftIndex)) jumpSteps.Add(GenerateSafeJumpStep(leftIndex));
            if ((rightIndex <= limitRightIndex) && !jumpSteps.Exists(e => e.index == rightIndex)) jumpSteps.Add(GenerateSafeJumpStep(rightIndex));
        }

        for (int i = limitLeftIndex; i <= limitRightIndex; i += 2)
        {
            if (!jumpSteps.Exists(e => e.index == i))
                jumpSteps.Add(GenerateRandomJumpStep(i));
        }

        return jumpSteps;
    }

    private static JumpStepData GenerateSafeJumpStep(int index)
    {
        var safeJumpSteps =  JumpManagerConfig.Ins.jumpStepConfigs.FindAll(e => e.safeJumpType && e.jumpType != JumpType.First);
        JumpStepData step = new(index, safeJumpSteps.Random());
        return step;
    }

    private static JumpStepData GenerateRandomJumpStep(int index)
    {
        var exceptFirstStep = JumpManagerConfig.Ins.jumpStepConfigs.FindAll(e => e.jumpType != JumpType.First);
        return new JumpStepData(index, exceptFirstStep.Random());
    }

    private static JumpStepData GenerateNoneJumpStep(int index)
    {
        return new JumpStepData(index, JumpManagerConfig.Ins.jumpStepConfigs.Find(e => e.jumpType == JumpType.None));
    }

    private static JumpStepData GenerateFirstJumpStep(int index)
    {
        return new JumpStepData(index, JumpManagerConfig.Ins.jumpStepConfigs.Find(e => e.jumpType == JumpType.First));
    }
}
