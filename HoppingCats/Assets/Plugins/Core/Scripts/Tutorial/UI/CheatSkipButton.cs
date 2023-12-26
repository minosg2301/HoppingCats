using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    [RequireComponent(typeof(Button))]
    public class CheatSkipButton : CheatButton
    {
        private Button button;
        public Button Button { get { if (!button) button = GetComponent<Button>(); return button; } }

        void Start()
        {
            Button.onClick.AddListener(() => TutorialManager.Ins.EndTutorial());
        }
    }
}