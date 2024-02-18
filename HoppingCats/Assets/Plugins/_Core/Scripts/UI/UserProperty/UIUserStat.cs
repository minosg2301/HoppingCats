using UnityEngine;
using TMPro;

namespace moonNest
{
    public class UIUserStat : MonoBehaviour, IObserver
    {
        public UserStatId stat = -1;
        public string prefix = "";
        public string suffix = "";
        public TextMeshProUGUI target;

        string _prefix, _suffix;

        void Reset()
        {
            if (!target) target = GetComponent<TextMeshProUGUI>();
        }

        void OnValidate()
        {
            StatDefinition statDef = UserPropertyAsset.Ins.properties.FindStat(stat);
            gameObject.name = "UIUserStat - " + (statDef ? statDef.name : "");
            if (target && !Application.isPlaying) target.text = prefix + "-" + suffix;
        }

        void OnEnable()
        {
            if (string.IsNullOrEmpty(_prefix)) _prefix = (prefix.Length > 0 ? prefix.ToLocalized() : "");
            if (string.IsNullOrEmpty(_suffix)) _suffix = (suffix.Length > 0 ? suffix.ToLocalized() : "");
            if (stat != -1) UserData.Ins.Subscribe(this, stat.ToString());
        }

        void OnDisable()
        {
            if (stat != -1) UserData.Ins.Unsubscribe(this);
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            if (target)
            {
                UpdateText(UserData.Stat(stat).ToString());
            }
        }

        public void UpdateText(string statValue)
        {
            var _prefix = (prefix.Length > 0 ? prefix.ToLocalized() : "");
            var _suffix = (suffix.Length > 0 ? suffix.ToLocalized() : "");
            target.text = $"{_prefix}{statValue}{_suffix}";
        }
    }
}