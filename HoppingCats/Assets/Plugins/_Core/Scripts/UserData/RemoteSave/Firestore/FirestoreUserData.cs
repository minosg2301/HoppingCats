using System.Collections.Generic;

namespace moonNest.remotedata
{
    public class FirestoreUserData : BaseFirestoreData
    {
        public string ProfileId { get; set; } = "";

        public int LoginDay { get; set; } = 0;

        public List<int> TutorialPlayeds { get; set; } = new List<int>();

        public Dictionary<int, double> Currencies { get; set; } = new Dictionary<int, double>();

        public Dictionary<int, double> Stats { get; set; } = new Dictionary<int, double>();

        public Dictionary<int, string> Attributes { get; set; } = new Dictionary<int, string>();

        public int TotalPurchase { get; set; }

        public string InviteLink { get; internal set; }

        public int InviteCount { get; internal set; }

        public List<int> InviteRewardIds { get; internal set; } = new List<int>();
        

        #region methods
        public FirestoreUserData() { }
        #endregion

        public override string ToString()
        {
            return $"{ProfileId}|{LoginDay}|{TotalPurchase}|{ToString(Currencies)}|{ToString(Stats)}|{ToString(Attributes)}";
        }

        string ToString(Dictionary<int, double> map)
        {
            string ret = "";
            foreach (var pair in map)
            {
                ret += $"[{pair.Key},{pair.Value}]";
            }
            return ret;
        }

        string ToString(Dictionary<int, string> map)
        {
            string ret = "";
            foreach (var pair in map)
            {
                ret += $"[{pair.Key},{pair.Value}]";
            }
            return ret;
        }

        public override void UploadData()
        {
            throw new System.NotImplementedException();
        }
    }
}