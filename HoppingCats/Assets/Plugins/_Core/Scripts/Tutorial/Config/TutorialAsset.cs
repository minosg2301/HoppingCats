using System.Collections.Generic;

namespace moonNest
{
    public class TutorialAsset : SingletonScriptObject<TutorialAsset>
    {
        public List<TutorialDetail> tutorials = new List<TutorialDetail>();
        public List<TutorialStep> steps = new List<TutorialStep>();

        public TutorialDetail FindTutorial(int tutoriaId) => tutorials.Find(turorial => turorial.id == tutoriaId);
        public List<TutorialStep> FindSteps(int tutorialId) => steps.FindAll(step => step.tutorialId == tutorialId);
        public TutorialStep FindStep(int stepId) => steps.Find(step => step.id == stepId);

#if UNITY_EDITOR
        public void Editor_RemoveStep(TutorialStep step)
        {
            steps.Remove(step);
        }
#endif

        internal TutorialDetail FindTutorialDependOn(int tutorialId)
        {
            return tutorials.Find(t => t.dependOnTutorial == tutorialId);
        }
    }
}