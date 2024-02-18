using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    public class UILeaderboardRankUpOSA : OSA<RankUpLeaderboardParams, LeaderboardViewHolder>
    {
        public string leaderboardId;
        public LeaderboardTimespan timeSpan;

        public int HideIndex1 { get; set; } = -1;
        public int HideIndex2 { get; set; } = -1;

        List<LeaderboardScore> scores;

        private RectTransform _rectTransform;
        public RectTransform RectTransform { get { if (!_rectTransform) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }

        public event Action OnReady = delegate { };

        void OnValidate()
        {
            gameObject.name = "UILeaderboardRankUpOSA";
        }

        protected override void Start()
        {
            base.Start();
            OnReady();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (IsInitialized)
                OnReady();
        }

        public void SetScores(List<LeaderboardScore> scores, int maxCount)
        {
            this.scores = scores;
            ResetItems(maxCount);
        }

        protected override LeaderboardViewHolder CreateViewsHolder(int itemIndex)
        {
            LeaderboardViewHolder instance = new();
            instance.Init(Parameters.ItemPrefab, Content, itemIndex);
            return instance;
        }

        protected override void UpdateViewsHolder(LeaderboardViewHolder newOrRecycled)
        {
            if ((HideIndex1 != -1 && newOrRecycled.ItemIndex == HideIndex1)
                || (HideIndex2 != -1 && newOrRecycled.ItemIndex == HideIndex2))
            {
                newOrRecycled.SetEmpty();
            }
            else
            {
                newOrRecycled.SetData(scores[newOrRecycled.ItemIndex]);
            }
        }
    }

    [Serializable]
    public class RankUpLeaderboardParams : BaseParamsWithPrefab
    {
        public void SetDragEnable(bool enabled)
        {
            DragEnabled = enabled;
        }

        public void SetScrollEnable(bool enabled)
        {
            ScrollEnabled = enabled;
        }
    }
}