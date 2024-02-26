using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JumpManager : MonoBehaviour
{
    private const int firstGenerateCount = 10;
    private const int rowCount = 20;

    private Dictionary<int, List<JumpStepData>> jumpStepsByRow;
    private List<List<JumpStepData>> jumpSteps;

    private int rowIndex;

    public Transform container;
    public GameObject firstStep;

    public void Clear()
    {
        container.RemoveAllChildren();
        jumpSteps = new();
        jumpStepsByRow = new();
        rowIndex = 0;
    }

    public void InitJumpSteps()
    {
        AddInitJumpStep();
        //foreach (var steps in jumpSteps)
        //{
        //    InstantiateJumpStep(steps, jumpSteps.IndexOf(steps) * 2);
        //}

        foreach (var step in jumpStepsByRow)
        {
            InstantiateJumpStep(step.Value, step.Key * 2);
        }
    }

    private void InstantiateJumpStep(List<JumpStepData> jumpSteps, int row)
    {
        foreach(var step in jumpSteps)
        {
            var stepIns = Instantiate(step.config.jumpStep, container);
            var pos = new Vector2(step.index, row - 4);
            stepIns.transform.SetLocalPositionAndRotation(pos, Quaternion.identity);
        }
    }

    private void AddInitJumpStep()
    {
        jumpStepsByRow.Add(rowIndex++, JumpGenerator.GenerateFirstJumpSteps());
        //jumpSteps.Add(JumpGenerator.GenerateFirstJumpSteps());
        for (int i = 0; i < firstGenerateCount; i++)
        {
            jumpStepsByRow.Add(rowIndex++, JumpGenerator.GenerateJumpsStep(jumpStepsByRow[i], i % 2 == 0));
            //jumpSteps.Add(JumpGenerator.GenerateJumpsStep(jumpSteps[i], i % 2 == 0));
        }
    }

    public void SpawnNextJumpSteps(bool moveLeft)
    {
        var listKeys = jumpStepsByRow.Keys;
        var lastRow = listKeys.MaxBy(e => e);
        var lastJumpStepsByRow = jumpStepsByRow[lastRow];
        jumpStepsByRow.Add(rowIndex++, JumpGenerator.GenerateJumpsStep(lastJumpStepsByRow, moveLeft));
        jumpStepsByRow = jumpStepsByRow.OrderBy(e => e.Key).ToDictionary(obj => obj.Key, obj => obj.Value);
        //jumpSteps.Add(JumpGenerator.GenerateJumpsStep(jumpSteps[jumpSteps.Count -1], moveLeft));
        container.RemoveAllChildren();
        //foreach (var steps in jumpSteps)
        //{
        //    InstantiateJumpStep(steps, jumpSteps.IndexOf(steps) * 2);
        //}

        foreach (var step in jumpStepsByRow)
        {
            InstantiateJumpStep(step.Value, step.Key * 2);
        }
    }

    public void RemoveFirstJumpStep()
    {
        var listSteps = jumpStepsByRow.Keys.ToList();
        var fistStepIdx = listSteps.Shift();
        jumpStepsByRow.Remove(fistStepIdx);
    }
}

public class JumpStepData
{
    public JumpConfig config;
    public int index;

    public JumpStepData(int index, JumpConfig config)
    {
        this.config = config;
        this.index = index;
    }
}
