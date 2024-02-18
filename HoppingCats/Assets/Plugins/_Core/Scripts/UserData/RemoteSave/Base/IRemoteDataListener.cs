namespace moonNest.remotedata
{
    public class RemoteDataUpdater<T> : BaseRemoteDataUpdater where T : BaseRemoteData
    {
        public delegate void RemoteDataEvent(T remoteData);

        public RemoteDataEvent onRemoteDataCreated = delegate { };
        public RemoteDataEvent onRemoteDataSync = delegate { };

        public RemoteDataUpdater()
        {
            RemoteDataManager.RegisterUpdater<T>(this);
        }

        T _remoteData;
        public T RemoteData { get { if (_remoteData == null) _remoteData = RemoteDataManager.Get<T>(); return _remoteData; } }

        public override void OnCreated() => onRemoteDataCreated(RemoteData);

        public override void OnSync() => onRemoteDataSync(RemoteData);
    }
}