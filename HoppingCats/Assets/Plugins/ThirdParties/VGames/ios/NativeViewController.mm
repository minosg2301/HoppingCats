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

#import "AdmobAdsHelper.h"
#import "NativeViewController.h"
#import <GoogleMobileAds/GoogleMobileAds.h>
#import "nativetemplates/GADTTemplateView.h"

@interface NativeViewController () <GADNativeAdLoaderDelegate,
GADVideoControllerDelegate,
GADNativeAdDelegate>

/// You must keep a strong reference to the GADAdLoader during the ad loading
/// process.
@property(nonatomic, strong) GADAdLoader *adLoader;

/// The native ad view that is being presented.
@property(nonatomic, strong) GADTTemplateView* _nativeAdView;

/// The height constraint applied to the ad view, where necessary.
@property(nonatomic, strong) NSLayoutConstraint *heightConstraint;

@end

@implementation NativeViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
}

- (void) setAdUnit:(NSString *)adUnitId
{
    self.m_adUnitId = adUnitId;
}

- (void) init:(GADTTemplateView*) nativeAdView withParent:(UIView *)parentView withType:(NSInteger) nativeAdType
{
    self.adType = nativeAdType;
    self._nativeAdView = nativeAdView;
    [parentView addSubview:nativeAdView];
    
    NSString *myBlueColor = @"#5C84F0";
    NSDictionary *styles = @{
        GADTNativeTemplateStyleKeyCallToActionFont : [UIFont systemFontOfSize:15.0],
        GADTNativeTemplateStyleKeyCallToActionFontColor : UIColor.whiteColor,
        GADTNativeTemplateStyleKeyCallToActionBackgroundColor : [GADTTemplateView colorFromHexString:myBlueColor],
        GADTNativeTemplateStyleKeySecondaryFont : [UIFont systemFontOfSize:15.0],
        GADTNativeTemplateStyleKeySecondaryFontColor : UIColor.grayColor,
        GADTNativeTemplateStyleKeySecondaryBackgroundColor : UIColor.whiteColor,
        GADTNativeTemplateStyleKeyPrimaryFont : [UIFont systemFontOfSize:15.0],
        GADTNativeTemplateStyleKeyPrimaryFontColor : UIColor.blackColor,
        GADTNativeTemplateStyleKeyPrimaryBackgroundColor : UIColor.whiteColor,
        GADTNativeTemplateStyleKeyTertiaryFont : [UIFont systemFontOfSize:15.0],
        GADTNativeTemplateStyleKeyTertiaryFontColor : UIColor.grayColor,
        GADTNativeTemplateStyleKeyTertiaryBackgroundColor : UIColor.whiteColor,
        GADTNativeTemplateStyleKeyMainBackgroundColor : [UIColor clearColor],
        GADTNativeTemplateStyleKeyCornerRadius : [NSNumber numberWithFloat:7.0],
    };
    
    self._nativeAdView.styles = styles;
    self._nativeAdView.translatesAutoresizingMaskIntoConstraints = NO;
    [self._nativeAdView setHidden:YES];
}

