namespace moonNest.remotedata
{
    public abstract class BaseRemoteDataUpdater
    {
        public abstract void OnSync();
        public abstract void OnCreated();
    }
}