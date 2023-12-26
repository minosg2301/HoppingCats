using I2.Loc;
using System;
using TMPro;
using UnityEngine;

namespace moonNest
{
    public class UIBattlePassSeason : MonoBehaviour, IObserver
    {
        public TextMeshProUGUI seasonText;
        public UICountDownTime countDownTime;

        private LocalizeParams _seasonParam;
        public LocalizeParams SeasonParam { get { if(!_seasonParam && seasonText) _seasonParam = seasonText.GetComponent<LocalizeParams>(); return _seasonParam; } }

        void Start()
        {
            UserArena.Ins.Subscribe(this);
        }

        void OnEnable()
        {
            UpdateUI();
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            UpdateUI();
        }

        void UpdateUI()
        {
            countDownTime.StartWithDuration((float)UserArena.Ins.LastSeconds);
            if(SeasonParam) SeasonParam.SetParameterValue("season", UserArena.Ins.Season.ToString());
            else if(seasonText) seasonText.SetText("Season " + UserArena.Ins.Season.ToString());
        }
    }
}