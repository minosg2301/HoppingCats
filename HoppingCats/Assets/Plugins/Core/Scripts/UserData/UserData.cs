using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using moonNest.remotedata;

namespace moonNest
{
    public class UserData : RemotableUserData<FirestoreUserData>
    {
        public static UserData Ins => LocalData.Get<UserData>();

        #region static
        public static bool Linked { get { return Ins.linked; } set { Ins.linked = value; Ins.dirty = true; } }
        public static bool MusicOn { get { return Ins.musicOn; } set { Ins.musicOn = value; Ins.dirty = true; } }
        public static bool SfxOn { get { return Ins.sfxOn; } set { Ins.sfxOn = value; Ins.dirty = true; } }
        public static bool HapticOn { get { return Ins.hapticOn; } set { Ins.hapticOn = value; Ins.dirty = true; } }
        public static bool FirstLoginDay => Ins.firstLoginDay;
        public static int OfflineDay => Ins.offDay;
        public static int LoginDay => Ins.loginDay;
        public static float SecondsForNextDay => (float)Ins.nextDay.LocalTime.Subtract(DateTime.Now).TotalSeconds;
        public static DateTime NextDay => Ins.nextDay.LocalTime;

        private static string FakeId => GlobalConfig.Ins.FakeUserId;
        public static string UserId
        {
#if UNITY_EDITOR
            get { return FakeId.Length > 0 ? FakeId : Attr(UserPropertyDefinition.kUserId).AsString; }
#else
            get { return Attr(UserPropertyDefinition.kUserId).AsString; }
#endif
            set { SetAttr(UserPropertyDefinition.kUserId, value); }
        }

        public static string UserName
        {
            get { return Attr(UserPropertyDefinition.kName).AsString; }
            set { SetAttr(UserPropertyDefinition.kName, value); }
        }

        public static string Language
        {
            get { return Attr(UserPropertyDefinition.kLanguage).AsString; }
            set { SetAttr(UserPropertyDefinition.kLanguage, value); }
        }
        public static string ProfileId
        {
            get { return Ins.profileId; }
            set { Ins.profileId = value; Ins.DirtyAndNotify(kProfile); }
        }

        public const string kProfile = "Profile";

        public Action<bool> onLogin = delegate { };
        public Action<StatData> onStatUpdated = delegate { };
        public Action<SafeStatData> onSafeStatUpdated = delegate { };
        public Action<AttributeData> onAttributeUpdated = delegate { };
        #endregion

        #region Serialize Field
        [SerializeField] private string profileId = "";
        [SerializeField] private bool musicOn;
        [SerializeField] private bool sfxOn;
        [SerializeField] private bool hapticOn;
        [SerializeField] private bool linked;
        [SerializeField] private SynchronizableTime nextDay = new SynchronizableTime(1);
        [SerializeField] private Dictionary<int, StatData> stats = new Dictionary<int, StatData>();
        [SerializeField] private Dictionary<int, SafeStatData> safeStats = new Dictionary<int, SafeStatData>();
        [SerializeField] private Dictionary<int, AttributeData> attrs = new Dictionary<int, AttributeData>();
        [SerializeField] private List<Feature> features = new List<Feature>();
        #endregion

        #region private variables
        private bool firstLoginDay = false;
        private int offDay = 0;
        private int loginDay = 0;
        private UserPropertyDefinition propertyDefs;
        private readonly Dictionary<string, StatData> nameStatMap = new Dictionary<string, StatData>();
        private readonly Dictionary<string, SafeStatData> nameSafeStatMap = new Dictionary<string, SafeStatData>();
        private readonly Dictionary<string, AttributeData> nameAttrMap = new Dictionary<string, AttributeData>();
        #endregion

        #region override method
        protected internal override void OnInit()
        {
            base.OnInit();
            musicOn = sfxOn = hapticOn = true;
        }

        protected internal override void OnLoad()
        {
            base.OnLoad();

            propertyDefs = UserPropertyAsset.Ins.properties;

            UpdateProperties();
            UpdateFeatures();
            InitNameMaps();

            if (GlobalConfig.Ins.StoreRemoteData)
            {
                onLogin += OnLogin;
                onStatUpdated += OnStatUpdated;
                onSafeStatUpdated += OnSafeStatUpdated;
                onAttributeUpdated += OnAttributeUpdated;
            }
        }
        #endregion

