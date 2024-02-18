using System;
using UnityEngine.Events;

namespace moonNest
{
    [Serializable]
    public class TutorialStepHandler
    {
        public TutorialStepId tutorialStep;
        public UnityEvent<TutorialStep> onEnter;
        public UnityEvent<TutorialStep> onExit;
    }
}