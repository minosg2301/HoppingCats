using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public class UILeaderboardRecord : BaseUIData<LeaderboardScore>
    {
        public TextMeshProUGUI userIdText;
        [LabelOverride("Name Text Pro")]
        public TextMeshProUGUI nameText;
        [LabelOverride("Name Text")]
        public Text nameNormalText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI rankText;
        public GameObject friendNode;

        private RectTransform rectTransform;
        public RectTransform RectTransform { get { if (!rectTransform) rectTransform = GetComponent<RectTransform>(); return rectTransform; } }

        public LeaderboardScore LeaderboardScore { get; private set; }

        void OnValidate()
        {
            //gameObject.name = "UILeaderboardRecord";
        }

        public override void SetData(LeaderboardScore score)
        {
            LeaderboardScore = score;
            if (userIdText) userIdText.text = score.UserId;
            if (nameText) nameText.text = score.UserName;
            if (nameNormalText) nameNormalText.text = score.UserName;
            if (scoreText) scoreText.text = score.Value.ToString();
            if (rankText) rankText.text = score.Rank.ToString();
        }
    }
}