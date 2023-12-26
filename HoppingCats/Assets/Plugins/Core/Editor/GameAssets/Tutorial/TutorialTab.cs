using UnityEditor;
using UnityEngine;
using moonNest;

public class TutorialTab : TabContent
{
    TutorialDetailTabDrawer tutorialTabDrawer;

    public TutorialTab()
    {
        tutorialTabDrawer = new TutorialDetailTabDrawer();
    }

    public override void DoDraw()
    {
        Undo.RecordObject(TutorialAsset.Ins, "Tutorial");
        tutorialTabDrawer.DoDraw(TutorialAsset.Ins.tutorials);
        if (GUI.changed) Draw.SetDirty(TutorialAsset.Ins);
    }

    public override bool DoDrawWindow()
    {
        if (!base.DoDrawWindow())
        {
            return tutorialTabDrawer.DoDrawWindow();
        }
        return true;
    }
}