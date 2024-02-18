//
//  Header.h
//  Unity-iPhone
//
//  Created by Admin on 28/08/2022.
//

#pragma once

#import "NativeViewController.h"

@interface AdmobAdsHelper : NSObject

+ (void) init;
+ (void) setViewController:(UIViewController*) rootView;
+ (void) tryToPresentAd:(UIViewController*) rootView;
+ (void) callUnityObject:(const char*)method Parameter:(const char*)parameter;

@end
