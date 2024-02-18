using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    public class UISwitcher : MonoBehaviour
    {
        [SerializeField] protected bool on;
        [SerializeField] protected List<UISwitchTarget> targets = new List<UISwitchTarget>();

        public event Action<bool> OnSwitched = delegate { };

        internal bool SelfOff { get; set; } = true;

#if UNITY_EDITOR
        public void UpdateTargets()
        {
            if (targets.Count == 0) targets.AddRange(GetComponentsInChildren<UISwitchTarget>());
        }
#endif

        void Start()
        {
            targets.ForEach(target => target.On = on);
        }

        void OnValidate()
        {
            targets.ForEach(target => target.On = on);
        }

        public bool On
        {
            get { return on; }
            set
            {
                if (on != value)
                {
                    on = value;
                    targets.ForEach(target => target.On = value);
                    OnSwitched(on);
                }
            }
        }

        public void Toggle()
        {
            if (!SelfOff && On) return;

            On = !On;
        }
    }
}