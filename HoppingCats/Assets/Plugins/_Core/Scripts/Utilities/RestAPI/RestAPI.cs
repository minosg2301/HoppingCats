using System;
using FullSerializer;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using Xxtea;

namespace moonNest
{
    public class RestAPI
    {
        public static int timeOut = 0;
        public static void LogRequest(string method, string uri)
        {
            Debug.Log("RestAPI-" + method + ": " + uri);
        }

        public static void LogResponse(string method, string uri, string response)
        {
            Debug.Log("RestAPI-" + method + "-res: " + ShortifyURI(uri) + "\n" + response);
        }

        static string ShortifyURI(string uri)
        {
            int lastSplashIndex = uri.LastIndexOf('/');
            return uri.Substring(lastSplashIndex, uri.Length - lastSplashIndex);
        }

        public static async Task<string> GET(string requestUrl, params object[] queries)
        {
            using var webRequest = WebRequestBuilder.Get(requestUrl, queries);

            if (GlobalConfig.Ins.LogRestApi) LogRequest("GET", webRequest.uri.ToString());

            webRequest.timeout = timeOut;
            webRequest.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();
            await webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ProtocolError ||
                webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                throw new Exception("HTTP ERROR " + webRequest.error + " " + requestUrl);
            }
            var text = webRequest.downloadHandler.text;

            if (GlobalConfig.Ins.LogRestApi) LogResponse("GET", requestUrl, text);

            return text;
        }

        public static async Task<T> GET<T>(string requestUrl, params object[] queries)
        {
            using var webRequest = WebRequestBuilder.Get(requestUrl, queries);

            if (GlobalConfig.Ins.LogRestApi) LogRequest("GET", webRequest.uri.ToString());

            webRequest.timeout = timeOut;
            webRequest.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();
            await webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new Exception("HTTP ERROR " + webRequest.error + " " + requestUrl);
            }
            string text = webRequest.downloadHandler.text;

            if (GlobalConfig.Ins.LogRestApi) LogResponse("GET", requestUrl, text);

