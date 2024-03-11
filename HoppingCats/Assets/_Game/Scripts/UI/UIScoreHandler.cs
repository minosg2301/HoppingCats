using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
using TMPro;
public class UIScoreHandler : SingletonMono<UIScoreHandler>
{
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI topScoreTxt;

    public int score;

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

    public void OnRunEnd()
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
