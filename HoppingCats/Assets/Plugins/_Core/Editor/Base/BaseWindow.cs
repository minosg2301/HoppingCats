using System;
using UnityEditor;
using UnityEngine;
using moonNest;

public abstract class BaseWindow
{
    private string name;
    private int id;
    protected Rect winRect = new Rect(10, 10, 200, 300);

    public delegate void WindowEvent(BaseWindow window);
    public WindowEvent OnClose;
    private bool firstShow = true;

    public bool Visible { get; protected set; } = false;

    public BaseWindow(int id, string name)
    {
        this.id = id;
        this.name = name;
    }

    protected void UpdateFirstShow()
    {
        firstShow = false;

        winRect.x = (Screen.width - winRect.width) / 2;
        winRect.y = (Screen.height - winRect.height) / 2;
    }

    public bool DoDraw()
    {
        if (Visible)
        {
            if (firstShow) UpdateFirstShow();

            winRect = GUI.Window(id, winRect, DrawWindow, name);
        }
        return Visible;
    }

    private void DrawWindow(int id)
    {
        if (Draw.ToolbarButton(new Rect(winRect.width - 32, 0, 32, 16), " x", Color.red, Color.white))
        {
            Visible = false;
            OnClose?.Invoke(this);
        }

        Draw.Space(6);

        BeginDrawWindow();
        OnDraw();
        EndDrawWindow();

        GUI.DragWindow();
    }

    Vector2 offset = Vector2.one * 32;
    Rect lastRect;
    private void BeginDrawWindow()
    {
        lastRect = GUILayoutUtility.GetLastRect();
    }

    private void EndDrawWindow()
    {
        Rect rect = GUILayoutUtility.GetLastRect();
        float diffY = rect.y + rect.height - (lastRect.y + lastRect.height) + offset.y;
        if (diffY > offset.y && winRect.height != diffY)
        {
            winRect.height = diffY;
            EditorWindow.focusedWindow.Repaint();
        }
    }

    public virtual void Show()
    {
        Visible = true;
        GUI.FocusWindow(id);
    }

    public virtual void Hide() => Visible = false;

    protected abstract void OnDraw();
}