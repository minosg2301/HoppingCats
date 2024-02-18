using System.Collections.Generic;
using System.Linq;

namespace moonNest.remotedata
{
    public abstract class BaseFirestoreData : BaseRemoteData
    {
        readonly IDictionary<string, object> requests = new Dictionary<string, object>();

        public void AddRequests(IDictionary<string, object> values) => requests.Concat(values);
        public void AddRequests(string field, IDictionary<int, double> values) => values.ForEach(pair => AddRequest(field, pair.Key, pair.Value));
        public void AddRequests(string field, IDictionary<int, string> values) => values.ForEach(pair => AddRequest(field, pair.Key, pair.Value));

        public void AddRequest(string field, object value) => requests[field] = value;
        public void AddRequest(string field, string key, object value) => requests[field + "." + key] = value;
        public void AddRequest(string field, int key, object value) => requests[field + "." + key] = value;
    }
}