using GoogleMobileAds.Api;
using moonNest;

public class InitAds : BootStep
{
    public override string ToString() => "Init Ads";

    public override void OnStep()
    {
        // Initialize the Google Mobile Ads SDK. (Admod)
        MobileAds.Initialize(initStatus => { Complete(); });
    }
}
