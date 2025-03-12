using UnityEngine;
using UnityEngine.EventSystems;

public class UIGameDetect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameController.Ins.isHoverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameController.Ins.isHoverUI = false;
    }
}
