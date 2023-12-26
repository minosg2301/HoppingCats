using DG.Tweening;
using Doozy.Engine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest
{
    public class TutorialManager : SingletonMono<TutorialManager>
    {
        [SerializeField] private GameObject lockTouchMask;
        [SerializeField] private GameObject container;
        [SerializeField] private UITutorialHand hand;
        [SerializeField] private UITutorialFocus focus;
        [SerializeField] private UIButton skipButton;

        private RectTransform _rect;
        public RectTransform RectTransform { get { if (_rect == null) _rect = GetComponent<RectTransform>(); return _rect; } }

        public Camera CameraUI => UICanvas.GetMasterCanvas().Canvas.worldCamera;
        public bool IsTutorialPlaying => tutorial != null;
        public bool StepRewinding { get; private set; }

        UITutorialInstruction instruction;
        UITutorialDialog dialog;
        UIButton focusedButton;

        TutorialDetail tutorial;
        List<TutorialStep> tutorialSteps;
        TutorialStep currentStep;
        int rewind;
        int stepCount;

        List<TutorialStepHandler> stepHandlers = new List<TutorialStepHandler>();
        List<UITutorialStepFocus> stepFocuses = new List<UITutorialStepFocus>();

        public event Action<TutorialDetail> OnTutorialStart = delegate { };
        public event Action<TutorialDetail> OnTutorialEnd = delegate { };
        public event Action<TutorialStep> OnTutorialStepEnter = delegate { };
        public event Action<TutorialStep> OnTutorialStepExit = delegate { };

        #region mono methods
        protected override void Awake()
        {
            base.Awake();

            container.SetActive(false);
            if (skipButton)
                skipButton.OnClick.OnTrigger.Event.AddListener(EndTutorial);
        }
        #endregion

        #region main flows
        public void StartTutorial(int tutorialId)
        {
            if (tutorial != null)
            {
                if (tutorial.id != tutorialId)
                {
                    var newTutorial = TutorialAsset.Ins.FindTutorial(tutorialId);
                    Debug.LogError($"Tutorial {tutorial.name} is running. Can not start tutorial {newTutorial.name}");
                }
                return;
            }

#if UNITY_EDITOR
            UserTutorial.Ins.ResetTutorial(tutorialId);
#endif

            tutorial = TutorialAsset.Ins.FindTutorial(tutorialId);
            tutorialSteps = TutorialAsset.Ins.FindSteps(tutorialId).ToList();

            InitUI();

            stepCount = 0;
            skipButton.gameObject.SetActive(tutorial.allowSkip);
            container.SetActive(true);
            focus.Hide();
            hand.Hide();

            OnTutorialStart(tutorial);

            var currentStepId = UserTutorial.Ins.GetCurrentStep(tutorialId);
            int index = tutorialSteps.IndexOf(s => s.id == currentStepId);
            if (index != -1)
            {
                tutorialSteps = tutorialSteps.SubList(index);
            }
            _ProcessNextStep(tutorialSteps.Shift());
        }

        public void ProcessNextStep(int tutorialId)
        {
            if (!IsPlaying(tutorialId)) return;

            _ProcessNextStep(tutorialSteps.Shift());
        }

        void _ProcessNextStep(TutorialStep nextStep)
        {
            if (nextStep.instruction.Length == 0 && instruction)
            {
                instruction.Hide();
            }

            stepCount++;
            rewind = nextStep.rewind;
            StepRewinding = false;
            StartCoroutine(ProcessToStep(nextStep));
        }

        IEnumerator ProcessToStep(TutorialStep step)
        {
            currentStep = step;

            if (currentStep.saveStep)
            {
                UserTutorial.Ins.SetCurrentStep(tutorial.id, currentStep.id);
            }

            lockTouchMask.SetActive(true);

            // excute on next frame
            yield return new WaitForSeconds(currentStep.delayStart);

            lockTouchMask.SetActive(false);

            // update ui
            if ((currentStep.dialogTitle.Length > 0 || currentStep.dialogContent.Length > 0))
            {
                var _dialog = currentStep.customDialog ? CreateCustomDialog(currentStep.customDialog) : dialog;
                _dialog.Show(currentStep.dialogTitle, currentStep.dialogContent);
                if (currentStep.autoCloseStep)
                {
                    _dialog.HideAfter(currentStep.closeAfterSeconds);
                }
            }
            else
            {
                if (instruction && currentStep.instruction.Length > 0)
                {
                    instruction.Show(currentStep.instruction, currentStep.showOverlay);
                }

                //focus button
                var stepFocus = stepFocuses.Find(b => b.StepIds.Contains(currentStep.id));
                if (stepFocus)
                {
                    SetStepFocus(stepFocus);
                }
                else if (currentStep.autoCloseStep)
                {
                    DOVirtual.DelayedCall(currentStep.closeAfterSeconds, CloseStep);
                }
            }

            // notify enter
            NotifyStepEnter();
        }

        public void CloseStep()
        {
            NotifyStepExit();

            focus.Hide();
            hand.Hide();

            if (focusedButton)
            {
                focusedButton.OnClick.OnTrigger.Event.RemoveListener(OnFocusButtonClicked);
                focusedButton = null;
            }

            if (tutorialSteps.Count > 0)
            {
                if (currentStep.autoNextStep)
                {
                    _ProcessNextStep(tutorialSteps.Shift());
                }
                else
                {
                    if (instruction) instruction.Hide();

                    if (currentStep.actionTrigger != -1)
                    {
                        CoreHandler.OnGameActionEvent -= HandleGameAction;
                        CoreHandler.OnGameActionEvent += HandleGameAction;
                    }
                }
            }
            else
            {
                EndTutorial();
            }
        }

        internal void EndTutorial()
        {
            if (instruction) instruction.Hide();

            hand.Hide();
            focus.Hide();
            skipButton.gameObject.SetActive(false);

            if (focusedButton)
            {
                focusedButton.OnClick.OnTrigger.Event.RemoveListener(OnFocusButtonClicked);
                focusedButton = null;
            }

            int tutorialId = tutorial.id;
            OnTutorialEnd(tutorial);
            UserTutorial.Ins.OnTutorialEnd(tutorialId);
            tutorial = null;
            currentStep = null;

            //var nextTutorial = TutorialAsset.Ins.FindTutorialDependOn(tutorialId);
            //if (nextTutorial)
            //{
            //    DOVirtual.DelayedCall(1f, () => StartTutorial(nextTutorial.id));
            //}
        }
        #endregion

        #region public methods
        public void RegisterStepFocus(UITutorialStepFocus stepFocus)
        {
            stepFocus.OnDestroyed += OnStepFocusDestroyed;
            stepFocuses.Add(stepFocus);

            if (currentStep != null && currentStep.latelyFocus && stepFocus.StepIds.Contains(currentStep.id))
            {
                lockTouchMask.SetActive(true);
                StartCoroutine(ISetStepFocus(stepFocus, currentStep.latelyDelay));
            }
        }

        public void UnregisterStepFocus(UITutorialStepFocus stepFocus)
        {
            stepFocus.OnDestroyed -= OnStepFocusDestroyed;
            stepFocuses.Remove(stepFocus);
        }

        public void RegisterHandlers(TutorialStepHandler[] handlers)
        {
            stepHandlers.AddRange(handlers);
        }

        public void UnregisterHandlers(TutorialStepHandler[] handlers)
        {
            foreach (var handler in handlers)
            {
                stepHandlers.Remove(handler);
            }
        }

        public void FocusToPosition(Vector2 screenPosition, Vector2 focusSize)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, screenPosition, CameraUI, out var uiPosition);

            focus.SetStepConfig(currentStep);
            focus.FocusToPosition(uiPosition, focusSize);

            if (currentStep.showHandFocus)
            {
                hand.Show(uiPosition);
            }
        }

        public void FocusToTarget(RectTransform target)
        {
            focusedButton = target.GetComponent<UIButton>();
            if (focusedButton)
            {
                focusedButton.OnClick.OnTrigger.Event.RemoveListener(OnFocusButtonClicked);
                focusedButton.OnClick.OnTrigger.Event.AddListener(OnFocusButtonClicked);
            }

            focus.SetStepConfig(currentStep);
            focus.FocusToTarget(target);
            if (currentStep.showHandFocus)
            {
                hand.Show(target);
            }
        }

        public void PlayHandDrag(Vector2 from, Vector2 to, float duration)
        {
            if (instruction) instruction.EnableClick(false);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, from, CameraUI, out from);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, to, CameraUI, out to);
            hand.PlayDrag(from, to, duration);
        }

        public void ShowInstructionTapToContinue(bool show)
        {
            if (instruction) instruction.ShowContinueText(show);
        }

        public bool IsPlaying(TutorialId tutorialId)
        {
            return tutorial && tutorial.id == tutorialId;
        }

        public bool IsPlayingStep(TutorialId tutorialId, TutorialStepId stepId)
        {
            return currentStep != null && currentStep.tutorialId == tutorialId && currentStep.id == stepId;
        }
        #endregion

        #region private methods
        void InitUI()
        {
            if (tutorial.defaultDialog)
            {
                if (dialog)
                {
                    Destroy(dialog.gameObject);
                }

                dialog = Instantiate(tutorial.defaultDialog, container.transform);
                dialog.gameObject.SetActive(false);
                dialog.OnHide += CloseStep;
            }
            else
            {
                Debug.LogError($"Tutorial {tutorial.name} isn't set default dialog");
            }

            if (tutorial.defaultInstruction)
            {
                if (instruction)
                {
                    Destroy(instruction.gameObject);
                }

                instruction = Instantiate(tutorial.defaultInstruction, container.transform);
                instruction.gameObject.SetActive(false);
                instruction.OnWritingEnded += OnInstructionWritingSkipped;
                instruction.OnClicked += CloseStep;
            }
            else
            {
                Debug.LogError($"Tutorial {tutorial.name} isn't set default instruction");
            }

            skipButton.transform.SetAsLastSibling();
        }

        UITutorialDialog CreateCustomDialog(UITutorialDialog customDialogPrefab)
        {
            var dialog = Instantiate(customDialogPrefab, container.transform);
            dialog.OnHide += () =>
            {
                Destroy(dialog.gameObject);
                CloseStep();
            };
            return dialog;
        }

        void OnInstructionWritingSkipped()
        {
            if (focusedButton)
            {
                instruction.EnableClick(false);
            }
        }

        void OnStepFocusDestroyed(UITutorialStepFocus stepFocus)
        {
            stepFocuses.Remove(stepFocus);
        }

        IEnumerator ISetStepFocus(UITutorialStepFocus stepFocus, float delay)
        {
            yield return new WaitForSeconds(delay);
            lockTouchMask.SetActive(false);
            SetStepFocus(stepFocus);
        }

        void SetStepFocus(UITutorialStepFocus stepFocus)
        {
            focusedButton = stepFocus.button;
            if (focusedButton)
            {
                focusedButton.OnClick.OnTrigger.Event.RemoveListener(OnFocusButtonClicked);
                focusedButton.OnClick.OnTrigger.Event.AddListener(OnFocusButtonClicked);
            }

            focus.SetStepConfig(currentStep);

            var rectTransform = stepFocus.GetComponent<RectTransform>();
            if (rectTransform)
            {
                focus.FocusToTarget(rectTransform);
            }
            else
            {
                focus.FocusToPosition(stepFocus.transform.position, stepFocus.size);
            }

            if (currentStep.showHandFocus)
            {
                hand.Show(stepFocus.transform);
            }
        }

        void OnFocusButtonClicked()
        {
            if (--rewind <= 0)
            {
                CloseStep();
            }
            else
            {
                StepRewinding = true;
                NotifyStepEnter();
            }
        }

        void NotifyStepEnter()
        {
            OnTutorialStepEnter(currentStep);

            var handlers = stepHandlers.FindAll(b => b.tutorialStep.stepId == currentStep.id);
            foreach (var handler in handlers)
            {
                handler.onEnter.Invoke(currentStep);
            }
        }

        void NotifyStepExit()
        {
            OnTutorialStepExit(currentStep);

            var handlers = stepHandlers.FindAll(b => b.tutorialStep.stepId == currentStep.id);
            foreach (var handler in handlers)
            {
                handler.onExit.Invoke(currentStep);
            }
        }

        void HandleGameAction(ActionData action)
        {
            if (currentStep && currentStep.actionTrigger == action.id)
            {
                CoreHandler.OnGameActionEvent -= HandleGameAction;
                _ProcessNextStep(tutorialSteps.Shift());
            }
        }
        #endregion
    }
}