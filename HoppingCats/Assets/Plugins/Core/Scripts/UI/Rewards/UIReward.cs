using Doozy.Engine;
using Doozy.Engine.UI;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public class UIReward : BaseUIData<RewardObject>
    {
        public Image icon;
        public GameObject prefabContainer;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI amountText;
        public UIButton infoButton;
        public bool prefixX;
        public bool nameTextForStatOnly;

        GameObject uiPrefab;

        public RewardObject Reward { get; private set; }

        void Awake()
        {
            if(infoButton) infoButton.OnClick.OnTrigger.Event.AddListener(OnInfoClick);
        }

        void OnInfoClick()
        {
            Tooltip.ShowReward(this);
        }

        public override void SetData(RewardObject reward)
        {
            Reward = reward;

            if(prefabContainer && reward.Prefab)
            {
                if(icon) icon.gameObject.SetActive(false);
                if(uiPrefab) uiPrefab.SetActive(false);
                prefabContainer.SetActive(true);

                uiPrefab = Instantiate(reward.Prefab);
                uiPrefab.transform.SetParent(prefabContainer.transform, false);
                uiPrefab.transform.localPosition = Vector3.zero;
                uiPrefab.transform.localScale = Vector3.one;
            }
            else if(icon && reward.Icon)
            {
                if(prefabContainer) prefabContainer.SetActive(false);
                icon.gameObject.SetActive(true);
                icon.sprite = reward.Icon;
            }

            if(nameText)
            {
                if(!nameTextForStatOnly || (nameTextForStatOnly && reward.type == RewardType.Stat))
                {
                    nameText.gameObject.SetActive(true);
                    nameText.text = reward.DisplayName.ToString();
                }
                else
                {
                    nameText.gameObject.SetActive(false);
                }
            }

            if(amountText)
            {
                amountText.text = prefixX ? "x" + reward.AmountString : reward.AmountString;
            }

            // Update info
            if(infoButton)
            {
                switch(reward.type)
                {
                    case RewardType.Currency: infoButton.Interactable = Tooltip.requestCurrencyTooltip != null; break;
                    case RewardType.Chest: infoButton.Interactable = Tooltip.requestChestTooltip != null; break;
                    case RewardType.Item: infoButton.Interactable = Tooltip.requestItemTooltip != null; break;
                    case RewardType.Stat: infoButton.Interactable = Tooltip.requestStatTooltip != null; break;
                }
            }
        }

        public virtual void SetData(CurrencyReward currencyReward)
        {
            Reward = new RewardObject(currencyReward);
            if(icon) icon.sprite = currencyReward.Icon;
            if(nameText) nameText.text = currencyReward.DisplayName;
            if(amountText) amountText.text = currencyReward.AmountString;
        }

        public virtual void SetData(ItemReward itemReward)
        {
            Reward = new RewardObject(itemReward);
            if(icon) icon.sprite = itemReward.Icon;
            if(nameText) nameText.GetComponent<Localize>().Term = itemReward.DisplayName;
            if(amountText)
            {
                string amountStr = prefixX ? "x" + itemReward.AmountString : itemReward.AmountString;
                if(itemReward.probability < 1f) amountStr = prefixX ? "x0 ~ " + amountStr : "0 ~ " + amountStr;
                amountText.text = amountStr;
            }
        }
    }
}