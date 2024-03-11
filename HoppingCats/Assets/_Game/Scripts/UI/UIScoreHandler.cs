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

    public void SetTopScore()
    {
        topScoreTxt.text = ("Top Score: " );
    }

    

}
