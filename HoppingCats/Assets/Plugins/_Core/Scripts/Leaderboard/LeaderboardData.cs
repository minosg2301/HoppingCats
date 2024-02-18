using System.Collections.Generic;

namespace moonNest
{
    public class LeaderboardData
    {
        internal int lastRank;
        internal bool requesting;
        
        internal LeaderboardDataInternal data;
        internal ScorePageToken prevPageToken;
        internal ScorePageToken nextPageToken;

        public bool Exist => data != null;
        public List<LeaderboardScore> Scores => data.scores;
        public LeaderboardScore PlayerScore => data.playerScore;
        public int MaxRange => data.maxRange;
    }

    public class LeaderboardDataInternal
    {
        public int code;
        public List<LeaderboardScore> scores;
        public LeaderboardScore playerScore;
        public int maxRange = -1;
    }

    public class ScorePageToken
    {
        private string leaderboardId;
        private LeaderboardTimespan timespan;
        private PageDirection direction;

        public ScorePageToken(string leaderboardId, LeaderboardTimespan timespan, PageDirection direction)
        {
            this.leaderboardId = leaderboardId;
            this.timespan = timespan;
            this.direction = direction;
        }

        public string LeaderboardId => leaderboardId;
        public LeaderboardTimespan Timespan => timespan;
        public PageDirection Direction => direction;
        public int MarkedRow { get; set; } = 0;
    }

    public class GameIdResult
    {
        public int code;
        public string appId;
    }    
}