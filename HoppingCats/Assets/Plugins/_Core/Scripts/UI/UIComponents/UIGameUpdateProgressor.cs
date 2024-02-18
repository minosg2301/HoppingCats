using Doozy.Engine.Progress;
using UnityEngine;

public class UIGameUpdateProgressor : MonoBehaviour
{
    public Progressor progressor;

    void Start()
    {
        foreach (var target in progressor.ProgressTargets)
        {
            target.gameObject.SetActive(false);
        }
#if UNITY_ANDROID
        GameUpdator.OnUpdateProgress += GameUpdator_OnUpdateProgress;
#endif
    }

    void OnDestroy()
    {
#if UNITY_ANDROID
        GameUpdator.OnUpdateProgress -= GameUpdator_OnUpdateProgress;
#endif
    }

#if UNITY_ANDROID
    private void GameUpdator_OnUpdateProgress(Google.Play.AppUpdate.AppUpdateRequest appUpdateRequest)
    {
        bool downloading = appUpdateRequest.Status == Google.Play.AppUpdate.AppUpdateStatus.Downloading;
        foreach (var target in progressor.ProgressTargets)
        {
            target.gameObject.SetActive(downloading);
        }

        progressor.SetProgress(appUpdateRequest.DownloadProgress);
    }
#endif
}