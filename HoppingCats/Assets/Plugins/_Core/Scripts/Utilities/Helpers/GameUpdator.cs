#if UNITY_ANDROID
using Google.Play.AppUpdate;
using System;
using System.Collections;
using UnityEngine;
using moonNest;

public class GameUpdator : SingletonMono<GameUpdator>
{
    public static event Action<AppUpdateRequest> OnUpdateStarted = delegate { };
    public static event Action<AppUpdateRequest> OnUpdateProgress = delegate { };
    public static event Action<AppUpdateRequest> OnUpdateCompleted = delegate { };
    public static event Action<AppUpdateRequest> OnAskForInstallUpdate = null;

    static AppUpdateManager s_appUpdateManager;

    public static void InstallUpdate()
    {
#if UNITY_EDITOR
        Debug.Log("[GameUpdator] Game will install update and restart on device.");
#else
        if (s_appUpdateManager != null)
        {
            s_appUpdateManager.CompleteUpdate();
            s_appUpdateManager = null;
        }
#endif
    }

    internal void CheckForUpdate()
    {
        if (!GlobalConfig.Ins.autoUpdate)
            return;

#if UNITY_EDITOR
        if (GlobalConfig.Ins.simulateAutoUpdate)
            StartCoroutine(SimulateAutoUpdate());
#else
        StartCoroutine(CheckForUpdate_Internal());
#endif
    }


#if UNITY_EDITOR
    IEnumerator SimulateAutoUpdate()
    {
        Debug.Log("[GameUpdator] Preparing for simulate auto update...");
        yield return new WaitForSeconds(3f);

        float progress = 0f;
        var updateRequest = new SimulateAppRequest();

        Debug.Log("[GameUpdator] Start download update");
        updateRequest.UpdateStatus(AppUpdateStatus.Pending);
        OnUpdateStarted(updateRequest);

        updateRequest.UpdateStatus(AppUpdateStatus.Downloading);
        while (!updateRequest.IsDone)
        {
            Debug.Log($"[GameUpdator] Downloading...{progress:0%}");
            updateRequest.UpdateProgress(progress);
            OnUpdateProgress(updateRequest);
            progress += 0.1f;
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("[GameUpdator] Update Completed");
        updateRequest.UpdateStatus(AppUpdateStatus.Downloaded);
        OnUpdateCompleted(updateRequest);
        if (OnAskForInstallUpdate != null)
        {
            OnAskForInstallUpdate(updateRequest);
        }
        else
        {
            InstallUpdate();
        }
    }
#endif

    IEnumerator CheckForUpdate_Internal()
    {
        var appUpdateManager = new AppUpdateManager();
        var appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();
        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfo = appUpdateInfoOperation.GetResult();
            if (appUpdateInfo.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                var stalenessDays = appUpdateInfo.ClientVersionStalenessDays;
                if (GlobalConfig.Ins.stalenessDays <= 0
                    || (stalenessDays != null && stalenessDays >= GlobalConfig.Ins.stalenessDays))
                {
                    if (GlobalConfig.Ins.updateType == AppUpdateType.Flexible)
                    {
                        yield return StartFlexibleUpdate(appUpdateManager, appUpdateInfo);
                    }
                    else
                    {
                        yield return StartImmediateUpdate(appUpdateManager, appUpdateInfo);
                    }
                }
            }
        }
    }

    IEnumerator StartFlexibleUpdate(AppUpdateManager appUpdateManager, AppUpdateInfo appUpdateInfo)
    {
        // Creates an AppUpdateRequest that can be used to monitor the
        // requested in-app update flow.
        var allowAssetPackDeletion = appUpdateInfo.IsUpdateTypeAllowed(AppUpdateOptions.FlexibleAppUpdateOptions());
        var updateOptions = AppUpdateOptions.FlexibleAppUpdateOptions(allowAssetPackDeletion);
        var updateRequest = appUpdateManager.StartUpdate(appUpdateInfo, updateOptions);

        OnUpdateStarted(updateRequest);

        while (!updateRequest.IsDone)
        {
            OnUpdateProgress(updateRequest);
            yield return null;
        }

        OnUpdateCompleted(updateRequest);
        if (OnAskForInstallUpdate != null)
        {
            s_appUpdateManager = appUpdateManager;
            OnAskForInstallUpdate(updateRequest);
        }
        else
        {
            yield return appUpdateManager.CompleteUpdate();
        }
    }

    IEnumerator StartImmediateUpdate(AppUpdateManager appUpdateManager, AppUpdateInfo appUpdateInfo)
    {
        // Creates an AppUpdateRequest that can be used to monitor the
        // requested in-app update flow.
        var allowAssetPackDeletion = appUpdateInfo.IsUpdateTypeAllowed(AppUpdateOptions.FlexibleAppUpdateOptions());
        var updateOptions = AppUpdateOptions.ImmediateAppUpdateOptions(allowAssetPackDeletion);
        var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfo, updateOptions);
        yield return startUpdateRequest;

        Debug.LogError("StartImmediateUpdate should not be reach");

        // If the update completes successfully, then the app restarts and this line
        // is never reached. If this line is reached, then handle the failure (for
        // example, by logging result.Error or by displaying a message to the user).
    }

    public class SimulateAppRequest : AppUpdateRequest
    {
        internal void UpdateProgress(float progress)
        {
            DownloadProgress = progress;
            IsDone = progress >= 1f;
        }

        internal void UpdateStatus(AppUpdateStatus status)
        {
            Status = status;
        }
    }
}
#endif