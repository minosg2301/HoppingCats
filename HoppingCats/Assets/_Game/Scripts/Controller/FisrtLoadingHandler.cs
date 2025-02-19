using UnityEngine;

public class FisrtLoadingHandler : MonoBehaviour
{
    private void Awake()
    {
        GameBootComplete.onComplete += OnFisrtOpenGame;
    }

    private void OnFisrtOpenGame()
    {
        //Call transition eff
        TransitionEffectController.Ins.Show();
        GameBootComplete.onComplete -= OnFisrtOpenGame;
    }
}
