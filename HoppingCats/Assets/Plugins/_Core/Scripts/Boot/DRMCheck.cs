using UnityEngine;
using System.Text;
using System;
using EasyMobile;

namespace moonNest
{
    public class DRMCheck
    {
        const string kServerParams = "/aa?!&;BASE64_STR";
        const string kParams = "me=APP_ID&you=CLIENT_PACKAGE";

        static string GetURL() => GlobalConfig.Ins.DRMServer + kServerParams;

#if !UNITY_EDITOR && UNITY_ANDROID
        public static async void DoCheck()
        {
            string @params = kParams.Replace("APP_ID", GlobalConfig.Ins.gameId).Replace("CLIENT_PACKAGE", Application.identifier);
            string url = GetURL().Replace("BASE64_STR", Convert.ToBase64String(Encoding.UTF8.GetBytes(@params)));

            int timeOut = RestAPI.timeOut;
            RestAPI.timeOut = 3;
            try
            {
                RestAPI.timeOut = 3;
                var result = await RestAPI.GET(url);
                if (!string.IsNullOrEmpty(result) && result != "0")
                {
                    // if force update, pause game and show native ui
                    Time.timeScale = 0;

                    // show native ui with force update button
                    NativeUI
                        .Alert("NEW UPDATE", "We have new wonderful version for you!", "Update Now")
                        .OnComplete += ret =>
                        {
                            ApplicationExt.OpenStore(result);
                            Application.Quit();
                        };
                }
            }
            catch (Exception) { }
            finally
            {
                RestAPI.timeOut = timeOut;
            }
        }
#else
        public static void DoCheck() { }
#endif
    }
}