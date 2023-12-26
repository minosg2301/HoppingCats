using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    [RequireComponent(typeof(Button))]
    public class UITutorialInstruction : MonoBehaviour
    {
        public GameObject overlay;
        public TextMeshProUGUI instructionText;
        public GameObject continueText;
        public float writingSpeed = 0.05f;

        public event Action OnClicked = delegate { };
        public event Action OnWritingEnded = delegate { };

        private Button _button;
        protected Button Button { get { if (!_button) _button = GetComponent<Button>(); return _button; } }

        public bool Writing { get; private set; } = false;

        string instruction;
        bool showContinueText;


        void Awake()
        {
            Button.onClick.AddListener(OnInstructionClicked);
        }

        void OnInstructionClicked()
        {
            if (Writing) DisplayImmediately();
            else OnClicked();
        }

        public void EnableClick(bool enabled)
        {
            Button.interactable = enabled;
            Button.targetGraphic.raycastTarget = enabled;
        }

        public void Show(string text, bool showOverlay)
        {
            if (showOverlay && !overlay)
            {
                Debug.LogError("Require show overlay Instruction but Overlay reference is not set!");
            }

            if (overlay)
            {
                overlay.SetActive(showOverlay);
            }

            gameObject.SetActive(true);
            showContinueText = true;
            EnableClick(true);

            DisplayInstruction(text);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ShowContinueText(bool show)
        {
            showContinueText = show;
            continueText.SetActive(true);
        }

        public void DisplayImmediately()
        {
            Writing = false;
            instructionText.text = instruction;
            continueText.SetActive(showContinueText && true);
            OnWritingEnded();
            StopAllCoroutines();
        }

        public void DisplayInstruction(string content)
        {
            Writing = true;
            instruction = content;
            StopAllCoroutines();
            continueText.SetActive(false);
            StartCoroutine(WriteInstruction());
        }

        IEnumerator WriteInstruction()
        {
            instructionText.text = "";
            foreach (char letter in instruction.ToCharArray())
            {
                instructionText.text += letter;
                yield return new WaitForSeconds(writingSpeed);
            }

            OnWritingEnded();
            continueText.SetActive(showContinueText && true);
            Writing = false;
        }
    }
}