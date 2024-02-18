using System;

namespace moonNest.remotedata
{
    public abstract class RemotableUserData<T> : BaseUserData where T : BaseRemoteData
    {
        [NonSerialized] protected RemoteDataUpdater<T> handler;

        public T RemoteData => handler?.RemoteData;

        public void CreateRemoteUpdater()
        {
            if (!saveLocally) return;

            handler = new RemoteDataUpdater<T>()
            {
                onRemoteDataCreated = OnRemoteDataCreated,
                onRemoteDataSync = OnRemoteDataSync
            };
        }

        protected abstract void OnRemoteDataSync(T remoteData);
        protected abstract void OnRemoteDataCreated(T remoteData);
    }
}
