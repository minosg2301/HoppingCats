using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using moonNest;

public class FBIGBuildProcessor
{
    private static BuildPlayerOptions s_options;

//#if USE_FB_INSTANT_ADS
//    [InitializeOnLoadMethod]
//#endif
    private static void Initialize()
    {
        BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
    }

    private static async void BuildPlayerHandler(BuildPlayerOptions options)
    {
        s_options = options;
        var accessToken = PlayerPrefs.GetString("fb_access_token", "");
        if (string.IsNullOrEmpty(accessToken))
        {
            var appId = PlayerPrefs.GetString("fb_app_id", "");
            var appSecret = PlayerPrefs.GetString("fb_app_secret", "");
            if (appId.Length == 0 || appSecret.Length == 0)
            {
                var popup = ((FBIGPopup)EditorWindow.GetWindow(typeof(FBIGPopup), false, "Game Setting"));
                popup.Show();
                popup.OnClose += async () =>
                {
                    var appId = PlayerPrefs.GetString("fb_app_id", "");
                    var appSecret = PlayerPrefs.GetString("fb_app_secret", "");
                    if (appId.Length == 0 || appSecret.Length == 0)
                    {
                        EditorUtility.DisplayDialog("Error", "App ID or App Secret are empty!", "Close");
                    }
                    else
                    {
                        await GetAccessToken(appId, appSecret);
                    }
                };
            }
            else
            {
                await GetAccessToken(appId, appSecret);
            }
        }
        else
        {
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
        }
    }

    private static async Task GetAccessToken(string appId, string appSecret)
    {
        try
        {
            var ret = await RestAPI.GET<AccessTokenData>("https://graph.facebook.com/oauth/access_token",
                      "client_id", appId,
                      "client_secret", appSecret,
                      "grant_type", "client_credentials");

            PlayerPrefs.SetString("fb_access_token", ret.access_token);
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(s_options);
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Can not get Access Token with Error\n{e.Message}", "Close");
        }
    }

    [Serializable]
    class AccessTokenData
    {
        public string access_token;
        public string token_type;
    }
}
