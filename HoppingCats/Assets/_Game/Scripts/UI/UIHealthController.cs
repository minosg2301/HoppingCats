using UnityEngine;
using Doozy.Engine.Progress;

public class UIHealthController : UIGroupByGameState
{
    public Progressor progressor;

    private float countdownTime = 13f;
    private float timer;

    private bool isCountDown;

    protected override void OnEnable()
    {
        base.OnEnable();
        GameEventManager.Ins.OnGameLose += OnGameLose;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameEventManager.Ins.OnGameLose -= OnGameLose;
    }

    protected override void OnShow()
    {
        base.OnShow();
        timer = countdownTime;
        progressor.SetValue(1);
        isCountDown = true;
    }
    private void OnGameLose()
    {
        isCountDown = false;
    }

    void Update()
    {
        if (!isCountDown) return;

        if (timer > 0)
        {
            timer -= Time.deltaTime; 
            progressor.SetValue(timer / countdownTime); 
        }
        else
        {
            progressor.SetValue(0);
            GameController.Ins.LoseHandle();
        }
    }
}
