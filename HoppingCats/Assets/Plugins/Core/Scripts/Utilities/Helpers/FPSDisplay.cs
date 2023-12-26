using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;
    int line = 2;

    void OnGUI()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        int w = Screen.width, h = Screen.height;
        float textH = h * 2 / 140;
        var rect = new Rect(0, 0, w, textH);

        var boxRect = rect;
        boxRect.x = w - 350;
        boxRect.width = 350;
        boxRect.height = textH * line;
        GUI.Box(boxRect, "");
        var style = new GUIStyle { alignment = TextAnchor.UpperRight, fontSize = (int)textH };
        style.normal.textColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
        GUI.Label(rect, string.Format("{0:0.0} ms {1:0.} fps", msec, fps), style);

        rect.y += textH;
        GUI.Label(rect, string.Format("Size ({0}, {1})|DPI:{2}", w, h, Screen.dpi), style);
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
}
