//
//  AppOpenAdHandler.m
//  Unity-iPhone
//
//  Created by Admin on 30/08/2022.
//

#include "AdmobAdsHelper.h"
#include "AppOpenAdHandler.h"

@interface AppOpenAdHandler() <GADFullScreenContentDelegate>

@end

static BOOL s_displayable = true;

@implementation AppOpenAdHandler : NSObject

+(id)sharedInstance {
    static AppOpenAdHandler *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [self alloc];
    });
    return sharedInstance;
}

- (BOOL)wasLoadTimeLessThanNHoursAgo:(int)n {
    NSDate *now = [NSDate date];
    NSTimeInterval timeIntervalBetweenNowAndLoadTime = [now timeIntervalSinceDate:self.loadTime];
    double secondsPerHour = 3600.0;
    double intervalInHours = timeIntervalBetweenNowAndLoadTime / secondsPerHour;
    return intervalInHours < n;
}

- (void)updateNextShowTime:(NSInteger) seconds {
    self.nextShowTime = [[NSDate date] dateByAddingTimeInterval:seconds];
}

- (void)requestAppOpenAd:(NSString*) adUnitId withDelay:(NSInteger) delayForSeconds
{
    _delayForSeconds = delayForSeconds;
    _adUnitId = adUnitId;
    _appOpenAd = nil;
    [GADAppOpenAd loadWithAdUnitID:adUnitId
                           request:[GADRequest request]
                       orientation:UIInterfaceOrientationPortrait
                 completionHandler:^(GADAppOpenAd *_Nullable appOpenAd, NSError *_Nullable error) {
        if (error) {
            NSLog(@"BAO - Failed to load app open ad: %@", error);
            
            NSString* params = [NSString stringWithFormat:@"%@", self.adUnitId];
            [AdmobAdsHelper callUnityObject:"AppOpenAd_onAdLoadFailed" Parameter:[params UTF8String]];
            return;
        }
        
        NSLog(@"BAO - App open ad Loaded");
        
        NSString* adSourceName = appOpenAd.responseInfo.loadedAdNetworkResponseInfo.adSourceName;
        
        NSString* params = [NSString stringWithFormat:@"%@", self.adUnitId];
        [AdmobAdsHelper callUnityObject:"AppOpenAd_onAdLoaded" Parameter:[params UTF8String]];
        
        self.appOpenAd = appOpenAd;
        self.appOpenAd.fullScreenContentDelegate = self;
        self.appOpenAd.paidEventHandler = ^(GADAdValue* adValue) {
            NSDecimalNumber* valueMicros = [adValue value];
            NSString* currencyCode = [adValue currencyCode];
            GADAdValuePrecision precisionType = [adValue precision];

            NSLog(@"BAO - OnPaidEvent %@ %@ %ld", valueMicros, currencyCode, precisionType);

            NSString* params = [NSString stringWithFormat:@"%@_%@_%@_%@_%ld", adUnitId, adSourceName, valueMicros, currencyCode, precisionType];
            [AdmobAdsHelper callUnityObject:"AppOpenAd_onPaidEvent" Parameter:[params UTF8String]];
        };
        self.loadTime = [NSDate date];
    }];
}

- (void)tryToPresentAd:(UIViewController*) rootView {
    
    if (!s_displayable) return;
    
    if ([self.nextShowTime compare:[NSDate date]] == NSOrderedDescending) {
        NSLog(@"BAO - Next show time is later than now");
        return;
    }
    
    if (_appOpenAd && [self wasLoadTimeLessThanNHoursAgo:4]) {
        [_appOpenAd presentFromRootViewController:rootView];
    } else if(_adUnitId) {
        [self requestAppOpenAd:_adUnitId withDelay:_delayForSeconds];
    }
}

- (void)setDisplayable:(BOOL) displayable {
    s_displayable = displayable;
}

#pragma mark - GADFullScreenContentDelegate

/// Tells the delegate that the ad failed to present full screen content.
- (void)ad:(nonnull id<GADFullScreenPresentingAd>)ad didFailToPresentFullScreenContentWithError:(nonnull NSError *)error {
    NSLog(@"BAO - didFailToPresentFullScreenContentWithError");

    [self requestAppOpenAd:_adUnitId withDelay:_delayForSeconds];
}

/// Tells the delegate that the ad will present full screen content.
- (void)adWillPresentFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
    NSLog(@"BAO - adWillPresentFullScreenContent");
    
    NSString* params = [NSString stringWithFormat:@"%@", _adUnitId];
    [AdmobAdsHelper callUnityObject:"AppOpenAd_onAdShowed" Parameter:[params UTF8String]];
}

/// Tells the delegate that the ad dismissed full screen content.
- (void)adDidDismissFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
    NSLog(@"BAO - adDidDismissFullScreenContent");

    [self requestAppOpenAd:_adUnitId withDelay:_delayForSeconds];
    [self updateNextShowTime:_delayForSeconds];
}

@end
