using DG.Tweening;
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

    private List<MoveType> listMove = new();
    private bool moving = false;

    private bool started = false;
    public void Prepare()
    {
        InitJumpSteps();
        InGameView.Ins.ShowIngameUI();
        listMove = new();
        moving = false;
        started = true;
    }

    private void InitJumpSteps()
    {
        jumpManager.Clear();
        jumpManager.InitJumpSteps();
    }

    private void GenerateNextJumpStep(bool moveLeft)
    {
        jumpManager.SpawnNextJumpSteps(moveLeft);
    }

    private void Update()
    {
        if (!started)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Prepare();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                AddMove(MoveType.Left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                AddMove(MoveType.Right);
            }

            DoMove();
        }
    }

    private void AddMove(MoveType type)
    {
        listMove.Add(type);
    }

    private void DoMove()
    {
        if (!moving && listMove.Count > 0)
        {
            moving = true;
            var type = listMove.Shift();

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
            GenerateNextJumpStep(type == MoveType.Left);
        }
    }

    private void MoveRight()
    {
        Turn(true);
        Vector3 targetPos = jumpManager.container.position;
        targetPos.x -= 1;
        targetPos.y -= 2;
        jumpManager.container.DOJump3D(targetPos, -jumpPower, jumpDuration).OnComplete(() => moving = false);
    }

    private void MoveLeft()
    {
        Turn(false);
        Vector3 targetPos = jumpManager.container.position;
        targetPos.x += 1;
        targetPos.y -= 2;
        jumpManager.container.DOJump3D(targetPos, -jumpPower, jumpDuration).OnComplete(() => moving = false);
    }
    private void Turn(bool faceRight)
    {
        Vector3 scale = cat.transform.localScale;
        if (faceRight) scale.x = -Mathf.Abs(scale.x);
        else scale.x = Mathf.Abs(scale.x);
        cat.transform.localScale = scale;
    }
    private enum MoveType
    {
        Left,
        Right
    }
}
