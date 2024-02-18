//
//  AdmobAdsHelper.m
//  UnityFramework
//
//  Created by Admin on 28/08/2022.
//

#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
#import "nativetemplates/GADTTemplateView.h"
#import "nativetemplates/GADTMediumTemplateView.h"
#import "nativetemplates/GADTSmallTemplateView.h"
#import "AdmobAdsHelper.h"
#import "NativeViewController.h"
#import "AppOpenAdHandler.h"
#import "RewardInterHandler.h"


static bool s_delayRefresh;
static NSInteger s_delaySeconds;

@implementation AdmobAdsHelper

static NativeViewController* _smallNativeView;
static NativeViewController* _mediumNativeView;

+ (void)init
{
    _smallNativeView = [[NativeViewController alloc] init];
    _mediumNativeView = [[NativeViewController alloc] init];
}

+ (NativeViewController*) smallNativeView { return _smallNativeView;}
+ (NativeViewController*) mediumNativeView { return _mediumNativeView;}

+ (void) initGA:(BOOL) delayRefresh withDelaySeconds:(NSInteger) seconds
{
    s_delayRefresh = delayRefresh;
    s_delaySeconds = seconds;
    
    [GADMobileAds.sharedInstance startWithCompletionHandler:^( GADInitializationStatus* status ) {
        NSLog(@"GAMobileAds startWithCompletionHandler %@", [status.adapterStatusesByClassName description] );
        [AdmobAdsHelper callUnityObject:"OnInit" Parameter:""];
    }];
}

+ (void) setViewController:(UIViewController*) rootView
{
    GADTMediumTemplateView* mediumNativeView = [[GADTMediumTemplateView alloc] init];
    [_mediumNativeView init:mediumNativeView withParent:rootView.view withType:1];
    
    GADTSmallTemplateView* smallNativeView = [[GADTSmallTemplateView alloc] init];
    [_smallNativeView init:smallNativeView withParent:rootView.view withType:0];
    
    [[RewardInterHandler sharedInstance] setRootView:rootView];
}

+ (NativeViewController*) getNativeAdView:(NSString*) nativeAdType
{
    return [nativeAdType isEqualToString:@"0"] ? _smallNativeView : _mediumNativeView;
}

+ (void)tryToPresentAd:(UIViewController *)rootView
{
    [[AppOpenAdHandler sharedInstance] tryToPresentAd:rootView];
}

+ (void) callUnityObject:(const char*)method Parameter:(const char*)parameter
{
    UnitySendMessage("VGame_NativeAdmob", method, parameter);
}

@end

static NativeViewController* loadingController = nullptr;
static NativeViewController* pendingController = nullptr;
static float delayLoadAds = 2;

void LoadNativeAds(NativeViewController* controller) {
    if(loadingController == nullptr) {
//        printf("BAO - LoadNativeAds %ld\n", [controller adType]);
        loadingController = controller;
        [loadingController loadAds:^(GADNativeAd* na) {
//            printf("BAO - LoadNativeAds callback %ld\n", [controller adType]);
            // load pending
            loadingController = nullptr;
            if (pendingController != nullptr) {
                LoadNativeAds(pendingController);
                pendingController = nullptr;
            }
            
            // if load failed, add to pending
            if (na == nullptr) {
//                printf("BAO - Retry %ld\n", [controller adType]);
                dispatch_time_t popTime = dispatch_time(DISPATCH_TIME_NOW, delayLoadAds * NSEC_PER_SEC);
                dispatch_after(popTime, dispatch_get_main_queue(), ^(void) {
                    LoadNativeAds(controller);
                });
            }
        }];
    } else if([loadingController adType] != [controller adType]) {
//        printf("BAO - Pending LoadNativeAds %ld\n", [controller adType]);
        pendingController = controller;
    }
}

