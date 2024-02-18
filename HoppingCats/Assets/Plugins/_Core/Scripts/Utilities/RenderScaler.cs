using UnityEngine;
using moonNest;

public class RenderScaler : MonoBehaviour
{
    public Size designSize = new Size(750f, 1334f);

    float CalculateRenderScale()
    {
        if(Application.isEditor) return 1f;

        float min = 0.6f;
        float max = 1f;
        float ratio = Mathf.Clamp(min, designSize.width / Screen.width, max);
        ratio = Mathf.Lerp(0.7f, 1f, ratio);
        return ratio;
    }
}