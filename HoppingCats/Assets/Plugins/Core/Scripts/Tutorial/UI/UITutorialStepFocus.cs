using Doozy.Engine.UI;
using System;
using UnityEngine;

namespace moonNest
{
    public class UITutorialStepFocus : MonoBehaviour
    {
        [Header("UI")]
        public UIButton button;

        [Header("Size of Rect")]
        [Tooltip("Used when focus an object on scene")]
        public Vector2 size;

        [Header("Tutorial Steps")]
        [SerializeField] TutorialStepId[] tutorialStepIds;

        public event Action<UITutorialStepFocus> OnDestroyed = delegate { };

        private int[] _stepIds;
        public int[] StepIds { get { _stepIds ??= tutorialStepIds.Map(a => a.stepId).ToArray(); return _stepIds; } }

        private void Reset()
        {
            button = GetComponentInChildren<UIButton>();
        }

        private void OnEnable()
        {
            TutorialManager.Ins.RegisterStepFocus(this);
        }

        private void OnDisable()
        {
            TutorialManager.Ins.UnregisterStepFocus(this);
        }

        void OnDestroy()
        {
            OnDestroyed(this);
        }
    }
}