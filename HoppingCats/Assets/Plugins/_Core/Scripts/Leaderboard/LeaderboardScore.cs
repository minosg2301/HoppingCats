using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace moonNest
{
    [Serializable]
    [Preserve]
    public class LeaderboardScore
    {
        [SerializeField] private string userId;
        [SerializeField] private string userName;
        [SerializeField] private ulong value = 0;
        [SerializeField] private int rank = -1;
        [SerializeField] private string metadata = "";

        public string UserId => userId;
        public string UserName => userName;
        public ulong Value => value;
        public int Rank => rank;
        public string Metadata => metadata;

        object metaObject;

        protected LeaderboardScore() { }

        public LeaderboardScore(string userId, string userName, ulong value, string metadata)
        {
            this.userId = userId;
            this.userName = userName;
            this.value = value;
            this.metadata = metadata;
        }

        public void UpdateRank(int rank) => this.rank = rank;

        public void UpdateUserName(string userName) => this.userName = userName;

        public void UpdateMetadata(string metadata) => this.metadata = metadata;

        public override string ToString() => string.Format("{0}:{1}:{2}", rank, userId, value);

        public T GetMetaObject<T>()
        {
            metaObject ??= JsonUtility.FromJson(metadata, typeof(T));
            return (T)metaObject;
        }
    }
}