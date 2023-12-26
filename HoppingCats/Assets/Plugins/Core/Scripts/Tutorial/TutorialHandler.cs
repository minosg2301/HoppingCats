using UnityEngine;

namespace moonNest
{
    public class TutorialHandler : MonoBehaviour
    {
        public TutorialStepHandler[] stepHandlers;

        protected virtual void OnEnable()
        {
            TutorialManager.Ins.RegisterHandlers(stepHandlers);
        }

        protected virtual void OnDisable()
        {
            TutorialManager.Ins.UnregisterHandlers(stepHandlers);
        }
    }
}