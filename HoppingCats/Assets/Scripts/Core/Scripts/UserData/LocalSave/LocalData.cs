using BayatGames.SaveGameFree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace vgame
{
    public class LocalData : SingletonMono<LocalData>
    {
#if UNITY_EDITOR
        const bool useEncodeFileName = false;
        const bool useEncode = false;
#else
        const bool useEncodeFileName = true;
        const bool useEncode = true;
#endif

        const string password = "vgamespass";

        public float FrequencySaveDataTime { get; set; } = 5.0f;//min
        public DateTime LastSaveDataTime { get; set; } = DateTime.UtcNow;
        public string UserMoney { get; internal set; }
        public int UserLevel { get; internal set; }

        readonly Dictionary<string, BaseUserData> userDatas = new Dictionary<string, BaseUserData>();

        public static bool allowSave = true;

        public static T Get<T>() where T : BaseUserData
        {
            return Ins?.Load<T>();
        }

        public static void Save(BaseUserData data)
        {
            Ins._Save(data);
        }

        public static void DeleteAll()
        {
            foreach (var pair in Ins.userDatas.ToList())
            {
                var data = pair.Value;
                if (data.preventDelete) continue;

                data.OnWillDeleted();
                
                Type type = data.GetType();
                string id = type.Name;
                SaveGame.Delete(EncodeString(id));

                Ins.userDatas.Remove(pair.Key);
            }
        }

        static string EncodeString(string id)
        {
            var encodeId = useEncodeFileName ? Convert.ToBase64String(Encoding.Default.GetBytes(id)) : id;
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return encodeId + "_win";
            }
            else
            {
                return encodeId;
            }
        }

        void _Save(BaseUserData data)
        {
            if (!data.saveLocally) return;

            Type type = data.GetType();
            string id = type.Name;
            SaveGame.Save(EncodeString(id), data, useEncode, password);
        }

        T Load<T>() where T : BaseUserData
        {
            Type type = typeof(T);
            string id = type.Name;
            string identifier = EncodeString(id);

            if (!userDatas.ContainsKey(id))
            {
                if (!SaveGame.Exists(identifier))
                {
                    CreateNew<T>(type, id);
                }
                else
                {
                    try
                    {
                        T obj = SaveGame.Load<T>(identifier, useEncode, password);
                        userDatas[id] = obj;
                        obj.OnLoad();
                    }
                    catch (Exception e)
                    {
                        // Failed to load
                        Debug.LogWarning("LocalData failed to " + type.Name + " data with Exeception: " + e.Message);

                        // create new data to continue playing
                        CreateNew<T>(type, id);

                    }
                }
            }

            return (T)userDatas[id];
        }

        void CreateNew<T>(Type type, string id) where T : BaseUserData
        {
            T obj = (T)Activator.CreateInstance(type);
            userDatas[id] = obj;
            obj.OnInit();
            obj.OnLoad();
            _Save(obj);
        }

        public void LateUpdate()
        {
            TimeSpan time = DateTime.UtcNow.Subtract(LastSaveDataTime);
            if (time.TotalMinutes > FrequencySaveDataTime)
            {
                SaveAll();
                LastSaveDataTime = DateTime.UtcNow;
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                userDatas.Values.ForEach(userData => userData.OnPause());
                SaveAll();
                LastSaveDataTime = DateTime.UtcNow;
            }
        }

        private void OnApplicationQuit()
        {
            userDatas.Values.ForEach(userData => userData.OnQuit());
            SaveAll(true);
        }

        private void SaveAll(bool forced = false)
        {
            if (!allowSave) return;

            userDatas.Values
            .Where(userData => (forced || userData.dirty) && userData.saveLocally)
            .ForEach(userData => _Save(userData));
        }
    }

#if UNITY_EDITOR
    public static class DataLocalTool
    {
        [MenuItem("EditorSettings/Clear Save Data", false, 99)]
        private static void ResetLocalData()
        {
            SaveGame.DeleteAll();
            PlayerPrefs.DeleteAll();
        }
    }
#endif
}