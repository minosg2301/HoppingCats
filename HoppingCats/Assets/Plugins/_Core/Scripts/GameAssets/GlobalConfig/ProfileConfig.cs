using System;

namespace moonNest
{
    [Serializable]
    public class ProfileConfig : BaseData
    {
        public ProfileConfig(string name) : base(name)
        { }

        public bool usePhoton = false;

        /// <summary>
        /// Verbose log in core
        /// </summary>
        public bool verboseLog = false;

        /// <summary>
        /// Enable unity debug log
        /// </summary>
        public bool debugLog = false;

        /// <summary>
        /// Print debug log for rest API
        /// </summary>
        public bool logRestApi;

        /// <summary>
        /// Enables Cheat
        /// </summary>
        public bool cheatEnabled = false;

        /// <summary>
        /// Enabled cheat for every game launch is a new day login
        /// </summary>
        public bool cheatNewDay = false;

        /// <summary>
        /// Sync date from server to prevent local cheat
        /// </summary>
        public bool syncDate = false;

        /// <summary>
        /// Sync Arena season for all clients
        /// </summary>
        public bool syncArena = false;

        /// <summary>
        /// Enabled authenticate user using Firebase Auth
        /// </summary>
        public bool authenticateUser = false;

        /// <summary>
        /// Enabled store user data on remote
        /// </summary>
        public bool storeRemoteData = false;

        /// <summary>
        /// Fake remote user id, used for testing
        /// </summary>
        public string fakeUserId = "";

        /// <summary>
        /// Leaderboard service url
        /// </summary>
        public string leaderboardUrl = "http://localhost:8890";


        public LaunchEnvironment environment;
    }
}