- (void) setTextColor:(NSString*) textColor
{
    NSLog(@"PHONG - setTextColor %@",  textColor);
    
    GADTTemplateView* nativeAdView = self._nativeAdView;
    
    //if([self.m_adUnitType isEqualToString:@"0"]){ //SMALL
    if(self.adType == 1) //BIG
    {
        ((UIImageView*)nativeAdView.iconView).superview.superview.superview.backgroundColor = [UIColor clearColor];
    }
    
    nativeAdView.layer.borderWidth = 0;
    nativeAdView.rootView.backgroundColor = [UIColor clearColor];
    nativeAdView.adBadge.layer.masksToBounds = TRUE;
    ((UILabel*)nativeAdView.headlineView).backgroundColor = [UIColor clearColor];
    ((UILabel*)nativeAdView.advertiserView).backgroundColor = [UIColor clearColor];
    ((UILabel*)nativeAdView.bodyView).backgroundColor = [UIColor clearColor];
    ((UILabel*)nativeAdView.storeView).backgroundColor = [UIColor clearColor];
    ((UILabel*)nativeAdView.starRatingView).backgroundColor = [UIColor clearColor];
    ((UIImageView*)nativeAdView.iconView).superview.backgroundColor = [UIColor clearColor];
    ((UIImageView*)nativeAdView.mediaView).backgroundColor = [UIColor clearColor];
    ((UILabel*)nativeAdView.starRatingView).textColor = [self colorWithHexString:@"#4D99FA"];
    
    if(textColor) {
        ((UILabel*)nativeAdView.headlineView).textColor = [self colorWithHexString:textColor];
        ((UILabel*)nativeAdView.advertiserView).textColor = [self colorWithHexString:textColor];
        ((UILabel*)nativeAdView.bodyView).textColor = [self colorWithHexString:textColor];
        ((UILabel*)nativeAdView.storeView).textColor = [self colorWithHexString:textColor];
    }
    
    [((UIButton*)nativeAdView.callToActionView) setBackgroundColor:[UIColor clearColor]];
    [((UIButton*)nativeAdView.callToActionView) setBackgroundImage:[UIImage imageNamed:@"Data/Raw/native_button.png"] forState:UIControlStateNormal];
    [((UIButton*)nativeAdView.callToActionView) setBackgroundImage:[UIImage imageNamed:@"Data/Raw/native_button.png"] forState:UIControlStateHighlighted];
}

- (void) setActionColor:(NSString*) actionColor
{
    NSLog(@"PHONG - setActionColor %@",  actionColor);
    ((UIButton*)self._nativeAdView.callToActionView).titleLabel.textColor = [self colorWithHexString:actionColor];
}

- (BOOL) checkUpdateConstraint:(NSLayoutConstraint*)constraint with:(NSLayoutAttribute) layoutAttribute ofView:(UIView*) view
{
    return constraint.firstAttribute == layoutAttribute
            && constraint.firstItem == view.superview
            && constraint.secondAttribute == layoutAttribute
            && constraint.secondItem == view;
}

