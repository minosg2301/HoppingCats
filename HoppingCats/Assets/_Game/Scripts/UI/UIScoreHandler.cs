using UnityEngine;
using moonNest;
using TMPro;
using System;

public class UIScoreHandler : UIGroupByGameState
{
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI topScoreTxt;

    private int score;

    protected override void OnEnable()
    {
        base.OnEnable();
        GameEventManager.Ins.OnAddScore += AddScore;
        GameEventManager.Ins.OnEndGame += OnEndGame;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameEventManager.Ins.OnAddScore -= AddScore;
        GameEventManager.Ins.OnEndGame -= OnEndGame;
    }

    protected override void OnShow()
    {
        SetUpScore();
        base.OnShow();
    }

    public void SetUpScore()
    {
        score = 0;
        scoreTxt.SetText(score.ToString());
        SetTopScore();
    }

    public void AddScore()
    {
        score++;
        scoreTxt.SetText(score.ToString());
    }

    private void SetTopScore()
    {
        topScoreTxt.text = ("Top Score: " + UserSaveData.Ins.topScore);
    }

    private void OnEndGame()
    {
        CheckIfScoreIsHigherThanHighScore();
    }

    private void CheckIfScoreIsHigherThanHighScore()
    {
        if (score > UserSaveData.Ins.topScore)
        {
            UserSaveData.Ins.SetTopScore(score);    
        }
    }

}
