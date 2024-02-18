#import <UIKit/UIKit.h>
#include <UserMessagingPlatform/UserMessagingPlatform.h>
#import "VGamesAppController.h"
#import "UnityAppController.h"
#import "AdmobAdsHelper.h"

@implementation VGamesAppController

- (id)init {
    if(self = [super init]) {
        [AdmobAdsHelper init];
    }
    return self;
}

// overriding main unity entry point.
UIViewController* getCurrentRootViewController() {
    
    UIViewController *result = nil;
    
    // Try to find the root view controller programmically
    
    // Find the top window (that is not an alert view or other window)
    UIWindow *topWindow = [[UIApplication sharedApplication] keyWindow];
    if (topWindow.windowLevel != UIWindowLevelNormal) {
        NSArray *windows = [[UIApplication sharedApplication] windows];
        for(topWindow in windows) {
            if (topWindow.windowLevel == UIWindowLevelNormal) {
                break;
            }
        }
    }
    
    UIView *rootView = [[topWindow subviews] objectAtIndex:0];
    id nextResponder = [rootView nextResponder];
    if ([nextResponder isKindOfClass:[UIViewController class]])
        result = nextResponder;
    else if ([topWindow respondsToSelector:@selector(rootViewController)] && topWindow.rootViewController != nil)
        result = topWindow.rootViewController;
    
    return result;
}

-(void) startUnity: (UIApplication*) application
{
    [super startUnity: application];  //call the super.
    
    [AdmobAdsHelper setViewController:getCurrentRootViewController()];
}

-(void) applicationDidBecomeActive:(UIApplication *)application
{
    [super applicationDidBecomeActive:application];
    [AdmobAdsHelper tryToPresentAd:self.window.rootViewController];
}

+(void) updateConsentInfo {
    
    
    // Create a UMPRequestParameters object.
    UMPRequestParameters *parameters = [[UMPRequestParameters alloc] init];
    
//    UMPDebugSettings *debugSettings = [[UMPDebugSettings alloc] init];
//    debugSettings.testDeviceIdentifiers = @[ @"C527A023-49BA-4A1B-B8D7-F873AD121694" ];
//    debugSettings.geography = UMPDebugGeographyEEA;
    
    // Set tag for under age of consent. Here NO means users are not under age.
    parameters.tagForUnderAgeOfConsent = NO;
//    parameters.debugSettings = debugSettings;
    
//    [UMPConsentInformation.sharedInstance reset];
    
    // Request an update to the consent information.
    [UMPConsentInformation.sharedInstance
        requestConsentInfoUpdateWithParameters:parameters
        completionHandler:^(NSError *_Nullable error) {
            // The consent information has updated.
            if (error) {
                UnitySendMessage("ConsentInfoUpdator", "OnCompleted", "true");
                return;
            }
            
            // The consent information state was updated.
            // You are now ready to see if a form is available.
            if (UMPConsentInformation.sharedInstance.formStatus == UMPFormStatusAvailable) {
                [self loadForm];
            } else {
                [self sendConsentResult];
            }
        }];
}

+(void) loadForm {
    [UMPConsentForm loadWithCompletionHandler:^(UMPConsentForm *form,
                                                NSError *loadError) {
        if (loadError) {
            UnitySendMessage("ConsentInfoUpdator", "OnCompleted", "false");
            return;
        }
        
        // Present the form. You can also hold on to the reference to present
        // later.
        if (UMPConsentInformation.sharedInstance.consentStatus == UMPConsentStatusRequired) {
            [form
             presentFromViewController:getCurrentRootViewController()
             completionHandler:^(NSError *_Nullable dismissError) {
                [self sendConsentResult];
                
                // Handle dismissal by reloading form.
                [self loadForm];
            }];
        } else {
            [self sendConsentResult];
        }
    }];
}

+(void) sendConsentResult {
    if ([self canRequestAds]) {
        UnitySendMessage("ConsentInfoUpdator", "OnCompleted", "true");
    } else {
        UnitySendMessage("ConsentInfoUpdator", "OnCompleted", "false");
    }
}

+(bool) canRequestAds {
    return UMPConsentInformation.sharedInstance.consentStatus == UMPConsentStatusObtained
    || UMPConsentInformation.sharedInstance.consentStatus == UMPConsentStatusNotRequired;
}

@end

//setting this as app controller.
IMPL_APP_CONTROLLER_SUBCLASS( VGamesAppController )

extern "C" {
    void ConsentInfoUpdator_UpdateConsentInfo() {
        [VGamesAppController updateConsentInfo];
    }
}
