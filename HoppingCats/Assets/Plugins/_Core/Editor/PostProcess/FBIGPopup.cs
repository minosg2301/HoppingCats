using System;
using UnityEngine;
using moonNest;

public class FBIGPopup : BaseEditorWindow
{
    private string app_id;
    private string app_secret;
    private bool init;

    public event Action OnClose = delegate { };

    protected override UnityEngine.Object GetTarget()
    {
        return null;
    }

    protected override void OnDraw()
    {
        if(!init)
        {
            app_id = PlayerPrefs.GetString("fb_app_id", "");
            app_secret = PlayerPrefs.GetString("fb_app_secret", "");
            init = true;
        }


        Draw.LabelBold("Enter App ID and App Secret");
        app_id = Draw.TextField("App ID", app_id, 220);
        app_secret = Draw.TextField("App Secret", app_secret, 220);

        if (Draw.Button("Close", 140))
        {
            Close();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PlayerPrefs.SetString("fb_app_id", app_id);
        PlayerPrefs.SetString("fb_app_secret", app_secret);

        OnClose();
    }
}