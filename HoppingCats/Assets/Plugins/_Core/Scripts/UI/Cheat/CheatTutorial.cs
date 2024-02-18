using Doozy.Engine.UI;
using System;
using TMPro;
using UnityEngine.UI;

namespace moonNest
{
    public class CheatTutorial : CheatButton
    {
        public UIButton replayButton;
        public TMP_Dropdown dropdown;

        protected override void Awake()
        {
            base.Awake();
            replayButton.OnClick.OnTrigger.Event.AddListener(OnReplayClick);
        }

        void OnEnable()
        {
            dropdown.ClearOptions();
            foreach (var tut in TutorialAsset.Ins.tutorials)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(tut.name));
            }

        }

        void OnReplayClick()
        {
            int index = dropdown.value;
            if (index > -1 && TutorialAsset.Ins.tutorials.Count > index)
            {
                UserTutorial.Ins.ResetTutorial(TutorialAsset.Ins.tutorials[index].id);
            }
        }
    }
}