        #region private methods
        private void InitNameMaps()
        {
            UserPropertyDefinition userDataDefinition = UserPropertyAsset.Ins.properties;
            stats.ForEach(pair =>
            {
                StatDefinition statDefinition = userDataDefinition.FindStat(pair.Key);
                if (!statDefinition.safe)
                {
                    nameStatMap[statDefinition.name] = pair.Value;
                    nameStatMap[statDefinition.name].name = statDefinition.name;
                    nameStatMap[statDefinition.name].id = statDefinition.id;
                    nameStatMap[statDefinition.name].sync = statDefinition.sync;
                }
            });
            safeStats.ForEach(pair =>
            {
                StatDefinition statDefinition = userDataDefinition.FindStat(pair.Key);
                if (statDefinition.safe)
                {
                    nameSafeStatMap[statDefinition.name] = pair.Value;
                    nameSafeStatMap[statDefinition.name].name = statDefinition.name;
                    nameSafeStatMap[statDefinition.name].id = statDefinition.id;
                    nameSafeStatMap[statDefinition.name].sync = statDefinition.sync;
                }
            });
            attrs.ForEach(pair =>
            {
                AttributeDefinition attrDefinition = userDataDefinition.FindAttribute(pair.Key);
                nameAttrMap[attrDefinition.name] = pair.Value;
                nameAttrMap[attrDefinition.name].id = attrDefinition.id;
                nameAttrMap[attrDefinition.name].name = attrDefinition.name;
                nameAttrMap[attrDefinition.name].sync = attrDefinition.sync;
            });
        }

        private void UpdateProperties()
        {
            foreach (StatDefinition stat in propertyDefs.stats)
            {
                if (stat.safe)
                {
                    if (!safeStats.ContainsKey(stat.id))
                        safeStats[stat.id] = new SafeStatData(stat.initValue);
                }
                else if (!stats.ContainsKey(stat.id))
                {
                    stats[stat.id] = new StatData(stat.initValue);
                }
            }

            foreach (AttributeDefinition attrDef in propertyDefs.attributes)
            {
                if (attrDef.type == AttributeType.GameObject) continue;

                if (!attrs.ContainsKey(attrDef.id))
                {
                    attrs[attrDef.id] = new AttributeData(attrDef.initValue);
                }
            }
        }

        private void UpdateFeatures()
        {
            foreach (var feature in features.ToList())
            {
                if (feature.Config == null) features.Remove(feature);
            }

            foreach (var featureConfig in GameDefinitionAsset.Ins.features)
            {
                var feature = FindFeature(featureConfig.id);
                if (feature == null) features.Add(new Feature(featureConfig));
            }
        }
        #endregion

        #region Update login methods
        public async Task<bool> UpdateLogin()
        {
            DateTime nextLoginDay = await nextDay.GetTime(UserId);
            if (nextLoginDay <= DateTime.Now)
            {
                offDay = Mathf.Max(0, DateTime.Today.Subtract(nextLoginDay).Days);
                firstLoginDay = true;
                loginDay++;
                dirty = true;
                onLogin(true);
                await nextDay.UpdateTimeByDay(UserId, 1);
                return true;
            }
            else
            {
                onLogin(false);
                return false;
            }
        }
        #endregion

        #region stat
        public static StatValue Stat(int statId)
        {
            var ins = Ins;
            if (ins.stats.TryGetValue(statId, out var statData)) return statData.value;
            else if (ins.safeStats.TryGetValue(statId, out var safeStatData)) return safeStatData.value.Value;

            return new StatValue();
        }

        public static StatValue Stat(string statName)
        {
            var ins = Ins;
            if (ins.nameStatMap.TryGetValue(statName, out var statData)) return statData.value;
            else if (ins.nameSafeStatMap.TryGetValue(statName, out var safeStatData)) return safeStatData.value.Value;

            return new StatValue();
        }

        public static string GetStatName(int statId)
        {
            var ins = Ins;
            if (ins.stats.TryGetValue(statId, out var statData)) return statData.name;
            else if (ins.safeStats.TryGetValue(statId, out var safeStatData)) return safeStatData.name;

            return "N/A";
        }

