using Com.TheFallenGames.OSA.Core;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.CustomParams;
using System;

namespace moonNest
{
    public class UITopLeaderboardOSA : OSA<TopLeaderboardParams, LeaderboardViewHolder>
    {
        [SerializeField] protected string leaderboardId;
        [SerializeField] protected LeaderboardTimespan timeSpan;
        [SerializeField] protected LeaderboardTimeForm timeForm;

        bool loadingNewScores;
        protected SimpleDataHelper<LeaderboardScore> data;

        public event Action OnScoresPreLoad = delegate { };
        public event Action OnScoresLoaded = delegate { };
        public event Action<LeaderboardViewHolder> OnViewUpdated = delegate { };

        void OnValidate()
        {
            gameObject.name = "UITopLeaderboardOSA";
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

        async void ReloadScores()
        {
            OnScoresPreLoad();
            var scores = timeForm == LeaderboardTimeForm.Current
                ? await Leaderboard.LoadTopScores(leaderboardId, timeSpan)
                : await Leaderboard.LoadLastTopScores(leaderboardId, timeSpan);
            OnScoresLoaded();
            data.ResetItems(scores);
        }

        public void SetTimeSpan(LeaderboardTimespan timeSpan)
        {
            this.timeSpan = timeSpan;
            if (gameObject.activeSelf)
            {
                ReloadScores();
            }
        }

        public void SetTimeForm(LeaderboardTimeForm timeForm)
        {
            this.timeForm = timeForm;
            if (gameObject.activeSelf)
            {
                ReloadScores();
            }
        }

        protected override LeaderboardViewHolder CreateViewsHolder(int itemIndex)
        {
            LeaderboardViewHolder instance = new LeaderboardViewHolder();
            instance.Init(Parameters.ItemPrefab, Content, itemIndex);
            return instance;
        }

        protected async override void UpdateViewsHolder(LeaderboardViewHolder newOrRecycled)
        {
            newOrRecycled.SetData(data[newOrRecycled.ItemIndex]);
            OnViewUpdated(newOrRecycled);

            if (newOrRecycled.ItemIndex == data.Count - 1
                && !Leaderboard.HasAllScores(leaderboardId, timeSpan, timeForm)
                && !loadingNewScores)
            {
                loadingNewScores = true;
                var moreScores = await LoadMoreScores();
                loadingNewScores = false;

                if (moreScores.Count > 0)
                {
                    data.InsertItemsAtEnd(moreScores);
                }
            }
        }

        async Task<List<LeaderboardScore>> LoadMoreScores()
        {
            if (timeForm == LeaderboardTimeForm.Current)
                return await Leaderboard.LoadMoreScores(leaderboardId, timeSpan);
            else
                return await Leaderboard.LoadMoreLastScores(leaderboardId, timeSpan);
        }
    }

    [Serializable]
    public class TopLeaderboardParams : BaseParamsWithPrefab { }
}