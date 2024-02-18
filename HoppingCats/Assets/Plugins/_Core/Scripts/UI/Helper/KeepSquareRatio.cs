using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeepSquareRatio : UIBehaviour
{
    public bool isHorizontalScreen = false;

    private RectTransform _rect;
    public RectTransform RectTransform { get { if (!_rect) _rect = GetComponent<RectTransform>(); return _rect; } }

    private UICanvas _uiCanvas;
    public UICanvas UICanvas { get { if (!_uiCanvas) _uiCanvas = GetComponentInParent<UICanvas>(); return _uiCanvas; } }

    private RectTransform _uiCanvasRect;
    public RectTransform UICanvasRect { get { if (!_uiCanvasRect) _uiCanvasRect = UICanvas.GetComponent<RectTransform>(); return _uiCanvasRect; } }

    bool selfApply;

    protected override void OnRectTransformDimensionsChange()
    {
        if (selfApply || !enabled) return;

        selfApply = true;
        var rect = UICanvasRect;

        float height = rect.sizeDelta.y;
        float width = rect.sizeDelta.x;
        var sizeDelta = RectTransform.sizeDelta;

        if (!isHorizontalScreen)
        {
            sizeDelta.x = height;
        }
        else
        {
            sizeDelta.y = width;
        }
        RectTransform.sizeDelta = sizeDelta;
        RectTransform.anchoredPosition = Vector2.zero;

        selfApply = false;
    }
}