using TMPro;
using UnityEngine;

namespace moonNest
{
    public class UIUnlockInfo : MonoBehaviour
    {
        public TextMeshProUGUI target;
        public string prefix;
        public string suffix;

        private UnlockConditionDetail unlockCondition;

        void Reset()
        {
            if(!target) target = GetComponent<TextMeshProUGUI>();
            gameObject.name = "UIUnlockInfo";
        }

        private void OnValidate()
        {
            if(target) target.text = prefix + "-" + suffix;
        }


        public void SetUnlockId(int unlockConditionId)
        {
            if(!unlockCondition || unlockCondition.id != unlockConditionId)
                unlockCondition = UnlockContentAsset.Ins.FindCondition(unlockConditionId);

            if(target)
                target.text = (prefix.Length > 0 ? prefix.ToLocalized() : "") + unlockCondition.name + (suffix.Length > 0 ? suffix.ToLocalized() : "");
        }
    }
}