namespace moonNest.remotedata
{
    public abstract class BaseRemoteData
    {
        public string key = "";
        public string id = "";

        public abstract void UploadData();
    }
}