        public static bool SetStat(int statId, StatValue value)
        {
            var ins = Ins;
            if (ins.stats.TryGetValue(statId, out var stat))
            {
                ins.SetValue(stat, value);
                return true;
            }
            else if (ins.safeStats.TryGetValue(statId, out var safeStat))
            {
                ins.SetValue(safeStat, value);
                return true;
            }

            return false;
        }

        public static bool AddStat(int statId, StatValue value)
        {
            var ins = Ins;
            if (ins.stats.TryGetValue(statId, out var stat))
            {
                ins.SetValue(stat, stat.value + value);
                return true;
            }
            else if (ins.safeStats.TryGetValue(statId, out var safeStat))
            {
                ins.SetValue(safeStat, safeStat.value.Value + value);
                return true;
            }

            return false;
        }

        public static bool SetStat(string statName, StatValue value)
        {
            var ins = Ins;
            if (ins.nameStatMap.TryGetValue(statName, out var stat))
            {
                ins.SetValue(stat, value);
                return true;
            }
            else if (ins.nameSafeStatMap.TryGetValue(statName, out var safeStat))
            {
                ins.SetValue(safeStat, value);
                return true;
            }

            return false;
        }

        public static bool AddStat(string statName, StatValue value)
        {
            var ins = Ins;
            if (ins.nameStatMap.TryGetValue(statName, out var stat))
            {
                ins.SetValue(stat, stat.value + value);
                return true;
            }
            else if (ins.nameSafeStatMap.TryGetValue(statName, out var safeStat))
            {
                ins.SetValue(safeStat, safeStat.value.Value + value);
                return true;
            }

            return false;
        }

        private void SetValue(StatData stat, StatValue value)
        {
            stat.value = value;
            Notify(stat.id.ToString());
            dirty = true;
            onStatUpdated(stat);
        }

        private void SetValue(SafeStatData safeStat, StatValue value)
        {
            safeStat.value.Value = value;
            Notify(safeStat.id.ToString());
            dirty = true;
            onSafeStatUpdated(safeStat);
        }
        #endregion

        #region attribute
        private void SetValue(AttributeData attribute, Sprite value)
        {
            attribute.SetValue(value);
            Notify(attribute.id.ToString());
            dirty = true;
            onAttributeUpdated(attribute);
        }

        private void SetValue(AttributeData attribute, string value)
        {
            attribute.SetValue(value);
            Notify(attribute.id.ToString());
            dirty = true;
            onAttributeUpdated(attribute);
        }

        public static AttributeValue Attr(int attrId)
        {
            if (Ins.attrs.TryGetValue(attrId, out var attributeData))
            {
                return attributeData;
            }
            else
            {
                AttributeDefinition attr = Ins.propertyDefs.FindAttribute(attrId);
                return attr && attr.type == AttributeType.GameObject ? attr.initValue : new AttributeValue();
            }
        }

        public static void SetAttr(int attrId, Sprite value)
        {
            UserData ins = Ins;
            if (ins.attrs.TryGetValue(attrId, out var attribute))
                ins.SetValue(attribute, value);
        }

        public static void SetAttr(int attrId, string value)
        {
            UserData ins = Ins;
            if (ins.attrs.TryGetValue(attrId, out var attribute))
                ins.SetValue(attribute, value);
        }

        public static AttributeValue Attr(string statName)
        {
            if (Ins.nameAttrMap.TryGetValue(statName, out var attributeData))
            {
                return attributeData;
            }
            else
            {
                AttributeDefinition attr = Ins.propertyDefs.FindAttribute(statName);
                return attr && attr.type == AttributeType.GameObject ? attr.initValue : new AttributeValue();
            }
        }

        public static void SetAttr(string statName, Sprite value)
        {
            UserData ins = Ins;
            if (ins.nameAttrMap.TryGetValue(statName, out var attribute))
                ins.SetValue(attribute, value);
        }

        public static void SetAttr(string statName, string value)
        {
            UserData ins = Ins;
            if (ins.nameAttrMap.TryGetValue(statName, out var attribute))
                ins.SetValue(attribute, value);
        }
        #endregion

        #region feature
        public IReadOnlyList<Feature> Features => features.AsReadOnly();

