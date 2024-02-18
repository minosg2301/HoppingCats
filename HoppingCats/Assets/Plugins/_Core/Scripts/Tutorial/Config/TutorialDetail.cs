using System;

namespace moonNest
{
    [Serializable]
    public class TutorialDetail : BaseData
    {
        public bool allowSkip;
        public bool resetOnSkip;
        public bool nonForce = false;
        public int dependOnTutorial = -1;
        public UITutorialDialog defaultDialog;
        public UITutorialInstruction defaultInstruction;

        public string trackingId = "";

        // for editor
        public bool drawTable = false;

        public TutorialDetail(string name) : base(name) { }
    }
}