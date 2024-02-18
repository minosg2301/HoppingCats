#if UNITY_ANDROID
using Google.Play.AppUpdate;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Purchasing;

namespace moonNest
{
    public class GlobalConfig : SingletonScriptObject<GlobalConfig>
    {
        public ProfileConfig SelectedProfile { get; set; }

        public int selectedProfileId;

        /// <summary>
        /// Enables Cheat
        /// </summary>
        public bool CheatEnabled => SelectedProfile ? SelectedProfile.cheatEnabled : true;

        /// <summary>
        /// Enabled cheat for every game launch is a new day login
        /// </summary>
        public bool CheatNewDay => SelectedProfile ? SelectedProfile.cheatNewDay : true;

        /// <summary>
        /// Enables Ads
        /// </summary>
        public bool adsEnabled = true;

        /// <summary>
        /// Enable check auto update for Android
        /// </summary>
        public bool autoUpdate = false;
        public AppUpdateType updateType;
        public int stalenessDays = 1;
        public bool simulateAutoUpdate = false;

        /// <summary>
        /// Verbose log in core
        /// </summary>
        public bool VerboseLog => SelectedProfile ? SelectedProfile.verboseLog : true;

        /// <summary>
        /// Enable unity debug log
        /// </summary>
        public bool DebugLog => SelectedProfile ? SelectedProfile.debugLog : true;

        /// <summary>
        /// Screen sleep mode
        /// </summary>
        public int screenSleepMode = SleepTimeout.NeverSleep;

        /// <summary>
        /// Enables intercepting promotional purchases that come directly from the Apple App Store
        /// </summary>
        public bool appStorePromotionPurchase = false;

        /// <summary>
        /// The FakeStore supports: no-ui (always succeeding), basic ui (purchase pass/fail), and
        /// developer ui (initialization, purchase, failure code setting). These correspond to
        /// the FakeStoreUIMode Enum values passed into StandardPurchasingModule.useFakeStoreUIMode.
        /// </summary>
        public FakeStoreUIMode fakeStoreMode;

        /// <summary>
        /// Verify a purchase via remote server instead of locally
        /// </summary>
        public bool verifyPurchasingOnline = false;

        /// <summary>
        /// Enabled store user data on remote
        /// </summary>
        public bool AuthenticateUser => SelectedProfile ? SelectedProfile.authenticateUser : false;

        /// <summary>
        /// Enabled store user data on remote
        /// </summary>
        public bool StoreRemoteData => SelectedProfile
            ? SelectedProfile.storeRemoteData && SelectedProfile.authenticateUser
            : false;

        /// <summary>
        /// Fake remote user id, used for testing
        /// </summary>
        public string FakeUserId => SelectedProfile ? SelectedProfile.fakeUserId : "";

        /// <summary>
        /// Enable remote service in editor mode
        /// </summary>
        //public bool enableInEditor = false;

        /// <summary>
        /// Sync date from server to prevent local cheat
        /// </summary>
        public bool SyncDate => SelectedProfile ? SelectedProfile.syncDate : false;

        /// <summary>
        /// Sync Arena season for all clients
        /// </summary>
        public bool SyncArena => SelectedProfile ? SelectedProfile.syncArena : false;

#if KEEP_GAME_CODE // keep game code for old leaderboard -  remove in future
        /// <summary>
        /// Game Code
        /// </summary>
        public string gameCode = "gameCode";
#endif

        /// <summary>
        /// Used for services
        /// </summary>
        public string gameId = "";

        /// <summary>
        /// Leaderboard URL
        /// </summary>
        public string LeaderboardURL => SelectedProfile.leaderboardUrl;

        /// <summary>
        /// GameService URL
        /// </summary>
        public string GameServiceURL => SelectedProfile.leaderboardUrl;

        /// <summary>
        /// Check DRM hosting
        /// </summary>
        public string DRMServer = "http://vgamestd.com:30003";

        /// <summary>
        /// Tracking hosting
        /// </summary>
        public string trackingServer = "https://promote.vgamestd.com:8080";

        /// <summary>
        /// Print debug log for rest API
        /// </summary>
        public bool LogRestApi => SelectedProfile && SelectedProfile.logRestApi;

        /// <summary>
        /// iOS App Id
        /// </summary>
        public string iosAppId = "";
        public bool useGameCenter = false;
        public bool useSignInWithApple = false;

        /// <summary>
        /// Google App Licenses
        /// </summary>
        public string googlePlayLicense = "";

        /// <summary>
        /// Apply Resolution scaling on device
        /// </summary>
        public bool resolutionScaling = false;

        /// <summary>
        /// Apply scaling factor if device DPI is greater than this value.
        /// scalingFactor = min(targetDPI / Screen.dpi, 1);
        /// </summary>
        public int targetDPI = 360;

        /// <summary>
        ///
        /// </summary>
        public string googleWebClientId = "WEB_CLIENTID";

        public string GenPath
        {
            get
            {
                var genPath = Application.dataPath + "/_Game/Scripts/_gen";
                if (!Directory.Exists(genPath))
                {
                    Directory.CreateDirectory(genPath);
                }

                return genPath;
            }
        }

        public LaunchEnvironment Environment => SelectedProfile.environment;

        public bool UsePhoton => SelectedProfile.usePhoton;

        /// <summary>
        /// List of suported languages
        /// </summary>
        public List<Language> languages = new List<Language>();
        public string FindLanguageText(string currentLanguage)
        {
            Language language = languages.Find(l => l.code == currentLanguage);
            return language != null ? language.text.ToLocalized() : "English";
        }

        public AudioMixerGroup mainMixer;
        public string musicParam = "music";
        public string sfxParam = "sfx";
        public string ingameSfxParam = "ingame-sfx";

    }

    [Serializable]
    public class Language
    {
        public string code;
        public string text;

        public Language() { }

        public Language(string code, string text)
        {
            this.code = code;
            this.text = text;
        }
    }

    public enum LaunchEnvironment
    {
        Local,
        Dev,
        Live,
        Staging,
        LocalNetwork
    }

#if !UNITY_ANDROID
    public enum AppUpdateType
    {
        Flexible = 0,
        Immediate = 1
    }
#endif
}