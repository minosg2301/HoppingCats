using System.Collections;
using UnityEngine;

namespace moonNest
{
    public class UITutorialTrigger : MonoBehaviour
    {
        public bool activeOnStart;
        public TutorialId tutorialId;

        protected virtual void OnEnable()
        {
            if (!activeOnStart)
                return;

            StartCoroutine(_TriggerUpdate());
        }

        protected virtual void OnDisable()
        {
        }

        public void TriggerUpdate()
        {
            StartCoroutine(_TriggerUpdate());
        }

        private IEnumerator _TriggerUpdate()
        {
            yield return null;

            if (!CheckCondition())
            {
                yield break;
            }

            if (!UserTutorial.Ins.IsTutorialPlayed(tutorialId))
            {
                TutorialManager.Ins.StartTutorial(tutorialId);
            }
        }

        protected virtual bool CheckCondition()
        {
            var tutorialConfig = TutorialAsset.Ins.FindTutorial(tutorialId);
            if (tutorialConfig && tutorialConfig.dependOnTutorial != -1)
            {
                if (!UserTutorial.Ins.IsTutorialPlayed(tutorialConfig.dependOnTutorial))
                    return false;
            }
            return true;
        }
    }
}