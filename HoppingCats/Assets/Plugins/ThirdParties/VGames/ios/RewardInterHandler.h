//
//  AppOpenAdHandler.h
//  Unity-iPhone
//
//  Created by Admin on 30/08/2022.
//
#pragma once

#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GoogleMobileAds.h>

@interface RewardInterHandler : NSObject

@property(strong, nonatomic) GADRewardedInterstitialAd* rewardedInterstitialAd;
@property(strong, nonatomic) NSString* adUnitId;
@property(strong, nonatomic) UIViewController* rootView;

+ (id) sharedInstance;

- (void) loadAds:(NSString*) adUnitId;
@end
