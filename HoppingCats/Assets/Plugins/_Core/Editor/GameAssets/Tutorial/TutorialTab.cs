using UnityEditor;
using UnityEngine;
using moonNest;

public class TutorialTab : TabContent
{
    TutorialDetailTabDrawer tutorialTabDrawer;

    public TutorialTab()
    {
        tutorialTabDrawer = new TutorialDetailTabDrawer();
        tutorialTabDrawer.onElementCloned += OnTutorialCloned;
    }

    private void OnTutorialCloned(TutorialDetail newTutorial, TutorialDetail origin)
    {
        var originSteps = TutorialAsset.Ins.FindSteps(origin.id);
        foreach (var step in originSteps)
        {
            var newStep = step.Clone() as TutorialStep;
            newStep.tutorialId = newTutorial.id;
            TutorialAsset.Ins.steps.Add(newStep);
        }
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