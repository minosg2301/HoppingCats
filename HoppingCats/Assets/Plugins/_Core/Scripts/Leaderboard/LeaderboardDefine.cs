namespace moonNest
{
    /// <summary>Values specifying which leaderboard timespan to use.</summary>
    public enum LeaderboardTimespan
    {
        /// <summary>Monthly scores. Reset at beginner of month.</summary>
        Monthly = 1,

        /// <summary>Weekly scores.  The week resets at 11:59 PM PST on Sunday.</summary>
        Weekly = 2,

        /// <summary>All time scores.</summary>
        AllTime = 3,
    }

    /// <summary>
    /// Form of time
    /// </summary>
    public enum LeaderboardTimeForm
    {
        /// <summary>
        /// Current one of Leaderboard per TimeSpan
        /// </summary>
        Current = 0,

        /// <summary>
        /// Last one of Leaderboard per TimeSpan
        /// </summary>
        Last = 1
    }

    public enum PageDirection
    {
        Forward = 1,
        Backward = 2,
    }
}