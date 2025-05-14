using UnityEngine;
using UnityEngine.EventSystems;

public class UIGameDetect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameController.Ins)
        {
            GameController.Ins.Pause(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameController.Ins)
        {
            GameController.Ins.Pause(false);
        }
    }
}
