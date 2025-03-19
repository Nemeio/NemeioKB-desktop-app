//
//  LayoutHelper.m
//  ExtendedTools
//
//  Created by Kevin Sibue on 17/06/2020.
//  Copyright © 2020 Nemeio. All rights reserved.
//

#import "LayoutHelper.h"

@implementation LayoutHelper

static int const maxStringLength        = 4;
static NSString* const abcLayoutId      = @"com.apple.keylayout.ABC";

+ (BOOL) setCurrentKeyboardLayout:(NSString*) layoutName{
    
    NSLog(@"setCurrentKeyboardLayout called with layoutName : <%@>", layoutName);
    
    const void *key = (const void *)kTISPropertyInputSourceID;
    const void *value = (__bridge_retained CFStringRef)layoutName;
    
    CFDictionaryRef propertiesDictionary = CFDictionaryCreate(
                                                              kCFAllocatorDefault,
                                                              &key, &value, 1,
                                                              &kCFTypeDictionaryKeyCallBacks,
                                                              &kCFTypeDictionaryValueCallBacks
                                                              );
    
    if (propertiesDictionary != NULL) {
        CFArrayRef inputSources = TISCreateInputSourceList(propertiesDictionary, TRUE);
        if (inputSources != NULL && CFArrayGetCount(inputSources) > 0) {
            TISInputSourceRef inputSourceRef = (TISInputSourceRef)CFArrayGetValueAtIndex(inputSources, 0);
            if (inputSourceRef != NULL) {
                TISSelectInputSource(inputSourceRef);
                CFRelease(inputSources);
                CFRelease(propertiesDictionary);
                CFRelease(value);
                return YES;
            }
            CFRelease(inputSources);
        }
        CFRelease(propertiesDictionary);
    }
    CFRelease(value);
    
    return NO;
    
}

+ (NSString*) createStringForKeyWithModifiers: (CGKeyCode)keyCode withLayout: (NSString*) layoutName withShift: (BOOL)shift andWithAltGr: (BOOL)altGr andCapslock: (BOOL)capslock {
    
    int mac_modifiers = 0;
    
    if (shift)
        mac_modifiers |= shiftKey;
    
    if (altGr)
        mac_modifiers |= rightOptionKey;
    
    if (capslock)
        mac_modifiers |= alphaLock;
    
    UInt32 modifier_key_state = (mac_modifiers >> 8) & 0xFF;
    
    const void *key = (const void *)kTISPropertyInputSourceID;
    const void *value = (__bridge_retained CFStringRef)layoutName;
    
    CFDictionaryRef propertiesDictionary = CFDictionaryCreate(
                                                              kCFAllocatorDefault,
                                                              &key, &value, 1,
                                                              &kCFTypeDictionaryKeyCallBacks,
                                                              &kCFTypeDictionaryValueCallBacks
                                                              );
    
    if (propertiesDictionary != NULL) {
        
        CFArrayRef inputSources = TISCreateInputSourceList(propertiesDictionary, TRUE);
        
        if (inputSources != NULL && CFArrayGetCount(inputSources) > 0) {
            
            TISInputSourceRef inputSourceRef = (TISInputSourceRef)CFArrayGetValueAtIndex(inputSources, 0);
            
            if (inputSourceRef != NULL) {
                
                CFDataRef ref = TISGetInputSourceProperty(inputSourceRef, kTISPropertyUnicodeKeyLayoutData);
                
                if (!ref) {
                    //  In some cases like Japanese or Korean, TISGetInputSourceProperty return NULL
                    //  We automatically return ABC content
                    return [self createStringForKeyWithModifiers:keyCode withLayout:abcLayoutId withShift:shift andWithAltGr:altGr andCapslock:capslock];
                }
                
                const UCKeyboardLayout *keyboardLayout = (const UCKeyboardLayout *)CFDataGetBytePtr(ref);
                
                UInt32 keysDown = 0;
                UniChar chars[4];
                UniCharCount realLength;
                
                UCKeyTranslate(keyboardLayout,
                               keyCode,
                               kUCKeyActionDisplay,
                               modifier_key_state,
                               LMGetKbdType(),
                               kUCKeyTranslateNoDeadKeysBit,
                               &keysDown,
                               sizeof(chars) / sizeof(chars[0]),
                               &realLength,
                               chars);
                
                CFRelease(inputSources);
                CFRelease(propertiesDictionary);
                CFRelease(value);
                
                return (__bridge NSString *) CFStringCreateWithCharacters(kCFAllocatorDefault, chars, 1);

            }
        }
        
        CFRelease(inputSources);
        CFRelease(propertiesDictionary);
        
    }
    
    CFRelease(value);
    
    return @"";
    
}

+ (NSString*) createStringForKey: (CGKeyCode)keyCode {
    
    TISInputSourceRef currentKeyboard = TISCopyCurrentKeyboardInputSource();
    CFDataRef layoutData =
    TISGetInputSourceProperty(currentKeyboard,
                              kTISPropertyUnicodeKeyLayoutData);
    const UCKeyboardLayout *keyboardLayout =
    (const UCKeyboardLayout *)CFDataGetBytePtr(layoutData);
    
    UInt32 keysDown = 0;
    UniChar chars[maxStringLength];
    UniCharCount realLength;
    
    UCKeyTranslate(keyboardLayout,
                   keyCode,
                   kUCKeyActionDisplay,
                   0,
                   LMGetKbdType(),
                   kUCKeyTranslateNoDeadKeysBit,
                   &keysDown,
                   sizeof(chars) / sizeof(chars[0]),
                   &realLength,
                   chars);
    
    
    CFRelease(currentKeyboard);
    
    return [[NSString alloc] initWithBytes:chars length:realLength encoding:NSUTF8StringEncoding];
    
}

+ (CGKeyCode) keyCodeForChar: (const UniChar)character {
    
    CGKeyCode code = 0;
    CFStringRef charStr = CFStringCreateWithCharacters(kCFAllocatorDefault, &character, 1);
    if (charStr)
    {
        for (int i = 0; code == 0 && i <= CHAR_MAX; ++i) {
            CFStringRef string = (__bridge_retained CFStringRef) [LayoutHelper createStringForKey:(CGKeyCode)i];
            if (string != NULL) {
                if (CFStringCompare(string, charStr, kCFCompareCaseInsensitive) == kCFCompareEqualTo)
                {
                    code = i;
                }
                CFRelease(string);
            }
        }
        CFRelease(charStr);
    }
    return code;
    
}

@end
