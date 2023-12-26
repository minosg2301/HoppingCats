using UnityEngine;

public class UITabContent : MonoBehaviour
{
    private RectTransform _rect;
    public RectTransform RectTransform { get { if(!_rect) _rect = GetComponent<RectTransform>(); return _rect; } }

    internal void Show(bool show)
    {
        gameObject.SetActive(show);
    }
}
