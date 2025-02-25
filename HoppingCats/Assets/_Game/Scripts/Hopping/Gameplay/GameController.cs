using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlatformManager platformManager;
    public CatController cat;

    

    private bool isMoving = false;
    private bool isIngame = false;

    private void Awake()
    {
        StartGame();
    }

    private void Update()
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


    public void StartGame()
    {
        if (isIngame) return;

        InitPlatforms();
        //InGameView.Ins.ShowIngameUI();
        //UIScoreHandler.Ins.SetUpScore();
        isMoving = false;
        isIngame = true;
    }

    private void InitPlatforms()
    {
        platformManager.Clear();
        platformManager.InitPlatforms();
    }

    private void DoMove(MoveType moveType)
    {
        if (cat.IsMoving) return;

        if(moveType == MoveType.Left)
        {
            cat.MoveLeft();
        }
        else
        {
            cat.MoveRight();
        }

        GenerateNextJumpStep(moveType == MoveType.Left);
    }

    private void GenerateNextJumpStep(bool moveLeft)
    {
        platformManager.RemoveFirstPlatform();
        platformManager.SpawnNextPlatforms(moveLeft);
    }

    
    
}
