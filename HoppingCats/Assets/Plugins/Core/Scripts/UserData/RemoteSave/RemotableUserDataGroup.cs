using System;

namespace moonNest.remotedata
{
    public abstract class RemotableUserDataGroup<T, Data, DataDetail, Group, GroupDetail>
        : BaseUserDataGroup<Data, DataDetail, Group, GroupDetail>
        where T : BaseRemoteData
        where Data : DataObject<DataDetail>
        where DataDetail : BaseData
        where Group : GroupObject<GroupDetail>
        where GroupDetail : BaseData
    {
        [NonSerialized] protected RemoteDataUpdater<T> handler;

        public T RemoteData => handler.RemoteData;

        public void CreateRemoteUpdater()
        {
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
