#pragma once
// Copyright (C) 2015 Google, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
#import "nativetemplates/GADTTemplateView.h"

@interface NativeViewController : UIViewController
{
    void (^_loadAdHanlder)(GADNativeAd *);
}

@property(nonatomic, assign) NSInteger adType;
@property(nonatomic, strong) NSString *m_adUnitId;
@property(weak, nonatomic) NSDate *showTime;
@property(nonatomic, assign) BOOL firstShow;
@property(nonatomic, assign) BOOL clicked;

- (void) init:(GADTTemplateView*) nativeAdView withParent:(UIView*) parentView withType:(NSInteger) nativeAdType;
- (void) setAdUnit:(NSString*) adUnitId;

- (IBAction) loadAds:(void(^)(GADNativeAd*))handler;

- (void) setAlignments:(NSString*) nativeTop left:(NSString*) nativeLeft width:(NSString*) nativeWidth height:(NSString*) nativeHeight;
- (void) setTextColor:(NSString*) textColor;
- (void) setActionColor:(NSString*) actionColor;

- (void) showView;
- (void) hideView;
- (bool) haveAds;
- (bool) isShowing;
@end
