using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public class UIBattlePassButton : MonoBehaviour, IObserver
    {
        public TextMeshProUGUI levelText;
        public UIProgress progress;
        public Image icon;
        public GameObject highlightNode;
        public AudioSource animSfx;
        public CollectStatHandler tokenCollect;

        private bool playingAnim = false;
        private int lastEXP = -1;
        private int lastLevel = -1;
        private int lastRequire = -1;

        private UserArena Arena => UserArena.Ins;

        void Start()
        {
            if(tokenCollect)
            {
                tokenCollect.OnStart += OnCollectStart;
                tokenCollect.OnProcess += OnCollectProccess;
            }
            Arena.Subscribe(this);
            playingAnim = false;
        }

        void OnDisable()
        {
            if(Arena != null)
            {
                lastEXP = Arena.EXP;
                lastLevel = Arena.Level;
                lastRequire = Arena.Require;
                playingAnim = false;
            }
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            if(lastLevel == -1 || !tokenCollect)
            {
                lastEXP = Arena.EXP;
                lastLevel = Arena.Level;
                lastRequire = Arena.Require;

                if(levelText) levelText.text = Arena.Level.ToString();
                if(progress) progress.SetProgress(Arena.EXP, Arena.Require);
                if(highlightNode) highlightNode.SetActive(UserArena.Ins.HaveRewardCanClaim);
            }
            else if(tokenCollect)
            {
                if(highlightNode) highlightNode.SetActive(UserArena.Ins.HaveRewardCanClaim);
                DisplayLastValue();
            }
        }

        void OnCollectStart()
        {
            DisplayLastValue();
        }

        void OnCollectProccess(int count, float percent)
        {
            if(count == 1) PlayAnim();
        }

        public void PlayAnim()
        {
            if(!playingAnim && gameObject.activeSelf)
            {
                playingAnim = true;
                int toEXP = lastLevel < Arena.Level ? lastRequire : Arena.EXP;
                DoTweenEXP(lastEXP, toEXP, lastRequire, lastLevel);
            }
        }

        public void DisplayLastValue()
        {
            if(levelText) levelText.text = lastLevel.ToString();
            if(progress) progress.SetProgress(lastEXP, lastRequire);
        }

        private void DoTweenEXP(int fromEXP, int toEXP, int require, int currentLevel)
        {
            animSfx.Play();

            DOTween
            .To(() => fromEXP, val =>
            {
                progress.SetProgress(val, require);
            }, toEXP, 1f)
            .OnComplete(() =>
            {
                if(currentLevel < Arena.Level)
                {
                    currentLevel += 1;
                    BattlePassLevel battlePassLevel = ArenaAsset.Ins.FindLevel(currentLevel);
                    if(levelText) levelText.text = Arena.Level.ToString();
                    toEXP = currentLevel < Arena.Level ? battlePassLevel.requireValue : Arena.EXP;
                    DoTweenEXP(0, toEXP, battlePassLevel.requireValue, currentLevel);
                }
            });
        }
    }
}