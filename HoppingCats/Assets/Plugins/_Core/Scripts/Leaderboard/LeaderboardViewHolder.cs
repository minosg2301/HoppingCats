using Com.TheFallenGames.OSA.Core;
using UnityEngine;

namespace moonNest
{
    public class LeaderboardViewHolder : BaseItemViewsHolder
    {
        public UILeaderboardRecord UI { get; protected set; }

        CanvasGroup canvasGroup;

        public override void CollectViews()
        {
            base.CollectViews();
            UI = root.GetComponent<UILeaderboardRecord>();
            canvasGroup = root.GetComponent<CanvasGroup>();
        }

        public virtual void SetData(LeaderboardScore score)
        {
            if (canvasGroup) canvasGroup.alpha = 1;
            UI.SetData(score);
        }

        public void SetEmpty()
        {
            if (!canvasGroup)
            {
                Debug.LogError("CanvasGroup is missing. Add CanvasGroup to SetEmpty for UILeaderboardRecord");
            }
            else
            {
                canvasGroup.alpha = 0;
            }
        }
    }
}