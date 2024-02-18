using Com.TheFallenGames.OSA.Core;
using UnityEngine;
using Com.TheFallenGames.OSA.DataHelpers;
using System;

namespace moonNest
{
    public class UIRangeLeaderboardOSA : OSA<TopLeaderboardParams, LeaderboardViewHolder>
    {
        [SerializeField] protected string leaderboardId;
        //[SerializeField] protected LeaderboardTimespan timeSpan;
        //[SerializeField] protected LeaderboardTimeForm timeForm;


        bool loadingNewScores;
        ulong minScore = 0;
        ulong maxScore = 0;

        protected SimpleDataHelper<LeaderboardScore> data;

        public event Action OnScoresPreLoad = delegate { };
        public event Action OnScoresLoaded = delegate { };

        void OnValidate()
        {
            //gameObject.name = "UIRangeLeaderboardOSA";
        }

        protected override void Start()
        {
            data = new SimpleDataHelper<LeaderboardScore>(this);
            base.Start();
            ReloadScores();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (IsInitialized)
                ReloadScores();
        }

        public void SetScoreRange(ulong minScore, ulong maxScore, bool reloadIfPossible = true)
        {
            this.minScore = minScore;
            this.maxScore = maxScore;
            if (reloadIfPossible && IsInitialized)
                ReloadScores();
        }

        async void ReloadScores()
        {
            if (minScore == 0 && maxScore == 0)
                return;

            OnScoresPreLoad();
            var scores = await Leaderboard.LoadScoresByRange(leaderboardId, minScore, maxScore);
            OnScoresLoaded();
            data.ResetItems(scores);            
        }

        protected override LeaderboardViewHolder CreateViewsHolder(int itemIndex)
        {
            LeaderboardViewHolder instance = new LeaderboardViewHolder();
            instance.Init(Parameters.ItemPrefab, Content, itemIndex);
            return instance;
        }

        protected override async void UpdateViewsHolder(LeaderboardViewHolder newOrRecycled)
        {
            newOrRecycled.SetData(data[newOrRecycled.ItemIndex]);
            if (newOrRecycled.ItemIndex == data.Count - 1
                && !Leaderboard.HasAllScoresByRange(leaderboardId, minScore, maxScore)
                && !loadingNewScores)
            {
                loadingNewScores = true;
                var moreScores = await Leaderboard.LoadMoreScoresByRange(leaderboardId, minScore, maxScore);
                loadingNewScores = false;

                if (moreScores.Count > 0)
                {
                    data.InsertItemsAtEnd(moreScores);
                }
            }
        }
    }
}