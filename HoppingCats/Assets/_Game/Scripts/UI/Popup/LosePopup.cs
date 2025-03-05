using Doozy.Engine.UI;
using System;
using UnityEngine;

public class LosePopup : BasePopupExt
{
    private static LosePopup Ins;

    public UIButton giveUpButton;
    public UIButton continueButton;

    public static LosePopup Show()
    {
        if(!Ins)
        {
            Ins = Show<LosePopup>("Lose", moonNest.PopupShowMethod.NO_QUEUE);
        }
        return Ins;
    }

    protected override void OnShow()
    {
        base.OnShow();

        giveUpButton.OnClick.OnTrigger.Event.AddListener(OnGiveUpClick);
        continueButton.OnClick.OnTrigger.Event.AddListener(OnContinueClick);
    }

    private void OnContinueClick()
    {
        TransitionEffectController.Ins.Show(() =>
        {
            GameEventManager.Ins.OnSetupLevel();
        });
        Hide();
    }

    private void OnGiveUpClick()
    {
        GameEventManager.Ins.OnEndGame();
        TransitionEffectController.Ins.Show(() =>
        {
            GameEventManager.Ins.OnSetupLevel();
        });
        Hide();
    }
}
