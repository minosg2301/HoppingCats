using FullSerializer;

namespace moonNest
{
    public static class SerializeHelper
    {
        static readonly fsSerializer serializer = new fsSerializer();

        public static string Serialize<T>(T instance)
        {
            serializer.TrySerialize(instance, out var data);
            return data.ToString();
        }

        public static T Deserialize<T>(string json)
        {
            fsData fsData = fsJsonParser.Parse(json);
            T result = default;
            serializer.TryDeserialize(fsData, ref result);
            return result;
        }
    }
}
