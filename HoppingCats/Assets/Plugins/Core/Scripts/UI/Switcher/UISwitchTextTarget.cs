using I2.Loc;
using TMPro;
using UnityEngine;

namespace moonNest
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UISwitchTextTarget : UISwitchTarget
    {
        public string on;
        public string off;

        private TextMeshProUGUI _target;
        public TextMeshProUGUI Target { get { if(!_target) _target = GetComponent<TextMeshProUGUI>(); return _target; } }

        private Localize _targetLoc;
        public Localize TargetLoc { get { if(!_targetLoc && Target) _targetLoc = Target.GetComponent<Localize>(); return _targetLoc; } }

        public override bool On
        {
            get { return base.On; }
            set
            {
                base.On = value;
                if(Target)
                {
                    if(TargetLoc) TargetLoc.Term = value ? on : off;
                    else Target.text = value ? on : off;
                }
            }
        }
    }
}