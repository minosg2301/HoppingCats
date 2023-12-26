using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIProgressBar : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI progressText;
    [Range(0, 1)]
    public float percent = 0.5f;
    public bool inverse;

    private RectTransform _rectTransform;
    public RectTransform RectTransform { get { if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }

    private void OnValidate()
    {
        if (image != null) image.fillAmount = Mathf.Clamp01(inverse && percent > 0 ? 1 - percent : percent);
    }

    public void SetPercent(float _percent)
    {
        percent = Mathf.Clamp01(_percent);
        image.fillAmount = Mathf.Clamp01(inverse && percent > 0 ? 1 - percent : percent);
        SetText(_percent.ToString("0%"));
    }

    public void SetPercent(float _percent, float animDuration, float delay = 0f, System.Action callback = null)
    {
        float lastPercent = percent;
        percent = Mathf.Clamp01(_percent);
        DOTween.To(() => lastPercent
        , val =>
        {
            image.fillAmount = val;
            SetText(val.ToString("0%"));
        }
        , percent, animDuration).SetDelay(delay)
        .OnComplete(() => callback?.Invoke());
    }

    public void SetText(string text)
    {
        if (progressText)
        {
            progressText.text = text;
        }
    }
}
