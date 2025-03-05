using UnityEngine;

public class FisrtLoadingHandler : MonoBehaviour
{
    private void Awake()
    {
        GameEventManager.Ins.OnFirstLoadingComplete += OnFisrtOpenGame;
    }

    private void OnFisrtOpenGame()
    {
        //Call transition eff
        TransitionEffectController.Ins.Show();
        GameEventManager.Ins.OnFirstLoadingComplete -= OnFisrtOpenGame;
    }
}