- (void) setAlignments:(NSString*) nativeTop
                  left:(NSString*) nativeLeft
                 width:(NSString*) nativeWidth
                height:(NSString*) nativeHeight
{
//    NSLog(@"PHONG - setAlignments %@, %@, %@, %@", nativeLeft, nativeTop, nativeWidth, nativeHeight);
    
    GADTTemplateView* nativeAdView = self._nativeAdView;
    [nativeAdView addHorizonCenterConstraintToSuperview];
    if(nativeAdView.superview) {
        
        CGRect bounds = [UIScreen mainScreen].bounds;
//        NSLog(@"PHONG - bounds %f, %f", bounds.size.width, bounds.size.height);
        
        CGFloat top = -[nativeTop floatValue] * bounds.size.height;
        CGFloat left = -[nativeLeft floatValue] * bounds.size.width;
        CGFloat width = [nativeWidth floatValue] * bounds.size.width;
        CGFloat height = [nativeHeight floatValue] * bounds.size.height;
        
//        NSLog(@"PHONG - rect %f, %f, %f, %f", left, top, width, height);
        bool shouldAddNewWidth = true;
        bool shouldAddNewHeight = true;
        bool shouldAddNewTop = true;
        bool shouldAddNewLeft = true;
        for(NSLayoutConstraint *constraint in nativeAdView.superview.constraints)
        {
//            NSLog(@"PHONG - alignNativeAds contraint %ld | %ld | %ld | %ld", constraint.firstAttribute, NSLayoutAttributeWidth, NSLayoutAttributeHeight, NSLayoutAttributeTop);
            if([self checkUpdateConstraint:constraint with:NSLayoutAttributeLeft ofView:nativeAdView]) {
//                NSLog(@"PHONG - alignNativeAds HIT updateLeft");
                shouldAddNewLeft = false;
                constraint.constant = left;
            }
            if([self checkUpdateConstraint:constraint with:NSLayoutAttributeWidth ofView:nativeAdView]) {
//                NSLog(@"PHONG - alignNativeAds HIT updateWidth");
                shouldAddNewWidth = false;
                constraint.constant = bounds.size.width - width;
            }
            if([self checkUpdateConstraint:constraint with:NSLayoutAttributeHeight ofView:nativeAdView]) {
//                NSLog(@"PHONG - alignNativeAds HIT updateHeight");
                shouldAddNewHeight = false;
                constraint.constant = bounds.size.height - height;
            }
            if([self checkUpdateConstraint:constraint with:NSLayoutAttributeTop ofView:nativeAdView]) {
//                NSLog(@"PHONG - alignaNativeAds HIT updateTop");
                shouldAddNewTop = false;
                constraint.constant = top;
            }
        }
//        NSLog(@"PHONG - alignNativeAds 2 %d, %d, %d", shouldAddNewWidth, shouldAddNewHeight, shouldAddNewTop);
        
        if(shouldAddNewLeft) {
            [nativeAdView.superview addConstraint:[NSLayoutConstraint constraintWithItem:nativeAdView.superview
                                                                               attribute:NSLayoutAttributeLeft
                                                                               relatedBy:NSLayoutRelationEqual
                                                                                  toItem:nativeAdView
                                                                               attribute:NSLayoutAttributeLeft
                                                                              multiplier:1
                                                                                constant:left]];
        }
        
        if(shouldAddNewWidth) {
            [nativeAdView.superview addConstraint:[NSLayoutConstraint constraintWithItem:nativeAdView.superview
                                                                               attribute:NSLayoutAttributeWidth
                                                                               relatedBy:NSLayoutRelationEqual
                                                                                  toItem:nativeAdView
                                                                               attribute:NSLayoutAttributeWidth
                                                                              multiplier:1
                                                                                constant:bounds.size.width - width]];
        }
        
        if(shouldAddNewHeight) {
            [nativeAdView.superview addConstraint:[NSLayoutConstraint constraintWithItem:nativeAdView.superview
                                                                               attribute:NSLayoutAttributeHeight
                                                                               relatedBy:NSLayoutRelationEqual
                                                                                  toItem:nativeAdView
                                                                               attribute:NSLayoutAttributeHeight
                                                                              multiplier:1
                                                                                constant:bounds.size.height - height]];
        }

        if(shouldAddNewTop) {
            [nativeAdView.superview addConstraint:[NSLayoutConstraint constraintWithItem:nativeAdView.superview
                                                                               attribute:NSLayoutAttributeTop
                                                                               relatedBy:NSLayoutRelationEqual
                                                                                  toItem:nativeAdView
                                                                               attribute:NSLayoutAttributeTop
                                                                              multiplier:1
                                                                                constant:top]];
        }
        
        //[self.templateView.superview setNeedsUpdateConstraints];
    }
    
    if(self.adType == 1) // BIG
    {
        nativeAdView.mediaView.translatesAutoresizingMaskIntoConstraints = NO;
        bool shouldAddNewHeightMediaView = true;
        for(NSLayoutConstraint *constraint in nativeAdView.constraints)
        {
            if(constraint.firstAttribute == NSLayoutAttributeHeight && constraint.firstItem == nativeAdView.mediaView) {
                shouldAddNewHeightMediaView = false;
            }
        }
        
        if(shouldAddNewHeightMediaView) {
            [nativeAdView addConstraint:[NSLayoutConstraint
                                         constraintWithItem:nativeAdView.mediaView
                                         attribute:NSLayoutAttributeHeight
                                         relatedBy:NSLayoutRelationEqual
                                         toItem:nativeAdView
                                         attribute:NSLayoutAttributeHeight
                                         multiplier:0.45
                                         constant:0]];
        }
    }
}