extern "C" {
    void NativeAdMob_Init(bool delayRefresh, bool delaySeconds) {
        [AdmobAdsHelper initGA:delaySeconds withDelaySeconds:delaySeconds];
    }

    // app open ads
    void InitAppOpenAd_iOS(const char* adUnitId, int delayForSeconds) {
        NSString* _adUnitId = [[NSString alloc] initWithUTF8String:adUnitId];
        [[AppOpenAdHandler sharedInstance] requestAppOpenAd:_adUnitId withDelay:delayForSeconds];
        [[AppOpenAdHandler sharedInstance] updateNextShowTime:0];
    }

    // app open ads
    void UpdateAppOpenAdsTime_iOS() {
        NSInteger delayForSeconds = [[AppOpenAdHandler sharedInstance] delayForSeconds];
        [[AppOpenAdHandler sharedInstance] updateNextShowTime: delayForSeconds];
    }

    void SetAppOpenAdsDisplayable_iOS(int displayable) {
        [[AppOpenAdHandler sharedInstance] setDisplayable:displayable == 1];
    }

    // reward inter
    void LoadRewardedInterstitial_iOS(const char* adunitId) {
        NSString* _adUnitId = [[NSString alloc] initWithUTF8String:adunitId];
        [[RewardInterHandler sharedInstance] loadAds:_adUnitId];
    }

    void ShowRewardedInterstitial_iOS() {
        [[RewardInterHandler sharedInstance] show];
    }

    // native ads
    void InitNativeAds_iOS(const char* m_adUnitIdSmall, const char* m_adUnitIdMedium) {
        NSString* adUnitIdSmall = [[NSString alloc] initWithUTF8String:m_adUnitIdSmall];
        NSString* adUnitIdMedium = [[NSString alloc] initWithUTF8String:m_adUnitIdMedium];
        [_smallNativeView setAdUnit:adUnitIdSmall];
        [_mediumNativeView setAdUnit:adUnitIdMedium];

        if ([adUnitIdSmall length] != 0) LoadNativeAds(_smallNativeView);
        if ([adUnitIdMedium length] != 0) LoadNativeAds(_mediumNativeView);
    }

    void LoadNativeAds_iOS(const char* m_nativeType) {
        NSString* nativeAdType = [[NSString alloc] initWithUTF8String:m_nativeType];
        NativeViewController* nativeViewController = [AdmobAdsHelper getNativeAdView:nativeAdType];
        [nativeViewController loadAds:nil];
    }

    void ShowNativeAds_iOS(const char* m_nativeType, const char* m_nativeLeft, const char* m_nativeTop, const char* m_nativeWidth, const char* m_nativeHeight, const char* m_nativeTextColor, const char* m_nativeActionColor ) {
        NSString* nativeAdType = [[NSString alloc] initWithUTF8String:m_nativeType];
        NSString* nativeWidth = [[NSString alloc] initWithUTF8String:m_nativeWidth];
        NSString* nativeHeight = [[NSString alloc] initWithUTF8String:m_nativeHeight];
        NSString* nativeTop = [[NSString alloc] initWithUTF8String:m_nativeTop];
        NSString* nativeLeft = [[NSString alloc] initWithUTF8String:m_nativeLeft];
        NSString* nativeTextColor = [[NSString alloc] initWithUTF8String:m_nativeTextColor];
        NSString* nativeActionColor = [[NSString alloc] initWithUTF8String:m_nativeActionColor];
        
        NativeViewController* nativeViewController = [AdmobAdsHelper getNativeAdView:nativeAdType];
        if([nativeViewController haveAds])
        {
            [nativeViewController setAlignments:nativeTop left:nativeLeft width:nativeWidth height:nativeHeight];
            [nativeViewController setTextColor:nativeTextColor];
            [nativeViewController setActionColor:nativeActionColor];
            [nativeViewController showView];
        }
    }

    void HideNativeAds_iOS(const char* m_nativeType) {
        NSString* nativeAdType = [[NSString alloc] initWithUTF8String:m_nativeType];
        NativeViewController* nativeViewController = [AdmobAdsHelper getNativeAdView:nativeAdType];
        [nativeViewController hideView];
        
        if(!s_delayRefresh
           || nativeViewController.clicked
           || [[NSDate date] compare:[nativeViewController.showTime dateByAddingTimeInterval:s_delaySeconds]] == NSOrderedDescending) {
            LoadNativeAds(nativeViewController);
        }
    }

    bool IsNativeAdsReady_iOS(const char* m_nativeType) {
        NSString* nativeAdType = [[NSString alloc] initWithUTF8String:m_nativeType];
        NativeViewController* nativeViewController = [AdmobAdsHelper getNativeAdView:nativeAdType];
        return [nativeViewController haveAds];
    }
}
