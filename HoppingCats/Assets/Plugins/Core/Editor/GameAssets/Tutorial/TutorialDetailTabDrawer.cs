using moonNest;

class TutorialDetailTabDrawer : ListTabDrawer<TutorialDetail>
{
    TutorialStepTabDrawer tutorialStepTabDrawer;
    private TableDrawer<TutorialStep> stepTable;

    public TutorialDetailTabDrawer()
    {
        tutorialStepTabDrawer = new TutorialStepTabDrawer();

        stepTable = new TableDrawer<TutorialStep>();
        stepTable.AddCol("Name", 200, ele => ele.name = Draw.Text(ele.name, 200));
        stepTable.onOrderChanged = OnOrderChanged;
        stepTable.elementCreator = () => new TutorialStep("New Step");
    }

    private void OnOrderChanged(TutorialStep step1, TutorialStep step2)
    {
        var steps = TutorialAsset.Ins.steps;
        int index1 = steps.IndexOf(step1);
        int index2 = steps.IndexOf(step2);
        (steps[index2], steps[index1]) = (steps[index1], steps[index2]);
    }

    protected override TutorialDetail CreateNewElement() => new TutorialDetail("Tutorial " + (TutorialAsset.Ins.tutorials.Count + 1));

    protected override void DoDrawContent(TutorialDetail element)
    {
        Draw.BeginHorizontal();

        Draw.BeginVertical();
        Draw.BeginDisabledGroup(true);
        element.id = Draw.IntField("ID", element.id, 200);
        Draw.EndDisabledGroup();
        element.name = Draw.TextField("Name", element.name, 200);
        element.defaultDialog = Draw.ObjectField("Step Dialog", element.defaultDialog, 200);
        element.defaultInstruction = Draw.ObjectField("Step Instruction", element.defaultInstruction, 200);
        element.drawTable = Draw.ToggleField("Draw Table", element.drawTable, 100);
        Draw.EndVertical();

        Draw.Space(60);
        Draw.BeginVertical();
        element.allowSkip = Draw.ToggleField("Allow Skip", element.allowSkip, 80);
        element.dependOnTutorial = Draw.IntPopupField("Depend On", element.dependOnTutorial, TutorialAsset.Ins.tutorials, "name", "id");
        Draw.EndVertical();

        Draw.Space(60);
        Draw.BeginVertical();
        element.trackingId = Draw.TextField("Tracking Id", element.trackingId, 100);
        Draw.EndVertical();

        Draw.FlexibleSpace();

        Draw.EndHorizontal();

        Draw.Space();
        var steps = TutorialAsset.Ins.FindSteps(element.id);
        if (element.drawTable)
        {
            stepTable.DoDraw(steps);
        }
        else
        {
            tutorialStepTabDrawer.TutorialId = element.id;
            tutorialStepTabDrawer.DoDraw(steps);
        }
    }

    protected override string GetTabLabel(TutorialDetail element) => element.name;

    public override bool DoDrawWindow()
    {
        return base.DoDrawWindow() || tutorialStepTabDrawer.DoDrawWindow();
    }
}