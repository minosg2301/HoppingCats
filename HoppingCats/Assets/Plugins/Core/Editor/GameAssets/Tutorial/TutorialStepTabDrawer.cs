using moonNest;

class TutorialStepTabDrawer : ListTabDrawer<TutorialStep>
{
    public int TutorialId { get; internal set; }

    public UITutorialDialog defaultDialog;
    public UITutorialInstruction instructionPrefab;

    public TutorialStepTabDrawer()
    {
        HideOnSwap = false;

        onElementAdded = OnStepAdded;
        onElementRemoved = OnStepRemoved;
        onSwapPerformed = OnStepSwapPerformed;
    }

    private void OnStepSwapPerformed(TutorialStep step1, TutorialStep step2)
    {
        var steps = TutorialAsset.Ins.steps;
        int index1 = steps.IndexOf(step1);
        int index2 = steps.IndexOf(step2);
        steps.Remove(step1);
        steps.Remove(step2);
        steps.Insert(index1, step2);
        steps.Insert(index2, step1);
    }

    private void OnStepRemoved(TutorialStep step) => TutorialAsset.Ins.Editor_RemoveStep(step);

    private void OnStepAdded(TutorialStep step)
    {
        step.tutorialId = TutorialId;
        TutorialAsset.Ins.steps.Add(step);
    }

    protected override TutorialStep CreateNewElement() => new TutorialStep($"Step {(List.Count + 1)}");

    protected override string GetTabLabel(TutorialStep element) => element.name;

    protected override void DoDrawContent(TutorialStep element)
    {
        element.name = Draw.TextField("Name", element.name, 200);

        Draw.BeginHorizontal();

        Draw.BeginVertical(600);
        {
            Draw.SpaceAndLabelBold("Dialog");
            element.dialogTitle = Draw.TextField("Title", element.dialogTitle, 400);
            element.dialogContent = Draw.TextField("Content", element.dialogContent, 400);
            element.customDialog = Draw.ObjectField("Custom Dialog", element.customDialog, 200);

            Draw.SpaceAndLabelBold("Instruction");
            element.instruction = Draw.TextField("Instruction", element.instruction, 400);
            element.showOverlay = Draw.ToggleField("Show Overlay", element.showOverlay, 80);
            //element.customInstruction = Draw.ObjectField("Custom UI", element.customInstruction, 200);

            Draw.SpaceAndLabelBold("Focus");
            element.skipZoomIn = Draw.ToggleField("Skip Anim", element.skipZoomIn, 80);
            element.latelyFocus = Draw.ToggleField("Lately Focus", element.latelyFocus, 80);
            element.latelyDelay = Draw.FloatField("Lately Delay Focus", element.latelyDelay, 80);

            Draw.SpaceAndLabelBold("Focus Overrided");
            Draw.BeginHorizontal();
            element.overridePadding = Draw.ToggleField("Padding", element.overridePadding, 80);
            Draw.Space();
            if (element.overridePadding) element.zoomPadding = Draw.Vector2(element.zoomPadding, 120);
            Draw.FlexibleSpace();
            Draw.EndHorizontal();

            Draw.BeginHorizontal();
            element.overridePosOffset = Draw.ToggleField("Position Offset", element.overridePosOffset, 80);
            Draw.Space();
            if (element.overridePosOffset) element.zoomPosOffset = Draw.Vector2(element.zoomPosOffset, 120);
            Draw.FlexibleSpace();
            Draw.EndHorizontal();

            Draw.BeginHorizontal();
            element.overrideAnchorPosOffset = Draw.ToggleField("AnchorPos Offset", element.overrideAnchorPosOffset, 80);
            Draw.Space();
            if (element.overrideAnchorPosOffset) element.zoomAnchorPosOffset = Draw.Vector2(element.zoomAnchorPosOffset, 120);
            Draw.FlexibleSpace();
            Draw.EndHorizontal();
        }
        Draw.EndVertical();

        Draw.Space(40);

        Draw.BeginVertical();
        {
            Draw.LabelBold("Setting");
            element.delayStart = Draw.FloatField("Delay Start", element.delayStart, 80);
            element.showHandFocus = Draw.ToggleField("Show Hand", element.showHandFocus, 80);
            if (element.showHandFocus)
                element.showMaskedFocus = Draw.ToggleField("Show Masked Focus", element.showMaskedFocus, 80);
            element.rewind = Draw.IntField("Rewind", element.rewind, 80);
            element.autoCloseStep = Draw.ToggleField("Auto Close Step", element.autoCloseStep, 80);
            if (element.autoCloseStep)
            {
                element.closeAfterSeconds = Draw.FloatField("Close After (s)", element.closeAfterSeconds, 80);
            }

            Draw.SpaceAndLabelBold("Next Step");
            element.autoNextStep = Draw.ToggleField("Auto Next", element.autoNextStep, 80);
            if (!element.autoNextStep)
            {
                //element.eventTrigger = Draw.IntPopupField("By Trigger", element.eventTrigger, GameDefinitionAsset.Ins.events, "name", "id", 120);
                element.actionTrigger = Draw.IntPopupField("By Action", element.actionTrigger, GameDefinitionAsset.Ins.actions, "name", "id", 120);
            }

            Draw.Space();
            element.saveStep = Draw.ToggleField("Save Step", element.saveStep, 80);
        }
        Draw.EndVertical();

        Draw.FlexibleSpace();

        Draw.EndHorizontal();
    }
}