        public Feature FindFeature(int featureId)
        {
            return features.Find(feature => feature.Id == featureId);
        }
        #endregion

        #region remote override methods
        void OnLogin(bool newDay)
        {
            if (newDay)
            {
                RemoteData.LoginDay = loginDay;
                RemoteData.AddRequest("LoginDay", loginDay);
            }
        }

        void OnStatUpdated(StatData stat)
        {
            if (!stat.sync) return;
            if (RemoteData == null) return;
            RemoteData.AddRequest("Stats", stat.id, stat.value.AsDouble);
        }

        void OnSafeStatUpdated(SafeStatData safeStat)
        {
            if (!safeStat.sync) return;
            if (RemoteData == null) return;
            RemoteData.AddRequest("Stats", safeStat.id, (double)safeStat.value.Value);
        }

        void OnAttributeUpdated(AttributeData attribute)
        {
            if (!attribute.sync || attribute.Type != AttributeType.String) return;
            if (RemoteData == null) return;
            RemoteData.AddRequest("Attributes", attribute.id, (string)attribute);
        }

        protected override void OnRemoteDataSync(FirestoreUserData remoteData)
        {
            if (RemoteData.Stats == null || RemoteData.Attributes == null)
            {
                OnRemoteDataCreated(remoteData);
                return;
            }

            // sync profile id
            ProfileId = RemoteData.ProfileId;
            loginDay = RemoteData.LoginDay;

            // sync stats
            Dictionary<int, double> remoteStats = RemoteData.Stats;
            foreach (var pair in remoteStats)
            {
                if (stats.TryGetValue(pair.Key, out var value) && value.sync)
                {
                    if (value.value.type == StatValueType.Int) value.value = (int)pair.Value;
                    else value.value = (float)pair.Value;
                }
                else if (safeStats.TryGetValue(pair.Key, out var safeValue) && safeValue.sync)
                {
                    safeValue.value.Value = (int)pair.Value;
                }
            }

            foreach (var pair in stats)
            {
                if (!remoteStats.ContainsKey(pair.Key) && pair.Value.sync)
                {
                    double value = pair.Value.value.AsDouble;
                    remoteStats[pair.Key] = value;
                    RemoteData.AddRequest("Stats", pair.Key, value);
                }
            }

            foreach (var pair in safeStats)
            {
                if (!remoteStats.ContainsKey(pair.Key) && pair.Value.sync)
                {
                    double value = pair.Value.value.Value;
                    remoteStats[pair.Key] = value;
                    RemoteData.AddRequest("Stats", pair.Key, value);
                }
            }

            // sync attributes
            Dictionary<int, string> remoteAttributes = RemoteData.Attributes;
            foreach (var pair in remoteAttributes)
            {
                if (attrs.TryGetValue(pair.Key, out var attr) && attr.Type == AttributeType.String && attr.sync)
                {
                    attr.SetValue(pair.Value);
                }
            }

            foreach (var pair in attrs)
            {
                if (!pair.Value.sync || pair.Value.Type != AttributeType.String) return;

                if (!remoteAttributes.ContainsKey(pair.Key))
                {
                    string content = pair.Value;
                    remoteAttributes[pair.Key] = content;
                    RemoteData.AddRequest("Attributes", pair.Key, content);
                }
            }
        }

        protected override void OnRemoteDataCreated(FirestoreUserData remoteData)
        {
            RemoteData.ProfileId = ProfileId = StringExt.RandomAlphabetWithNumber(6);
            RemoteData.AddRequest("ProfileId", ProfileId);

            RemoteData.Stats = stats.ToList().FindAll(pair => pair.Value.sync).ToMap(v => v.Key, v => v.Value.value.AsDouble);
            RemoteData.Stats.Concat(safeStats.ToList().FindAll(pair => pair.Value.sync).ToMap(v => v.Key, v => (double)v.Value.value.Value));
            RemoteData.AddRequests("Stats", RemoteData.Stats);

            RemoteData.Attributes = attrs.ToList().FindAll(pair => pair.Value.sync && pair.Value.Type == AttributeType.String).ToMap(v => v.Key, v => (string)v.Value);
            RemoteData.AddRequests("Attributes", RemoteData.Attributes);
        }
        #endregion
    }
}