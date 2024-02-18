//
//  AppOpenAdHandler.h
//  Unity-iPhone
//
//  Created by Admin on 30/08/2022.
//
#pragma once

#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GoogleMobileAds.h>

@interface AppOpenAdHandler : NSObject

@property(strong, nonatomic) GADAppOpenAd* appOpenAd;
@property(strong, nonatomic) NSString* adUnitId;
@property(weak, nonatomic) NSDate *loadTime;
@property(weak, nonatomic) NSDate *nextShowTime;
@property NSInteger delayForSeconds;

+ (id) sharedInstance;

- (void)requestAppOpenAd:(NSString*) adUnitId withDelay:(NSInteger) delayForSeconds;
- (void)tryToPresentAd:(UIViewController*) rootView;
- (void)updateNextShowTime:(NSInteger) seconds;
- (void)setDisplayable:(BOOL) displayable;

@end
