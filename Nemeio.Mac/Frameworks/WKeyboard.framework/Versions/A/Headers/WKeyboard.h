//
//  WKeyboard.h
//  WKeyboard
//
//  Created by Kévin Sibué on 29/10/2018.
//  Copyright © 2018 Witekio. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#include <CoreFoundation/CoreFoundation.h>
#include <Carbon/Carbon.h> /* For kVK_ constants, and TIS functions. */

//! Project version number for WKeyboard.
FOUNDATION_EXPORT double WKeyboardVersionNumber;

//! Project version string for WKeyboard.
FOUNDATION_EXPORT const unsigned char WKeyboardVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <WKeyboard/PublicHeader.h>

@interface WKeyboard : NSObject
CFStringRef createStringForKey(CGKeyCode keyCode);
CGKeyCode keyCodeForChar(const char c);
@end

