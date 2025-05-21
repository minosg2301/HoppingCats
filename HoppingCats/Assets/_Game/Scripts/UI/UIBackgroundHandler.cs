using UnityEngine;
using UnityEngine.UI;

public class UIBackgroundHandler : MonoBehaviour
{
    public Image backgroundImage;

    private void OnEnable()
    {
        GameEventManager.Ins.OnSkinUpdate += OnSkinUpdated;
        OnSkinUpdated(SkinManager.Ins.CurrentSkinConfig);
    }

    private void OnDisable()
    {
        GameEventManager.Ins.OnSkinUpdate -= OnSkinUpdated;
    }

    private void OnSkinUpdated(SkinConfig skinConfig)
    {
        backgroundImage.sprite = skinConfig.bgSprite;
    }
}
