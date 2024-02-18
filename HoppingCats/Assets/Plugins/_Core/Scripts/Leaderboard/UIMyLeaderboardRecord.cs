using Google.Impl;
using UnityEngine;

namespace moonNest
{
    public class UIMyLeaderboardRecord : UILeaderboardRecord
    {
        [SerializeField] protected string leaderboardId;
        [SerializeField] protected LeaderboardTimespan timeSpan;
        [SerializeField] protected LeaderboardTimeForm timeForm;

        void OnValidate()
        {
            //gameObject.name = "UIMyLeaderboardRecord";
        }

        protected virtual void OnEnable()
        {
            ReloadScore();
        }

        public void SetTimeSpan(LeaderboardTimespan timeSpan)
        {
            this.timeSpan = timeSpan;
            ReloadScore();
        }

        public void SetTimeForm(LeaderboardTimeForm timeForm)
        {
            this.timeForm = timeForm;
            ReloadScore();
        }

        void ReloadScore()
        {
            if (timeForm == LeaderboardTimeForm.Current)
            {
                GetMyScore();
            }
            else
            {
                GetMyLastScore();
            }
        }

        async void GetMyScore()
        {
            gameObject.SetActive(Leaderboard.HaveMyScore(leaderboardId, timeSpan));
            var myScore = await Leaderboard.LoadMyScore(leaderboardId, timeSpan);
            gameObject.SetActive(myScore != null);
            if (myScore != null) SetData(myScore);
        }

        async void GetMyLastScore()
        {
            gameObject.SetActive(Leaderboard.HaveMyLastScore(leaderboardId, timeSpan));
            var myScore = await Leaderboard.LoadMyLastScore(leaderboardId, timeSpan);
            gameObject.SetActive(myScore != null);
            if (myScore != null) SetData(myScore);
        }
    }
}