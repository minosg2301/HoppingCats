using I2.Loc;
using TMPro;
using UnityEngine;

namespace moonNest
{
    public class UIBattlePassProgress : MonoBehaviour, IObserver
    {
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI requireText;
        public UIProgress progress;

        private LocalizationParamsManager _requireLocParam;
        public LocalizationParamsManager RequireLocParam { get { if(_requireLocParam == null) _requireLocParam = requireText.GetComponent<LocalizationParamsManager>(); return _requireLocParam; } }

        private UserArena Arena => UserArena.Ins;

        void OnEnable()
        {
            UserData.Ins.Subscribe(this, ArenaAsset.Ins.requireStatId.ToString());
        }

        void OnDisable()
        {
            UserData.Ins?.Unsubscribe(this);
        }

        private void OnValidate()
        {
            gameObject.name = "UIBattlePassProgress";
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            int require = Arena.Require;
            int exp = Arena.EXP;
            if(levelText) levelText.text = Arena.Level.ToString();
            if(progress) progress.SetProgress(exp, require);
            if(requireText)
            {
                if(RequireLocParam) RequireLocParam.SetParameterValue("progress", Mathf.Max(0, require - exp).ToString());
                else requireText.text = string.Format(ArenaAsset.Ins.requireDescription, Mathf.Max(0, require - exp));
            }
        }
    }
}