using System;
using System.Threading.Tasks;
using UnityEngine;

namespace moonNest
{
    public class LeaderboardRestAPI
    {
        const string kUpdateData = "update";
        const string kUpdateMeta = "update-meta";
        const string kUpdateName = "update-name";

        const string kReportScoreV3 = "report-v3";
        const string kReportScoresV3 = "reports-v3";
        const string kUpdateDatas = "update-datas";
        const string kUpdateMetas = "update-metas";
        const string kUpdateNames = "update-names";
        const string kRegister = "register";

        const string kLoadCenterScores = "center";
        const string kLoadTopScores = "top";
        const string kLoadMoreScores = "more";
        const string kLoadScores = "scores";
        const string kLoadRangeScores = "range-scores";

        private static string url;
        private static readonly string xxteakey;

        public string leaderboardName;

        static LeaderboardRestAPI()
        {
            xxteakey = "DGDd@$8*N1+@R?v";
            url = GlobalConfig.Ins.LeaderboardURL + "/leaderboard/";
        }

        public LeaderboardRestAPI(string leaderboardName)
        {
            this.leaderboardName = leaderboardName;
        }

        public static void UpdateURL()
        {
            url = GlobalConfig.Ins.LeaderboardURL + "/leaderboard/";
        }

        static string GetRequestUrl(string action) => url + action;

        static string ToServerId(string leaderboardId)
        {
            return $"{leaderboardId}";
        }

        internal static async Task<LeaderboardDataInternal> LoadScoresByRange(string userId, string leaderboardId, ulong minScore, ulong maxScore, int fromRow, int maxRow)
        {
            try
            {
                return await RestAPI.GET<LeaderboardDataInternal>(GetRequestUrl(kLoadRangeScores),
                    "id", ToServerId(leaderboardId),
                    "minScore", minScore,
                    "maxScore", maxScore,
                    "fromRow", fromRow,
                    "maxRow", maxRow);
            }
            catch (Exception)
            {
                return new LeaderboardDataInternal() { code = -1 };
            }
        }

        public static async Task<LeaderboardDataInternal> LoadScores(string leaderboardId, int timeSpan, int fromRow, int maxRow)
        {
            try
            {
                return await RestAPI.GET<LeaderboardDataInternal>(GetRequestUrl(kLoadScores),
                    "id", ToServerId(leaderboardId),
                    "timeSpan", timeSpan,
                    "fromRow", fromRow,
                    "maxRow", maxRow);
            }
            catch (Exception)
            {
                return new LeaderboardDataInternal() { code = -1 };
            }
        }

        public static async Task<LeaderboardDataInternal> LoadTopScores(string userId, string leaderboardId, int timeSpan, int timeForm, int maxRow)
        {
            try
            {
                return await RestAPI.GET<LeaderboardDataInternal>(GetRequestUrl(kLoadTopScores),
                    "id", ToServerId(leaderboardId),
                    "userId", userId,
                    "timeSpan", timeSpan,
                    "timeForm", timeForm,
                    "maxRow", maxRow);
            }
            catch (Exception)
            {
                return new LeaderboardDataInternal() { code = -1 };
            }
        }

        public static async Task<LeaderboardDataInternal> LoadMoreScore(string userId, ScorePageToken nextPageToken, int timeForm, int maxRow)
        {
            try
            {
                return await RestAPI.GET<LeaderboardDataInternal>(GetRequestUrl(kLoadMoreScores),
                    "id", ToServerId(nextPageToken.LeaderboardId),
                    "userId", userId,
                    "timeSpan", (int)nextPageToken.Timespan,
                    "timeForm", timeForm,
                    "fromRow", nextPageToken.MarkedRow,
                    "maxRow", maxRow);
            }
            catch (Exception)
            {
                return new LeaderboardDataInternal() { code = -1 };
            }
        }

        public static async Task<LeaderboardDataInternal> LoadCenterScores(string userId, string leaderboardId, int timeSpan, int timeForm, int maxRow)
        {
            try
            {
                return await RestAPI.GET<LeaderboardDataInternal>(GetRequestUrl(kLoadCenterScores),
                    "id", ToServerId(leaderboardId),
                    "userId", userId,
                    "timeSpan", timeSpan,
                    "timeForm", timeForm,
                    "maxRow", maxRow);
            }
            catch (Exception)
            {
                return new LeaderboardDataInternal() { code = -1 };
            }
        }

