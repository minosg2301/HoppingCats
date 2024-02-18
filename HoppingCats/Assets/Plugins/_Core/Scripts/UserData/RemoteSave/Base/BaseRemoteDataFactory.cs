using System.Threading.Tasks;

namespace moonNest.remotedata
{
    public abstract class BaseRemoteDataFactory
    {
        public abstract Task<T> Create<T>(string userId, string key) where T : BaseRemoteData;
        public abstract Task<T> Get<T>(string userId, string key) where T : BaseRemoteData;
        public abstract Task Delete(BaseRemoteData remoteData);
    }
}