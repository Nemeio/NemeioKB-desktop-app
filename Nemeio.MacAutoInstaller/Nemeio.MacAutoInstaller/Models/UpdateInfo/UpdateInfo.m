//
//  UpdateInfo.m
//  Nemeio.MacAutoInstaller
//
//  Created by Witekio on 15/10/2019.
//  Copyright © 2019 Witekio. All rights reserved.
//

#import "UpdateInfo.h"

@implementation UpdateInfo

- (instancetype) initWithJsonString: (NSData *)json {
    self = [super init];
    if (self) {
        NSError *error = nil;
        NSDictionary *responseObj = [NSJSONSerialization
                                     JSONObjectWithData:json
                                     options:0
                                     error:&error];
        _osx = [[PlatformInfo alloc] initWithInfos:responseObj[@"osx"]];
    }
    return self;
}

@end