        public static async Task ReportScoreAsync(string userId, string leaderboardId, ulong score, string metadata, bool isOverride = false)
        {
            try
            {
                WWWForm form = WebRequestBuilder.BuilFormWithEncryptByKey(xxteakey,
                    "id", ToServerId(leaderboardId),
                    "score", score,
                    "userId", userId,
                    "metadata", metadata ?? "",
                    "override", isOverride);
                await RestAPI.POST(GetRequestUrl(kReportScoreV3), form);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public static async Task ReportScoreAsync(string userId, string[] leaderboardIds, ulong score, string metadata, bool isOverride = false)
        {
            try
            {
                WWWForm form = WebRequestBuilder.BuilFormWithEncryptByKey(xxteakey,
                    "ids", leaderboardIds.Map(id => ToServerId(id)).ToArray(),
                    "score", score,
                    "userId", userId,
                    "metadata", metadata ?? "",
                    "override", isOverride);
                await RestAPI.POST(GetRequestUrl(kReportScoresV3), form);
            }
            catch (Exception)
            {
            }
        }

        public static async Task UpdateDataAsync(string userId, string leaderboardId, string userName, string metadata)
        {
            try
            {
                WWWForm form = WebRequestBuilder.BuilFormWithEncryptByKey(xxteakey,
                    "id", ToServerId(leaderboardId),
                    "userId", userId,
                    "data", RestAPI.Serialize(new LeaderboardScore(userId, userName, 0, metadata)));
                await RestAPI.POST(GetRequestUrl(kUpdateData), form);
            }
            catch (Exception)
            {
            }
        }

        public static async Task UpdateDataAsync(string userId, string[] leaderboardIds, string userName, string metadata)
        {
            try
            {
                WWWForm form = WebRequestBuilder.BuilFormWithEncryptByKey(xxteakey,
                    "ids", leaderboardIds.Map(id => ToServerId(id)).ToArray(),
                    "userId", userId,
                    "data", RestAPI.Serialize(new LeaderboardScore(userId, userName, 0, metadata)));
                await RestAPI.POST(GetRequestUrl(kUpdateDatas), form);
            }
            catch (Exception)
            {
            }
        }

        public static async Task UpdateMetadataAsync(string userId, string leaderboardId, string metadata)
        {
            try
            {
                WWWForm form = WebRequestBuilder.BuilFormWithEncryptByKey(xxteakey,
                    "id", ToServerId(leaderboardId),
                    "userId", userId,
                    "metadata", metadata);
                await RestAPI.POST(GetRequestUrl(kUpdateMeta), form);
            }
            catch (Exception)
            {
            }
        }

        public static async Task UpdateMetadataAsync(string userId, string[] leaderboardIds, string metadata)
        {
            try
            {
                WWWForm form = WebRequestBuilder.BuilFormWithEncryptByKey(xxteakey,
                    "ids", leaderboardIds.Map(id => ToServerId(id)).ToArray(),
                    "userId", userId,
                    "metadata", metadata);
                await RestAPI.POST(GetRequestUrl(kUpdateMetas), form);
            }
            catch (Exception)
            {
            }
        }

        public static async Task UpdateNameAsync(string userId, string leaderboardId, string userName)
        {
            try
            {
                WWWForm form = WebRequestBuilder.BuilFormWithEncryptByKey(xxteakey,
                    "id", ToServerId(leaderboardId),
                    "userId", userId,
                    "userName", userName);
                await RestAPI.POST(GetRequestUrl(kUpdateName), form);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public static async Task UpdateNameAsync(string userId, string[] leaderboardIds, string userName)
        {
            try
            {
                WWWForm form = WebRequestBuilder.BuilFormWithEncryptByKey(xxteakey,
                    "ids", leaderboardIds.Map(id => ToServerId(id)).ToArray(),
                    "userId", userId,
                    "userName", userName);
                await RestAPI.POST(GetRequestUrl(kUpdateNames), form);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public static async Task<GameIdResult> RegisterGameID(string gameId, string applicationId)
        {
            try
            {
                WWWForm form = WebRequestBuilder.BuilFormWithEncryptByKey(xxteakey,
                    "gameId", gameId,
                    "appId", applicationId);
                return await RestAPI.POST<GameIdResult>(GetRequestUrl(kRegister), form);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return null;
        }
    }
}