- (void) showView
{
    NSLog(@"BAO - showView %ld", self.adType);
    if([self firstShow]) {
        self.showTime = [NSDate date];
        self.firstShow = false;
    }
    [self._nativeAdView setHidden:NO];
}

- (void) hideView
{
    NSLog(@"BAO - hideView %ld", self.adType);
    
    if(self._nativeAdView.isHidden) return;
    
    [self._nativeAdView setHidden:YES];
}

- (bool) haveAds
{
    return self._nativeAdView.nativeAd != NULL;
}

- (bool) isShowing
{
    return !self._nativeAdView.isHidden;
}

- (IBAction)loadAds:(void(^)(GADNativeAd*))handler
{
    // Loads an ad for unified native ad.
    NSLog(@"BAO - loadAds %ld", self.adType);
    
    self._nativeAdView.nativeAd = NULL;
    self.clicked = false;
    
    GADVideoOptions *videoOptions = [[GADVideoOptions alloc] init];
    videoOptions.startMuted = true;
    
    GADMultipleAdsAdLoaderOptions *multiOptions = [[GADMultipleAdsAdLoaderOptions alloc] init];
    multiOptions.numberOfAds = 1;
    
    _loadAdHanlder = [handler copy];
    self.adLoader = [[GADAdLoader alloc] initWithAdUnitID:self.m_adUnitId
                                       rootViewController:self
                                                  adTypes:@[ GADAdLoaderAdTypeNative ]
                                                  options:@[ videoOptions, multiOptions ]];
    self.adLoader.delegate = self;
    [self.adLoader loadRequest:[GADRequest request]];
}

/// Gets an image representing the number of stars. Returns nil if rating is
/// less than 3.5 stars.
- (UIImage *)imageForStars:(NSDecimalNumber *)numberOfStars
{
    double starRating = numberOfStars.doubleValue;
    if (starRating >= 5) {
        return [UIImage imageNamed:@"stars_5"];
    } else if (starRating >= 4.5) {
        return [UIImage imageNamed:@"stars_4_5"];
    } else if (starRating >= 4) {
        return [UIImage imageNamed:@"stars_4"];
    } else if (starRating >= 3.5) {
        return [UIImage imageNamed:@"stars_3_5"];
    } else {
        return nil;
    }
}

#pragma mark GADAdLoaderDelegate implementation

- (void)adLoader:(GADAdLoader *)adLoader didFailToReceiveAdWithError:(NSError *)error
{
    NSLog(@"BAO - LoadAd %ld failed with error: %@", [self adType], error);
    self._nativeAdView.nativeAd = NULL;
    if(_loadAdHanlder != nullptr) {
        void(^temp)(GADNativeAd*) = _loadAdHanlder;
        _loadAdHanlder = nullptr;
        temp(nullptr);
    };
}

#pragma mark GADNativeAdLoaderDelegate implementation

- (void)adLoader:(GADAdLoader *)adLoader didReceiveNativeAd:(GADNativeAd *)nativeAd
{
    nativeAd.delegate = self;
    
    NSString* adUnit = self.m_adUnitId;
    NSString* adSourceName = nativeAd.responseInfo.loadedAdNetworkResponseInfo.adSourceName;
    
    self.firstShow = true;
    self._nativeAdView.nativeAd = nativeAd;
    self._nativeAdView.nativeAd.paidEventHandler = ^(GADAdValue* adValue) {
        NSDecimalNumber* valueMicros = [adValue value];
        NSString* currencyCode = [adValue currencyCode];
        GADAdValuePrecision precisionType = [adValue precision];

        NSLog(@"BAO - OnPaidEvent %@ %@ %ld",valueMicros, currencyCode, precisionType);

        NSString* params = [NSString stringWithFormat:@"%@_%@_%@_%@_%ld", adUnit, adSourceName, valueMicros, currencyCode, precisionType];
        [AdmobAdsHelper callUnityObject:"OnNativeAdPaid" Parameter:[params UTF8String]];
    };
    
    NSLog(@"BAO - Load Success %ld", [self adType]);
    
    NSString* params = [NSString stringWithFormat:@"%ld", self.adType];
    [AdmobAdsHelper callUnityObject:"OnNativeAdLoaded" Parameter:[params UTF8String]];
    if(_loadAdHanlder != nullptr) {
        _loadAdHanlder(nativeAd);
        _loadAdHanlder = nullptr;
    };
}

