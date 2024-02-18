using System;
using Doozy.Engine.UI;
using UnityEngine;

namespace moonNest
{
    public class UILuckyBox : BaseUIData<LuckyBox>
    {
        public UIButton openButton;
        public UIRewardDetail reward;
        public GameObject closeNode;
        public GameObject openedNode;

        public LuckyBox LuckyBox { get; private set; }

        public Action<UILuckyBox> onBoxClicked;

        void Start()
        {
            if(openButton) openButton.OnClick.OnTrigger.Event.AddListener(() => onBoxClicked(this));
        }

        public override void SetData(LuckyBox luckyBox)
        {
            LuckyBox = luckyBox;
            bool opened = LuckyBox.Opened;
            if(openButton) openButton.Interactable = !opened;
            if(closeNode) closeNode.SetActive(!opened);
            if(openedNode) openedNode.SetActive(opened);
            if(opened && LuckyBox.Config) reward.SetData(LuckyBox.Config.reward);
        }

        public void UpdateBoxOpened()
        {
            SetData(LuckyBox);
        }
    }
}