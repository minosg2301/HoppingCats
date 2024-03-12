using UnityEditor;

namespace GoogleMobileAds.Editor
{
    public class GoogleMobileAdsSetAppId
    {
        private GoogleMobileAdsSettings gmas;

        public GoogleMobileAdsSetAppId()
        {
            gmas = GoogleMobileAdsSettings.LoadInstance();
        }

        public string GoogleMobileAdsAndroidAppId
        {
            get { return gmas.GoogleMobileAdsAndroidAppId; }

            set { gmas.GoogleMobileAdsAndroidAppId = value; EditorUtility.SetDirty(gmas); }
        }
    }
}