#pragma mark GADVideoControllerDelegate implementation

- (void)videoControllerDidEndVideoPlayback:(GADVideoController *)videoController {
    
}

#pragma mark GADNativeAdDelegate

- (void)nativeAdDidRecordClick:(GADNativeAd *)nativeAd {
    NSLog(@"%s", __PRETTY_FUNCTION__);
    self.clicked = true;
}

- (void)nativeAdDidRecordImpression:(GADNativeAd *)nativeAd {
    NSLog(@"%s", __PRETTY_FUNCTION__);
}

- (void)nativeAdWillPresentScreen:(GADNativeAd *)nativeAd {
    NSLog(@"%s", __PRETTY_FUNCTION__);
}

- (void)nativeAdWillDismissScreen:(GADNativeAd *)nativeAd {
    NSLog(@"%s", __PRETTY_FUNCTION__);
}

- (void)nativeAdDidDismissScreen:(GADNativeAd *)nativeAd {
    NSLog(@"%s", __PRETTY_FUNCTION__);
}

- (void)nativeAdWillLeaveApplication:(GADNativeAd *)nativeAd {
    NSLog(@"%s", __PRETTY_FUNCTION__);
}

#pragma mark ColorHelper
- (UIColor *)colorWithHexString:(NSString *)hexString {
    NSString *colorString = [[hexString stringByReplacingOccurrencesOfString: @"#" withString: @""] uppercaseString];
    CGFloat alpha, red, blue, green;
    
    switch ([colorString length]) {
        case 3: // #RGB
            alpha = 1.0f;
            red   = [self colorComponentFrom: colorString start: 0 length: 1];
            green = [self colorComponentFrom: colorString start: 1 length: 1];
            blue  = [self colorComponentFrom: colorString start: 2 length: 1];
            break;
        case 4: // #ARGB
            alpha = [self colorComponentFrom: colorString start: 0 length: 1];
            red   = [self colorComponentFrom: colorString start: 1 length: 1];
            green = [self colorComponentFrom: colorString start: 2 length: 1];
            blue  = [self colorComponentFrom: colorString start: 3 length: 1];
            break;
        case 6: // #RRGGBB
            alpha = 1.0f;
            red   = [self colorComponentFrom: colorString start: 0 length: 2];
            green = [self colorComponentFrom: colorString start: 2 length: 2];
            blue  = [self colorComponentFrom: colorString start: 4 length: 2];
            break;
        case 8: // #AARRGGBB
            alpha = [self colorComponentFrom: colorString start: 0 length: 2];
            red   = [self colorComponentFrom: colorString start: 2 length: 2];
            green = [self colorComponentFrom: colorString start: 4 length: 2];
            blue  = [self colorComponentFrom: colorString start: 6 length: 2];
            break;
        default:
            [NSException raise:@"Invalid color value"
                        format: @"Color value %@ is invalid.  It should be a hex value of the form #RBG, #ARGB, #RRGGBB, or #AARRGGBB", hexString];
            break;
    }
    return [UIColor colorWithRed: red green: green blue: blue alpha: alpha];
}

- (CGFloat)colorComponentFrom:(NSString *)string start:(NSUInteger)start length:(NSUInteger)length {
    NSString *substring = [string substringWithRange: NSMakeRange(start, length)];
    NSString *fullHex = length == 2 ? substring : [NSString stringWithFormat: @"%@%@", substring, substring];
    unsigned hexComponent;
    [[NSScanner scannerWithString: fullHex] scanHexInt: &hexComponent];
    return hexComponent / 255.0;
}
@end
