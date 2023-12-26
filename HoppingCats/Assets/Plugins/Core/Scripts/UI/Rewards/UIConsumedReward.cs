using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace moonNest
{
    public class UIConsumedReward : BaseUIData<ConsumedReward>
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI amountText;
        public Image icon;
        public GameObject prefabContainer;

        public GameObject RewardPrefab { get; private set; }
        public ConsumedReward RewardResult { get; private set; }

        void Reset()
        {
            gameObject.name = "UIConsumedReward";
        }

        public override void SetData(ConsumedReward rewardResult)
        {
            RewardResult = rewardResult;

            if(nameText) nameText.text = rewardResult.Name.ToString();
            if(amountText) amountText.text = "x" + rewardResult.Amount.ToShortString(6);
            if(prefabContainer) prefabContainer.RemoveAllChildren();
            if(icon) icon.sprite = rewardResult.Icon;
            if(prefabContainer && rewardResult.Prefab)
            {
                RewardPrefab = Instantiate(rewardResult.Prefab, prefabContainer.transform);
                RewardPrefab.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            }
        }
    }
}