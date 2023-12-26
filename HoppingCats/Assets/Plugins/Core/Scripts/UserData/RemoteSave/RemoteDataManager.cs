using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace moonNest.remotedata
{
    public class RemoteDataManager
    {
        static readonly Dictionary<Type, BaseRemoteData> remoteDatas = new Dictionary<Type, BaseRemoteData>();
        static readonly Dictionary<Type, BaseRemoteDataFactory> factories = new Dictionary<Type, BaseRemoteDataFactory>();
        static readonly Dictionary<Type, List<BaseRemoteDataUpdater>> dataUpdaters = new Dictionary<Type, List<BaseRemoteDataUpdater>>();

        public static void RegisterUpdater<T>(BaseRemoteDataUpdater dataUpdater) where T : BaseRemoteData
        {
            Type type = typeof(T);
            List<BaseRemoteDataUpdater> list = dataUpdaters.GetOrCreate(type, t => new List<BaseRemoteDataUpdater>());
            if (!list.Contains(dataUpdater)) list.Add(dataUpdater);
        }

        public static List<BaseRemoteDataUpdater> GetUpdaters<T>() where T : BaseRemoteData
        {
            Type type = typeof(T);
            return dataUpdaters.GetOrCreate(type, t => new List<BaseRemoteDataUpdater>());
        }

        public static void SetFactory<T>(BaseRemoteDataFactory factory) where T : BaseRemoteData
        {
            Type type = typeof(T);
            if (factories.ContainsKey(type)) return;
            factories[type] = factory;
        }

        public static T Get<T>() where T : BaseRemoteData
        {
            Type type = typeof(T);
            if (!remoteDatas.TryGetValue(type, out var remoteData))
                throw new ArgumentException($"RemoteData with type {type} does not exist!");

            return remoteData as T;
        }

        public static async Task<T> GetOrCreate<T>(string key, string userId) where T : BaseRemoteData
        {
            Type type = typeof(T);
            if (remoteDatas.TryGetValue(type, out var remoteData))
            {
                return remoteData as T;
            }

            if (!factories.TryGetValue(type, out var factory))
            {
                throw new ArgumentException($"Can not find Factory with {typeof(T)}");
            }

            T t = await factory.Get<T>(key, userId);
            bool createNew = t == null;
            if (createNew) t = await factory.Create<T>(key, userId);
            remoteDatas[type] = t ?? throw new NullReferenceException($"Can not create instance of {typeof(T)}");

            if (createNew) GetUpdaters<T>().ForEach(u => u.OnCreated());
            else GetUpdaters<T>().ForEach(u => u.OnSync());

            t.key = key;
            t.id = userId;

            return t;
        }

        internal static void Update()
        {
            foreach (var data in remoteDatas.Values)
            {
                data.UploadData();
            }
        }

        public static void CleanUp()
        {
            remoteDatas.Clear();
            dataUpdaters.Clear();
        }

        public static async Task DeleteAll()
        {
            foreach (var kv in factories)
            {
                if (remoteDatas.TryGetValue(kv.Key, out var remoteData))
                {
                    await kv.Value.Delete(remoteData);
                }
            }
        }
    }
}
