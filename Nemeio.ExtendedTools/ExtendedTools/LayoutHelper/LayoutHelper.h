//
//  LayoutHelper.h
//  ExtendedTools
//
//  Created by Kevin Sibue on 17/06/2020.
//  Copyright © 2020 Nemeio. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreFoundation/CoreFoundation.h>
#import <Carbon/Carbon.h>

NS_ASSUME_NONNULL_BEGIN

@interface LayoutHelper : NSObject

+ (BOOL) setCurrentKeyboardLayout:(NSString*) layoutName;
+ (NSString*) createStringForKeyWithModifiers: (CGKeyCode)keyCode withLayout: (NSString*) layoutName withShift: (BOOL)shift andWithAltGr: (BOOL)altGr andCapslock: (BOOL)capslock;
+ (NSString*) createStringForKey: (CGKeyCode)keyCode;
+ (CGKeyCode) keyCodeForChar: (const UniChar)c;

@end

NS_ASSUME_NONNULL_END
