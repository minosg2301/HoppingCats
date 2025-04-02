using UnityEngine;
using Doozy.Engine.Progress;
using moonNest;
using System;

[RequireComponent(typeof(TickInterval))]
public class UIHealthController : UIGroupByGameState
{
    public Progressor progressor;
    public TickInterval tickInterval;

    private float countdownSeconds = 30f;
    private float seconds;

    private bool counting;

    protected override void OnEnable()
    {
        base.OnEnable();
        tickInterval.onTick += OnTick;
        GameEventManager.Ins.OnGameLose += OnGameLose;
        GameEventManager.Ins.OnAddHealth += OnAddHealth;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        tickInterval.onTick -= OnTick;
        GameEventManager.Ins.OnGameLose -= OnGameLose;
        GameEventManager.Ins.OnAddHealth -= OnAddHealth;
    }

    protected override void OnShow()
    {
        base.OnShow();
        progressor.SetValue(1);

        seconds = Mathf.Max(0, countdownSeconds);
        counting = true;
        tickInterval.Restart();
    }

    private void OnAddHealth(int value)
    {
        if (value < 0) return;
        seconds = Mathf.Max(0, seconds + value);
        tickInterval.Restart();
    }
    private void OnGameLose()
    {
        counting = false;
        tickInterval.Pause();
    }

    private void OnTick()
    {
        if (seconds > 0)
        {
            seconds = Mathf.Max(0, seconds - tickInterval.TimeEslaped);
            progressor.SetValue(seconds / countdownSeconds);

        }
        else if (counting)
        {
            counting = false;
            progressor.SetValue(0);
            GameController.Ins.LoseHandle();
        }
    }
}
