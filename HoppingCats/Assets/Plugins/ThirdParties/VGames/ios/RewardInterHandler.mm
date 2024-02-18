//
//  AppOpenAdHandler.m
//  Unity-iPhone
//
//  Created by Admin on 30/08/2022.
//

#import "AdmobAdsHelper.h"
#import "RewardInterHandler.h"

@interface RewardInterHandler() <GADFullScreenContentDelegate>

@end

@implementation RewardInterHandler : NSObject

+(id)sharedInstance {
    static RewardInterHandler *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [self alloc];
    });
    return sharedInstance;
}

- (void) loadAds:(NSString*) adUnitId
{
    NSLog(@"BAO - Load reward inter ad");
    
    _adUnitId = adUnitId;
    [GADRewardedInterstitialAd
           loadWithAdUnitID:adUnitId
                    request:[GADRequest request]
          completionHandler:^(GADRewardedInterstitialAd* _Nullable rewardedInterstitialAd, NSError* _Nullable error) {
        
        if (error) {
            NSLog(@"BAO - Failed to load reward inter ad: %@", error);
            
//            NSString* params = [NSString stringWithFormat:@"%@", self.adUnitId];
            [AdmobAdsHelper callUnityObject:"RewardedInterstitial_OnAdFailedToLoad" Parameter:""];
            return;
        }
        
        NSLog(@"BAO - Reward inter ad Loaded");
        
        NSString* adSourceName = rewardedInterstitialAd.responseInfo.loadedAdNetworkResponseInfo.adSourceName;
        
//        NSString* params = [NSString stringWithFormat:@"%@", self.adUnitId];
        [AdmobAdsHelper callUnityObject:"RewardedInterstitial_onAdLoaded" Parameter:""];
        
        self.rewardedInterstitialAd = rewardedInterstitialAd;
        self.rewardedInterstitialAd.fullScreenContentDelegate = self;
        self.rewardedInterstitialAd.paidEventHandler = ^(GADAdValue* adValue) {
            NSDecimalNumber* valueMicros = [adValue value];
            NSString* currencyCode = [adValue currencyCode];
            GADAdValuePrecision precisionType = [adValue precision];

            NSLog(@"BAO - OnPaidEvent %@ %@ %ld", valueMicros, currencyCode, precisionType);

            NSString* params = [NSString stringWithFormat:@"%@_%@_%@_%@_%ld", adUnitId, adSourceName, valueMicros, currencyCode, precisionType];
            [AdmobAdsHelper callUnityObject:"RewardedInterstitial_onPaidEvent" Parameter:[params UTF8String]];
        };
//        self.loadTime = [NSDate date];
    }];
}

- (void)show {
  [_rewardedInterstitialAd
        presentFromRootViewController:_rootView
             userDidEarnRewardHandler:^ {
      
      GADAdReward *reward = self.rewardedInterstitialAd.adReward;
      
      NSLog(@"BAO - OnRewarded %@ %@", reward.type, reward.amount);
      
      NSString* params = [NSString stringWithFormat:@"%@_%@", reward.type, reward.amount];
      [AdmobAdsHelper callUnityObject:"RewardedInterstitial_OnRewarded" Parameter:[params UTF8String]];
      
  }];
}

#pragma mark - GADFullScreenContentDelegate

/// Tells the delegate that the ad failed to present full screen content.
- (void)ad:(nonnull id<GADFullScreenPresentingAd>)ad didFailToPresentFullScreenContentWithError:(nonnull NSError *)error {
    NSLog(@"BAO - didFailToPresentFullScreenContentWithError");

    [AdmobAdsHelper callUnityObject:"RewardedInterstitial_onAdFailedToShow" Parameter:""];
}

/// Tells the delegate that the ad will present full screen content.
- (void)adWillPresentFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
    NSLog(@"BAO - adWillPresentFullScreenContent");
    
    [AdmobAdsHelper callUnityObject:"RewardedInterstitial_onAdShowed" Parameter:""];
}

/// Tells the delegate that the ad dismissed full screen content.
- (void)adDidDismissFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
    NSLog(@"BAO - adDidDismissFullScreenContent");

    [AdmobAdsHelper callUnityObject:"RewardedInterstitial_OnDismissed" Parameter:""];
}

/// Tells the delegate that an impression has been recorded for the ad.
- (void)adDidRecordImpression:(nonnull id<GADFullScreenPresentingAd>)ad {
    NSLog(@"BAO - adDidRecordImpression");
    
    [AdmobAdsHelper callUnityObject:"RewardedInterstitial_onAdImpression" Parameter:""];
}
@end
