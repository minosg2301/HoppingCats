using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameController : MonoBehaviour
{
    public JumpManager jumpManager;
    public GameObject cat;

    public float jumpPower = 1f;
    public float jumpDuration = 0.2f;

    private bool started = false;

    public void Prepare()
    {
        InitJumpSteps();
        started = true;
    }

    private void InitJumpSteps()
    {
        jumpManager.Clear();
        jumpManager.InitJumpSteps();
    }

    private void GenerateNextJumpStep()
    {

    }

    private void Update()
    {
        if (started)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                DoMove(MoveType.Left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                DoMove(MoveType.Right);
            }
        }
    }

    private void DoMove(MoveType type)
    {
        // Check status of cat

        // Cat do animation jump

        // Jump container do move
        switch (type)
        {
            case MoveType.Left:
                MoveLeft();
                break;
            case MoveType.Right:
                MoveRight();
                break;
        }

        // Process Jump Manager (Add - remove step, Gen new jump steps, ...)
        GenerateNextJumpStep();
    }

    private void MoveRight()
    {
        Vector3 targetPos = jumpManager.transform.position;
        targetPos.x -= 1;
        targetPos.y -= 1;
        jumpManager.transform.DOJump3D(targetPos, -jumpPower, jumpDuration);
    }

    private void MoveLeft()
    {
        Vector3 targetPos = jumpManager.transform.position;
        targetPos.x += 1;
        targetPos.y -= 1;
        jumpManager.transform.DOJump3D(targetPos, -jumpPower, jumpDuration);
    }

    private enum MoveType
    {
        Left,
        Right
    }
}
