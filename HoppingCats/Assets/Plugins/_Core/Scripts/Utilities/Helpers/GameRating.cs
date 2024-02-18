#if UNITY_ANDROID
using Google.Play.Review;
#endif
using System.Collections;
using moonNest;

public class GameRating : SingletonMono<GameRating>
{
    public static void Show() => Ins.Show_Internal();

#if UNITY_ANDROID
    ReviewManager _reviewManager = null;
    IEnumerator ShowInAppReview()
    {
        _reviewManager ??= new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }

        var playReviewInfo = requestFlowOperation.GetResult();
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launchFlowOperation;
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            ApplicationExt.OpenStoreReview();
            yield break;
        }
    }
#endif

    public void Show_Internal()
    {
#if UNITY_ANDROID
        StartCoroutine(ShowInAppReview());
#elif UNITY_IOS
        ApplicationExt.OpenStoreReview();
#endif
    }
}