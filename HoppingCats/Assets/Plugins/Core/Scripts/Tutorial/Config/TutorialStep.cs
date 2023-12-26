using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class TutorialStep : BaseData
    {
        public int tutorialId;
        public float delayStart;

        public string dialogTitle;
        public string dialogContent;
        public UITutorialDialog customDialog;

        public string instruction;
        public UITutorialInstruction customInstruction;

        public bool showOverlay = true;
        public bool showHandFocus = true;
        public bool showMaskedFocus = true;
        public bool autoCloseStep;
        public float closeAfterSeconds;
        public int rewind = 0;
        public bool saveStep = false;

        public bool autoNextStep = true;
        public int eventTrigger = -1;
        public int actionTrigger = -1;

        public bool latelyFocus;
        public float latelyDelay;
        public bool skipZoomIn;
        public bool overridePadding;
        public Vector2 zoomPadding;
        public bool overridePosOffset;
        public Vector2 zoomPosOffset;
        public bool overrideAnchorPosOffset;
        public Vector2 zoomAnchorPosOffset;

        public TutorialStep(string name) : base(name) { }
    }
}