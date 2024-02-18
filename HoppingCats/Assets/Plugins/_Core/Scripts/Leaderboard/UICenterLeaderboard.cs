using System;
using UnityEngine;

namespace moonNest
{
    public class UICenterLeaderboard : UIBaseListContainer<LeaderboardScore, UILeaderboardRecord>
    {
        [SerializeField] protected string leaderboardId;
        [SerializeField] protected LeaderboardTimespan timeSpan;
        [SerializeField] protected LeaderboardTimeForm timeForm;
        [SerializeField] protected int maxRow = 5;

        public event Action OnScoresPreLoad = delegate { };
        public event Action OnScoresLoaded = delegate { };

        void OnValidate()
        {
            gameObject.name = "UICenterLeaderboard";
        }

        protected virtual void OnEnable()
        {
            Reload();
        }

        public void SetTimeSpan(LeaderboardTimespan timeSpan)
        {
            this.timeSpan = timeSpan;
            if (gameObject.activeSelf)
            {
                Reload();
            }
        }

        async void Reload()
        {
            OnScoresPreLoad();
            var scores = timeForm == LeaderboardTimeForm.Current
                ? await Leaderboard.LoadCenterScores(leaderboardId, timeSpan, maxRow)
                : await Leaderboard.LoadLastCenterScores(leaderboardId, timeSpan, maxRow);
            OnScoresLoaded();
            if (scores != null)
            {
                SetList(scores);
            }
        }
    }
}