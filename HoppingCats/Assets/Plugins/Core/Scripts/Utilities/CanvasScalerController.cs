using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasScalerController : MonoBehaviour
{
    CanvasScaler _canvasScaler;
    // Start is called before the first frame update
    void Start()
    {
        _canvasScaler = this.GetComponent<CanvasScaler>();
        float ratio = Screen.height / Screen.width;
        if (ratio > 1.8f) _canvasScaler.matchWidthOrHeight = 0;
        else _canvasScaler.matchWidthOrHeight = 1;
    }
}