            return Deserialize<T>(text);
        }

        public static async Task<string> POST(string requestUrl, WWWForm form)
        {
            using var webRequest = WebRequestBuilder.Post(requestUrl, form);

            if (GlobalConfig.Ins.LogRestApi) LogRequest("POST", webRequest.uri.ToString());

            webRequest.timeout = timeOut;
            webRequest.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();
            await webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new Exception("HTTP ERROR " + webRequest.error + " " + requestUrl);
            }
            string text = webRequest.downloadHandler.text;

            if (GlobalConfig.Ins.LogRestApi) LogResponse("POST", requestUrl, text);

            return text;
        }

        public static async Task<T> POST<T>(string requestUrl, WWWForm form)
        {
            using var webRequest = WebRequestBuilder.Post(requestUrl, form);

            if (GlobalConfig.Ins.LogRestApi) LogRequest("POST", webRequest.uri.ToString());

            webRequest.timeout = timeOut;
            webRequest.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();
            await webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new Exception("HTTP ERROR " + webRequest.error + " " + requestUrl);
            }
            string text = webRequest.downloadHandler.text;

            if (GlobalConfig.Ins.LogRestApi) LogResponse("POST", requestUrl, text);

            return Deserialize<T>(text);
        }

        public static async Task<T> POST<T>(string requestUrl, params object[] data)
        {
            var form = WebRequestBuilder.BuilForm(data);
            using var webRequest = WebRequestBuilder.Post(requestUrl, form);

            if (GlobalConfig.Ins.LogRestApi) LogRequest("POST", webRequest.uri.ToString());

            webRequest.timeout = timeOut;
            webRequest.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();
            await webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new Exception("HTTP ERROR " + webRequest.error + " " + requestUrl);
            }
            string text = webRequest.downloadHandler.text;

            if (GlobalConfig.Ins.LogRestApi) LogResponse("POST", requestUrl, text);

            return Deserialize<T>(text);
        }

        static fsSerializer Serializer = new fsSerializer();
        public static string Serialize<T>(T t)
        {
            Serializer.TrySerialize(t, out var data);
            return data.ToString();
        }

        public static T Deserialize<T>(string json)
        {
            fsData fsData = fsJsonParser.Parse(json);
            T result = default;
            Serializer.TryDeserialize(fsData, ref result);
            return result;
        }
    }

    public class WebRequestBuilder
    {
        public static string xxteaKey;

        private static string ParseQueries(string url, params object[] queries)
        {
            string finalUrl = url;
            finalUrl += "?";
            for (int i = 0, l = queries.Length; i + 1 < l;)
            {
                finalUrl += queries[i] + "=" + queries[i + 1];

                i += 2;
                if (i + 1 < l)
                {
                    finalUrl += "&";
                }
            }

            return finalUrl;
        }

        public static UnityWebRequest Get(string url, params object[] queries)
        {
            string finalUrl = ParseQueries(url, queries);
            return UnityWebRequest.Get(finalUrl);
        }

        public static UnityWebRequest Post(string url, WWWForm form, params object[] queries)
        {
            string finalUrl = ParseQueries(url, queries);
            var wr = UnityWebRequest.Post(finalUrl, form);
            return wr;
        }

        public static WWWForm BuilForm(params object[] data)
        {
            WWWForm form = new WWWForm();
            for (int i = 0, l = data.Length; i + 1 < l;)
            {
                try
                {
                    form.AddField((string)data[i], TrySerialize(data[i + 1]));
                    i += 2;
                }
                catch (Exception)
                {
                    Debug.Log("Failed To Serialize " + data[i + 1]);
                    return new WWWForm();
                }
            }

            if (GlobalConfig.Ins.LogRestApi) RestAPI.LogRequest("BuilForm", $"{string.Join(" ", data)}");

            return form;
        }

        internal static WWWForm BuilForm(Dictionary<string, object> dict)
        {
            WWWForm form = new WWWForm();
            foreach (var pair in dict)
            {
                try { form.AddField(pair.Key, TrySerialize(pair.Value)); }
                catch (Exception)
                {
                    Debug.LogFormat("Failed To Serialize ({0}, {1}", pair.Key, pair.Value);
                    return new WWWForm();
                }
            }
            return form;
        }

        private static string TrySerialize(object o)
        {
            Type type = o.GetType();
            if (type == typeof(string))
            {
                return (string)o;
            }
            else if (type.IsNumericType())
            {
                return o.ToString();
            }
            else if (type.IsArray)
            {
                return JsonHelper.Serialize(o);
            }
            else
            {
                fsSerializer fsSerializer = new fsSerializer();
                fsSerializer.TrySerialize(o, out var fsData);
                return fsData.ToString();
            }
        }

        public static WWWForm BuilFormWithEncrypt(params object[] datas)
        {
            string encryptData = ToJsonString(datas);
            return BuilForm(
                "data", XXTEA.EncryptToBase64String(encryptData, xxteaKey),
                "enc", 1);
        }

        public static WWWForm BuilFormWithEncryptByKey(string xxteakey, params object[] datas)
        {
            string encryptData = ToJsonString(datas);
            return BuilForm(
                "data", XXTEA.EncryptToBase64String(encryptData, xxteakey),
                "enc", 1);
        }

        static string ToJsonString(params object[] datas)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            for (int i = 0, l = datas.Length; i + 1 < l;)
            {
                try
                {
                    dict[(string)datas[i]] = TrySerialize(datas[i + 1]);
                    i += 2;
                }
                catch (Exception)
                {
                    Debug.Log("Failed To Serialize " + datas[i + 1]);
                    return "{}";
                }
            }

            if (GlobalConfig.Ins.LogRestApi) RestAPI.LogRequest("BuilForm", $"{string.Join(" ", datas)}");

            return JsonHelper.Serialize(dict);
        }
    }

    // Based on https://www.owasp.org/index.php/Certificate_and_Public_Key_Pinning#.Net
    class AcceptAllCertificatesSignedWithASpecificPublicKey : CertificateHandler
    {
        // Encoded RSAPublicKey
        private static string PUB_KEY = "keyishere";

        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //Debug.Log("I'm here");
            //X509Certificate2 certificate = new X509Certificate2(certificateData);

            //string pk = certificate.GetPublicKeyString();
            //Debug.Log(pk.ToLower());
            //return pk.Equals(PUB_KEY);
            return true; // pk.Equals(PUB_KEY);
        }
    }
}