//
//  LoginHelper.m
//  ExtendedTools
//
//  Created by Kevin Sibue on 17/06/2020.
//  Copyright © 2020 Nemeio. All rights reserved.
//

#import "LoginHelper.h"

@implementation LoginHelper

+ (void)registerAppAtLogin {
    
    NSURL *applicationPathURL;
    NSString *applicationPath = [NSBundle mainBundle].bundlePath;
    
    if (applicationPath) {
        
        applicationPathURL = [NSURL fileURLWithPath: applicationPath];
        LSSharedFileListRef sharedFileList = LSSharedFileListCreate(NULL, kLSSharedFileListSessionLoginItems, NULL);
        
        if (sharedFileList) {
            LSSharedFileListItemRef sharedFileListItem = LSSharedFileListInsertItemURL(sharedFileList, kLSSharedFileListItemLast, NULL, NULL, (__bridge CFURLRef)applicationPathURL, NULL, NULL);
            if (sharedFileListItem) {
                CFRelease(sharedFileListItem);
            }
            CFRelease(sharedFileList);
        } else {
            NSLog(@"Unable to create the shared file list.");
        }
    }
    
}

+ (void)unregisterAppAtLogin {
    
    LSSharedFileListRef sharedFileList = LSSharedFileListCreate(NULL, kLSSharedFileListSessionLoginItems, NULL);
    NSString *applicationPath = [NSBundle mainBundle].bundlePath;
    
    if (sharedFileList && applicationPath) {
        
        NSArray *sharedFileListArray = nil;
        UInt32 seedValue;
        
        sharedFileListArray = CFBridgingRelease(LSSharedFileListCopySnapshot(sharedFileList, &seedValue));
        
        for (id sharedFile in sharedFileListArray) {
            
            LSSharedFileListItemRef sharedFileListItem = (__bridge LSSharedFileListItemRef)sharedFile;
            CFURLRef applicationPathURL = NULL;
            applicationPathURL = LSSharedFileListItemCopyResolvedURL(sharedFileListItem, 0, NULL);
            
            if (applicationPathURL != NULL) {
                NSString *resolvedApplicationPath = [(__bridge NSURL *)applicationPathURL path];
                if ([resolvedApplicationPath compare: applicationPath] == NSOrderedSame) {
                    LSSharedFileListItemRemove(sharedFileList, sharedFileListItem);
                }
                CFRelease(applicationPathURL);
            }
        }
        
        CFRelease(sharedFileList);
        
    } else {
        NSLog(@"Unable to create the shared file list.");
    }
    
}

@end
