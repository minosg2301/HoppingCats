using UnityEngine;

namespace moonNest
{
    public class UISwitcherGroup : MonoBehaviour
    {
        private UISwitcher[] switchers;
        private UISwitcher switcherOn;

        void Start()
        {
            switchers = GetComponentsInChildren<UISwitcher>(true);
            switcherOn = switchers.Find(switcher => switcher.On);
            switchers.ForEach(switcher =>
            {
                if(switcher != switcherOn) switcher.On = false;
                switcher.SelfOff = false;
                switcher.OnSwitched += (on) => OnSwitched(switcher);
            });
        }

        private void OnSwitched(UISwitcher switcher)
        {
            if(switcher.On)
            {
                if(switcherOn) switcherOn.On = false;
                switcherOn = switcher;
            }
        }
    }
}