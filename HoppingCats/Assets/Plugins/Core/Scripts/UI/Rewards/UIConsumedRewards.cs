using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace moonNest
{
    public class UIConsumedRewards : MonoBehaviour
    {
        private UIConsumedReward[] _uiRewards;
        public UIConsumedReward[] UIRewards { get { if(_uiRewards == null) _uiRewards = GetComponentsInChildren<UIConsumedReward>(true); return _uiRewards; } }

        public ConsumedRewards RewardResults { get; private set; }

        readonly UIListContainer<ConsumedReward, UIConsumedReward> listContainer = new UIListContainer<ConsumedReward, UIConsumedReward>();

        void Reset()
        {
            gameObject.name = "UIConsumedRewards";
        }

        public void SetData(ConsumedRewards rewardResults)
        {
            RewardResults = rewardResults;
            listContainer.SetList(transform, RewardResults.list);
        }
    }
}