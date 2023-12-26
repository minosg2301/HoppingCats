using UnityEngine;
using TMPro;

public class UIProgress : MonoBehaviour
{
    public TextMeshProUGUI progressText;
    public UIProgressBar progressBar;

    public void SetText(string content)
    {
        if (progressText) progressText.text = content;
    }

    public void SetProgress(int progress, int require, float duration = 0)
    {
        if (progressText) progressText.text = $"{progress} / {require}";
        if (progressBar) progressBar.SetPercent((float)progress / require, duration);
    }
}