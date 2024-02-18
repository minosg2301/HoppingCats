using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace moonNest
{
    public static class Leaderboard
    {
        #region variables
        static readonly Dictionary<int, LeaderboardData> topLeaderboards = new();
        static readonly Dictionary<int, LeaderboardData> centerLeaderboards = new();
        static readonly Dictionary<int, LeaderboardData> lastTopLeaderboards = new();
        static readonly Dictionary<int, LeaderboardData> lastCenterLeaderboards = new();
        static readonly Dictionary<int, LeaderboardData> byRangeLeaderboards = new();

        /// <summary>
        /// Row cached dùng để lưu loại các loại leaderboard center khác nhau theo row
        /// </summary>
        static readonly List<int> maxCenterRowCacheds = new();

        const int kMaxRow = 25;
        #endregion

        #region private methods
        static int ToId(string leaderboardId, LeaderboardTimespan timeSpan, int maxRow = 0)
        {
            return $"{leaderboardId}:{timeSpan}:{maxRow}".GetHashCode();
        }

        static int ToId(string leaderboardId, ulong minScore, ulong maxScore)
        {
            return $"{leaderboardId}:{minScore}:{maxScore}".GetHashCode();
        }

        static void RemoveAllCached(string leaderboardId)
        {
            ClearLearderboard(topLeaderboards, leaderboardId, LeaderboardTimespan.Monthly);
            ClearLearderboard(topLeaderboards, leaderboardId, LeaderboardTimespan.Weekly);
            ClearLearderboard(topLeaderboards, leaderboardId, LeaderboardTimespan.AllTime);
            ClearLearderboard(lastTopLeaderboards, leaderboardId, LeaderboardTimespan.Monthly);
            ClearLearderboard(lastTopLeaderboards, leaderboardId, LeaderboardTimespan.Weekly);
            ClearLearderboard(lastTopLeaderboards, leaderboardId, LeaderboardTimespan.AllTime);

            foreach (var maxRow in maxCenterRowCacheds)
            {
                ClearLearderboard(centerLeaderboards, leaderboardId, LeaderboardTimespan.Monthly, maxRow);
                ClearLearderboard(centerLeaderboards, leaderboardId, LeaderboardTimespan.Weekly, maxRow);
                ClearLearderboard(centerLeaderboards, leaderboardId, LeaderboardTimespan.AllTime, maxRow);
                ClearLearderboard(lastCenterLeaderboards, leaderboardId, LeaderboardTimespan.Monthly, maxRow);
                ClearLearderboard(lastCenterLeaderboards, leaderboardId, LeaderboardTimespan.Weekly, maxRow);
                ClearLearderboard(lastCenterLeaderboards, leaderboardId, LeaderboardTimespan.AllTime, maxRow);
            }

            byRangeLeaderboards.Clear();
        }

        static void ClearLearderboard(
            Dictionary<int, LeaderboardData> leaderboards,
            string leaderboardId,
            LeaderboardTimespan timespan,
            int maxRow = 0)
        {
            if (leaderboards.TryGetValue(ToId(leaderboardId, timespan, maxRow), out var leaderboard))
                leaderboard.data = null;
        }

        static void UpdateMetadataLocally(Dictionary<int, LeaderboardData> leaderboardDatas, string metadata)
        {
            foreach (var p in leaderboardDatas)
            {
                if (p.Value.Exist)
                {
                    p.Value.PlayerScore?.UpdateMetadata(metadata);
                }
            }
        }

        static void UpdateNameLocally(Dictionary<int, LeaderboardData> leaderboardDatas)
        {
            string userName = UserData.UserName;
            foreach (var p in leaderboardDatas)
            {
                if (p.Value.Exist)
                {
                    p.Value.PlayerScore?.UpdateUserName(userName);
                }
            }
        }
        #endregion

        #region submit score & updating methods
        public static void SubmitScore(string leaderboardId, ulong score, bool isOverride = false)
        {
            RemoveAllCached(leaderboardId);
            LeaderboardRestAPI
                .ReportScoreAsync(UserData.UserId, leaderboardId, score, null, isOverride)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Submit score to single leaderboard, with metadata if needed 
        /// </summary>
        /// <param name="leaderboardId"></param>
        /// <param name="score"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static Task SubmitScoreAsync(string leaderboardId, ulong score,
            string metadata = null,
            bool isOverride = false)
        {
            RemoveAllCached(leaderboardId);

            if (!string.IsNullOrEmpty(metadata))
            {
                UpdateMetadataLocally(topLeaderboards, metadata);
                UpdateMetadataLocally(centerLeaderboards, metadata);
            }

            return LeaderboardRestAPI.ReportScoreAsync(UserData.UserId, leaderboardId, score, metadata, isOverride);
        }

        /// <summary>
        /// Submit score to many leaderboards, and metadata if needed
        /// </summary>
        /// <param name="leaderboardIds"></param>
        /// <param name="score"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static Task SubmitScoreAsync(string[] leaderboardIds, ulong score,
            string metadata = null,
            bool isOverride = false)
        {
            foreach (var leaderboardId in leaderboardIds)
            {
                RemoveAllCached(leaderboardId);
            }

            if (!string.IsNullOrEmpty(metadata))
            {
                UpdateMetadataLocally(topLeaderboards, metadata);
                UpdateMetadataLocally(centerLeaderboards, metadata);
            }

            return LeaderboardRestAPI.ReportScoreAsync(UserData.UserId, leaderboardIds, score, metadata, isOverride);
        }

        /// <summary>
        /// Update user name and metadata to single leaderboard
        /// </summary>
        /// <param name="leaderboardId"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static Task UpdateUserData(string leaderboardId, string metadata)
        {
            if (string.IsNullOrEmpty(metadata)) return Task.CompletedTask;

            UpdateMetadataLocally(topLeaderboards, metadata);
            UpdateMetadataLocally(centerLeaderboards, metadata);
            UpdateNameLocally(topLeaderboards);
            UpdateNameLocally(centerLeaderboards);
            return LeaderboardRestAPI.UpdateDataAsync(UserData.UserId, leaderboardId, UserData.UserName, metadata);
        }

        /// <summary>
        /// Update user name and metadata to many leaderboards
        /// </summary>
        /// <param name="leaderboardIds"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static Task UpdateUserData(string[] leaderboardIds, string metadata)
        {
            if (string.IsNullOrEmpty(metadata)) return Task.CompletedTask;

            UpdateMetadataLocally(topLeaderboards, metadata);
            UpdateMetadataLocally(centerLeaderboards, metadata);
            UpdateNameLocally(topLeaderboards);
            UpdateNameLocally(centerLeaderboards);
            return LeaderboardRestAPI.UpdateDataAsync(UserData.UserId, leaderboardIds, UserData.UserName, metadata);
        }

        /// <summary>
        /// Update metadata to single leaderboard
        /// </summary>
        /// <param name="leaderboardId"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static Task UpdateMetaData(string leaderboardId, string metadata)
        {
            if (string.IsNullOrEmpty(metadata)) return Task.CompletedTask;

            UpdateMetadataLocally(topLeaderboards, metadata);
            UpdateMetadataLocally(centerLeaderboards, metadata);
            return LeaderboardRestAPI.UpdateMetadataAsync(UserData.UserId, leaderboardId, metadata);
        }

        /// <summary>
        /// Update metadata only to many leaderboards
        /// </summary>
        /// <param name="leaderboardIds"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static Task UpdateMetaData(string[] leaderboardIds, string metadata)
        {
            if (string.IsNullOrEmpty(metadata)) return Task.CompletedTask;

            UpdateMetadataLocally(topLeaderboards, metadata);
            UpdateMetadataLocally(centerLeaderboards, metadata);
            return LeaderboardRestAPI.UpdateMetadataAsync(UserData.UserId, leaderboardIds, metadata);
        }

        /// <summary>
        /// Update metadata to single leaderboard
        /// </summary>
        /// <param name="leaderboardId"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static Task UpdateMetaData<T>(string leaderboardId, T metadata)
        {
            if (metadata == null) throw new NullReferenceException("Metadata object must be not null");

            var metadataStr = JsonUtility.ToJson(metadata);
            if (string.IsNullOrEmpty(metadataStr)) return Task.CompletedTask;

            UpdateMetadataLocally(topLeaderboards, metadataStr);
            UpdateMetadataLocally(centerLeaderboards, metadataStr);
            return LeaderboardRestAPI.UpdateMetadataAsync(UserData.UserId, leaderboardId, metadataStr);
        }

        /// <summary>
        /// Update metadata to many leaderboards
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="leaderboardIds"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static Task UpdateMetaData<T>(string[] leaderboardIds, T metadata)
        {
            if (metadata == null) throw new NullReferenceException("Metadata object must be not null");

            var metadataStr = JsonUtility.ToJson(metadata);
            if (string.IsNullOrEmpty(metadataStr)) return Task.CompletedTask;

            UpdateMetadataLocally(topLeaderboards, metadataStr);
            UpdateMetadataLocally(centerLeaderboards, metadataStr);
            return LeaderboardRestAPI.UpdateMetadataAsync(UserData.UserId, leaderboardIds, metadataStr);
        }

        /// <summary>
        /// Update user name to single leaderboard
        /// </summary>
        /// <param name="leaderboardId"></param>
        /// <returns></returns>
        public static Task UpdateName(string leaderboardId)
        {
            UpdateNameLocally(topLeaderboards);
            UpdateNameLocally(centerLeaderboards);
            return LeaderboardRestAPI.UpdateNameAsync(UserData.UserId, leaderboardId, UserData.UserName);
        }

        /// <summary>
        /// Update user name to many leaderboards
        /// </summary>
        /// <param name="leaderboardIds"></param>
        /// <returns></returns>
        public static Task UpdateName(string[] leaderboardIds)
        {
            UpdateNameLocally(topLeaderboards);
            UpdateNameLocally(centerLeaderboards);
            return LeaderboardRestAPI.UpdateNameAsync(UserData.UserId, leaderboardIds, UserData.UserName);
        }
        #endregion

        #region load score no cached
        /// <summary>
        /// Get Scores List with specific from rank and maxRow
        /// NO CACHED on client side
        /// </summary>
        /// <param name="leaderboardId"></param>
        /// <param name="timeSpan"></param>
        /// <param name="fromIndex"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static async Task<List<LeaderboardScore>> LoadScores(
            string leaderboardId,
            LeaderboardTimespan timeSpan,
            int fromIndex,
            int limit = 20)
        {
            var dataRet = await LeaderboardRestAPI.LoadScores(leaderboardId, (int)timeSpan, fromIndex, limit);
            return dataRet.code == 0 ? dataRet.scores : new List<LeaderboardScore>();
        }

        #endregion

        #region center leaderboard
        public static async Task<List<LeaderboardScore>> LoadCenterScores(
            string leaderboardId,
            LeaderboardTimespan timeSpan,
            int maxRow)
        {
            var leaderboardData = await _LoadLeaderboardCenter(leaderboardId, centerLeaderboards, timeSpan, LeaderboardTimeForm.Current, maxRow);
            return leaderboardData.Exist ? leaderboardData.data.scores : new List<LeaderboardScore>();
        }

        public static async Task<List<LeaderboardScore>> LoadLastCenterScores(
            string leaderboardId,
            LeaderboardTimespan timeSpan,
            int maxRow)
        {
            var leaderboardData = await _LoadLeaderboardCenter(leaderboardId, lastCenterLeaderboards, timeSpan, LeaderboardTimeForm.Last, maxRow);
            return leaderboardData.Exist ? leaderboardData.data.scores : new List<LeaderboardScore>();
        }

        static async Task<LeaderboardData> _LoadLeaderboardCenter(
            string leaderboardId,
            Dictionary<int, LeaderboardData> _leaderboards,
            LeaderboardTimespan timeSpan,
            LeaderboardTimeForm timeForm,
            int maxRow)
        {
            int _maxRow = Mathf.Min(maxRow, 21);
            int _id = ToId(leaderboardId, timeSpan, _maxRow);

            if (!_leaderboards.TryGetValue(_id, out var leaderboard))
            {
                leaderboard = new LeaderboardData();
                _leaderboards[_id] = leaderboard;
            }

            if (leaderboard.Exist) return leaderboard;

            if (!leaderboard.requesting)
            {
                leaderboard.requesting = true;
                var leaderboardDataRet = await LeaderboardRestAPI.LoadCenterScores(UserData.UserId, leaderboardId, (int)timeSpan, (int)timeForm, _maxRow);
                if (leaderboardDataRet.code == 0 && leaderboardDataRet.scores.Count > 0)
                {
                    leaderboardDataRet.playerScore = leaderboardDataRet.scores.Find(s => s.UserId == UserData.UserId);
                    leaderboard.data = leaderboardDataRet;
                    leaderboard.lastRank = leaderboardDataRet.playerScore.Rank;

                    if (!maxCenterRowCacheds.Contains(_maxRow)) maxCenterRowCacheds.Add(_maxRow);
                    leaderboard.requesting = false;
                    return leaderboard;
                }

                leaderboard.requesting = false;
            }
            else
            {
                await Task.Factory.StartNew(() =>
                {
                    while (leaderboard.requesting) { Debug.Log($"_ prevent stuck, dont remove"); }
                });
            }
            return leaderboard;
        }
        #endregion

        #region top leaderboard
        public static async Task<LeaderboardData> GetLeaderboardAsync(
            string leaderboardId,
            LeaderboardTimespan timeSpan)
        {
            var leaderboardData = await _LoadTopScores(leaderboardId, topLeaderboards, timeSpan, LeaderboardTimeForm.Current, kMaxRow);
            return leaderboardData;
        }

        public static async Task<List<LeaderboardScore>> LoadTopScores(
            string leaderboardId,
            LeaderboardTimespan timeSpan)
        {
            var leaderboardData = await _LoadTopScores(leaderboardId, topLeaderboards, timeSpan, LeaderboardTimeForm.Current, kMaxRow);
            return leaderboardData.Exist ? leaderboardData.data.scores : null;
        }

        public static async Task<List<LeaderboardScore>> LoadLastTopScores(
            string leaderboardId,
            LeaderboardTimespan timeSpan)
        {
            var leaderboardData = await _LoadTopScores(leaderboardId, lastTopLeaderboards, timeSpan, LeaderboardTimeForm.Last, kMaxRow);
            return leaderboardData.Exist ? leaderboardData.data.scores : null;
        }

        static async Task<LeaderboardData> _LoadTopScores(
            string _leaderboardId,
            Dictionary<int, LeaderboardData> _leaderboards,
            LeaderboardTimespan _timeSpan,
            LeaderboardTimeForm _timeForm,
            int maxRow)
        {
            int _id = ToId(_leaderboardId, _timeSpan);

            if (!_leaderboards.TryGetValue(_id, out var leaderboard))
            {
                leaderboard = new LeaderboardData();
                _leaderboards[_id] = leaderboard;
            }

            if (leaderboard.Exist) return leaderboard;

            if (!leaderboard.requesting)
            {
                leaderboard.requesting = true;
                var leaderboardDataRet = await LeaderboardRestAPI.LoadTopScores(UserData.UserId, _leaderboardId, (int)_timeSpan, (int)_timeForm, maxRow);
                if (leaderboardDataRet.code == 0)
                {
                    if (leaderboardDataRet.playerScore != null)
                    {
                        // nếu có playerScore trong danh sách trả về
                        // thì lẩy playerScore là ref trong danh sách score thay cho playerScore từ server
                        leaderboardDataRet.playerScore = leaderboardDataRet.scores.Find(s => s.UserId == UserData.UserId);
                    }

                    leaderboard.data = leaderboardDataRet;
                    leaderboard.prevPageToken = new ScorePageToken(_leaderboardId, _timeSpan, PageDirection.Backward) { MarkedRow = maxRow };
                    leaderboard.nextPageToken = new ScorePageToken(_leaderboardId, _timeSpan, PageDirection.Forward) { MarkedRow = maxRow };
                    leaderboard.requesting = false;
                    return leaderboard;
                }

                leaderboard.requesting = false;
            }
            else
            {
                await Task.Factory.StartNew(() =>
                {
                    while (leaderboard.requesting) { Debug.Log($"_ prevent stuck, dont remove"); }
                });
            }
            return leaderboard;
        }

        public static bool HasAllScores(string leaderboardId, LeaderboardTimespan timeSpan,
            LeaderboardTimeForm timeForm)
        {
            int id = ToId(leaderboardId, timeSpan);
            var leaderboards = timeForm == LeaderboardTimeForm.Current
                ? topLeaderboards
                : lastTopLeaderboards;
            if (leaderboards.TryGetValue(id, out var leaderboardData) && leaderboardData.Exist)
            {
                return leaderboardData.data.scores.Count >= leaderboardData.data.maxRange;
            }
            return false;
        }

        /// <summary>
        /// Load more score for leaderboard
        /// </summary>
        /// <param name="leaderboardId"></param>
        /// <param name="timeSpan"></param>
        /// <returns>Scores to be loaded more</returns>
        public static Task<List<LeaderboardScore>> LoadMoreScores(string leaderboardId,
            LeaderboardTimespan timeSpan)
        {
            return _LoadMoreScores(leaderboardId, topLeaderboards, timeSpan, LeaderboardTimeForm.Current, kMaxRow);
        }

        /// <summary>
        /// Load more score for lastest leaderboard
        /// </summary>
        /// <param name="leaderboardId"></param>
        /// <param name="timeSpan"></param>
        /// <returns>Scores to be loaded more</returns>
        public static Task<List<LeaderboardScore>> LoadMoreLastScores(string leaderboardId,
            LeaderboardTimespan timeSpan)
        {
            return _LoadMoreScores(leaderboardId, lastTopLeaderboards, timeSpan, LeaderboardTimeForm.Current, kMaxRow);
        }

        static async Task<List<LeaderboardScore>> _LoadMoreScores(
            string _leaderboardId,
            Dictionary<int, LeaderboardData> _topLeaderboardDatas,
            LeaderboardTimespan _timeSpan,
            LeaderboardTimeForm _timeForm,
            int maxRow)
        {
            int id = ToId(_leaderboardId, _timeSpan);

            if (!_topLeaderboardDatas.TryGetValue(id, out var leaderboardData)
                || !leaderboardData.Exist
                || leaderboardData.data.scores.Count >= leaderboardData.data.maxRange)
            {
                return new List<LeaderboardScore>();
            }

            var newLeaderboardDataRet = await LeaderboardRestAPI.LoadMoreScore(UserData.UserId, leaderboardData.nextPageToken, (int)_timeForm, maxRow);
            if (newLeaderboardDataRet.code == 0)
            {
                leaderboardData.data.scores.AddRange(newLeaderboardDataRet.scores);
                leaderboardData.data.maxRange = newLeaderboardDataRet.maxRange;
                leaderboardData.nextPageToken.MarkedRow += maxRow;

                if (newLeaderboardDataRet.playerScore != null && leaderboardData.PlayerScore == null)
                {
                    // nếu có playerScore trong danh sách trả về
                    // thì lẩy playerScore là ref trong danh sách score thay cho playerScore từ server
                    leaderboardData.data.playerScore = leaderboardData.data.scores.Find(s => s.UserId == UserData.UserId);
                }

                return newLeaderboardDataRet.scores;
            }
            else
            {
                // nếu lấy thêm record failed
                // cập nhật lại max range để chỉ có thể dùng scores hiện tại
                // => ko cho gọi LeaderboardRestAPI.LoadMoreScores nữa
                leaderboardData.data.maxRange = leaderboardData.data.scores.Count;
                return new List<LeaderboardScore>();
            }
        }
        #endregion

        #region leaderboard by range
        public static bool HasAllScoresByRange(string leaderboardId,
            ulong minScore,
            ulong maxScore)
        {
            int id = ToId(leaderboardId, minScore, maxScore);
            if (byRangeLeaderboards.TryGetValue(id, out var leaderboardData) && leaderboardData.Exist)
            {
                return leaderboardData.data.maxRange > 0
                    && leaderboardData.data.scores.Count >= leaderboardData.data.maxRange;
            }
            return false;
        }

        public static async Task<List<LeaderboardScore>> LoadScoresByRange(string leaderboardId,
            ulong minScore,
            ulong maxScore)
        {
            int _id = ToId(leaderboardId, minScore, maxScore);

            if (!byRangeLeaderboards.TryGetValue(_id, out var leaderboard))
            {
                leaderboard = new LeaderboardData();
                byRangeLeaderboards[_id] = leaderboard;
            }

            if (leaderboard.Exist) return leaderboard.data.scores;

            if (!leaderboard.requesting)
            {
                leaderboard.requesting = true;
                var leaderboardDataRet = await LeaderboardRestAPI.LoadScoresByRange(
                    UserData.UserId, leaderboardId, minScore, maxScore, 0, kMaxRow);
                if (leaderboardDataRet.code == 0)
                {
                    bool maxRecordLoad = leaderboardDataRet.scores.Count == kMaxRow;

                    leaderboard.data = leaderboardDataRet;
                    leaderboard.data.maxRange = maxRecordLoad ? -1 : leaderboard.data.scores.Count;
                    leaderboard.prevPageToken = new ScorePageToken(leaderboardId, LeaderboardTimespan.AllTime, PageDirection.Backward) { MarkedRow = kMaxRow };
                    leaderboard.nextPageToken = new ScorePageToken(leaderboardId, LeaderboardTimespan.AllTime, PageDirection.Forward) { MarkedRow = kMaxRow };

                    if (leaderboardDataRet.playerScore != null)
                    {
                        // nếu có playerScore trong danh sách trả về
                        // thì lẩy playerScore là ref trong danh sách score thay cho playerScore từ server
                        leaderboardDataRet.playerScore = leaderboardDataRet.scores.Find(s => s.UserId == UserData.UserId);
                    }

                    leaderboard.requesting = false;

                    return leaderboard.data.scores;
                }

                leaderboard.requesting = false;
            }
            else
            {
                await Task.Run(() =>
                {
                    while (leaderboard.requesting)
                    {
                        Debug.Log("Leaderboard LoadScoresByRange waiting thread");
                    }
                });
            }
            return leaderboard.data.scores;
        }

        public static async Task<List<LeaderboardScore>> LoadMoreScoresByRange(string leaderboardId,
            ulong minScore,
            ulong maxScore)
        {
            int id = ToId(leaderboardId, minScore, maxScore);

            if (!byRangeLeaderboards.TryGetValue(id, out var leaderboard) || !leaderboard.Exist)
                return new List<LeaderboardScore>();

            if (leaderboard.data.maxRange > 0
                && leaderboard.data.scores.Count >= leaderboard.data.maxRange)
                return new List<LeaderboardScore>();

            var leaderboardDataRet = await LeaderboardRestAPI.LoadScoresByRange(
                UserData.UserId, leaderboardId, minScore, maxScore,
                leaderboard.nextPageToken.MarkedRow, kMaxRow);
            if (leaderboardDataRet.code == 0)
            {
                bool maxRecordLoad = leaderboardDataRet.scores.Count == kMaxRow;
                leaderboard.data.scores.AddRange(leaderboardDataRet.scores);
                leaderboard.data.maxRange = maxRecordLoad ? -1 : leaderboard.data.scores.Count;
                leaderboard.nextPageToken.MarkedRow += kMaxRow;

                if (leaderboardDataRet.playerScore != null && leaderboard.PlayerScore == null)
                {
                    // nếu có playerScore trong danh sách trả về
                    // thì lẩy playerScore là ref trong danh sách score thay cho playerScore từ server
                    leaderboard.data.playerScore = leaderboard.data.scores.Find(s => s.UserId == UserData.UserId);
                }

                return leaderboardDataRet.scores;
            }
            else
            {
                // nếu lấy thêm record failed
                // cập nhật lại max range để chỉ có thể dùng scores hiện tại
                // => ko cho gọi LeaderboardRestAPI.LoadMoreScores nữa
                leaderboard.data.maxRange = leaderboard.data.scores.Count;
                return new List<LeaderboardScore>();
            }
        }
        #endregion

        #region my score
        public static bool HaveMyScore(string leaderboardId, LeaderboardTimespan timeSpan)
        {
            return _CheckHaveMyScore(centerLeaderboards, leaderboardId, timeSpan);
        }

        public static bool HaveMyLastScore(string leaderboardId, LeaderboardTimespan timeSpan)
        {
            return _CheckHaveMyScore(lastCenterLeaderboards, leaderboardId, timeSpan);
        }

        static bool _CheckHaveMyScore(
            Dictionary<int, LeaderboardData> _centerLeaderboardDatas,
            string leaderboardId,
            LeaderboardTimespan timeSpan)
        {
            if (maxCenterRowCacheds.Count == 0) return false;

            // lấy player nên chỉ cần mấy cached max row đầu tiên
            int id = ToId(leaderboardId, timeSpan, maxCenterRowCacheds[0]);
            return _centerLeaderboardDatas.TryGetValue(id, out var leaderboardData)
                && leaderboardData.Exist
                && leaderboardData.PlayerScore != null;
        }

        public static Task<LeaderboardScore> LoadMyScore(
            string leaderboardId,
            LeaderboardTimespan timeSpan)
        {
            return _LoadMyScoreAsync(centerLeaderboards, leaderboardId, timeSpan);
        }

        public static Task<LeaderboardScore> LoadMyLastScore(
            string leaderboardId,
            LeaderboardTimespan timeSpan)
        {
            return _LoadMyScoreAsync(lastCenterLeaderboards, leaderboardId, timeSpan);
        }

        static async Task<LeaderboardScore> _LoadMyScoreAsync(
            Dictionary<int, LeaderboardData> _centerLeaderboardDatas,
            string leaderboardId,
            LeaderboardTimespan timeSpan)
        {
            int id = ToId(leaderboardId, timeSpan, 5);
            if (_centerLeaderboardDatas.TryGetValue(id, out var leaderboardData)
                && leaderboardData.Exist
                && leaderboardData.PlayerScore != null)
            {
                return leaderboardData.PlayerScore;
            }
            else
            {
                await LoadCenterScores(leaderboardId, timeSpan, 5);
                return _centerLeaderboardDatas.TryGetValue(id, out leaderboardData) && leaderboardData.Exist
                    ? leaderboardData.PlayerScore
                    : null;
            }
        }

        public static int GetLastRank(string leaderboardId, LeaderboardTimespan timeSpan)
        {
            if (maxCenterRowCacheds.Count == 0) return -1;

            int id = ToId(leaderboardId, timeSpan, maxCenterRowCacheds[0]);
            return centerLeaderboards.TryGetValue(id, out var leaderboard)
                ? leaderboard.lastRank
                : -1;
        }
        #endregion
    }
}