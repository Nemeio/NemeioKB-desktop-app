//
//  SoftwareInfo.m
//  Nemeio.MacAutoInstaller
//
//  Created by Witekio on 15/10/2019.
//  Copyright © 2019 Witekio. All rights reserved.
//

#import "SoftwareInfo.h"

@implementation SoftwareInfo

- (instancetype) initWithInfos: (NSString *)platform andVersion: (NSString *)version andUrl: (NSURL *)url andChecksum: (NSString*)checksum {
    self = [super init];
    if (self) {
        self.platform = platform;
        self.version = version;
        self.url = url;
        self.checksum = checksum;
    }
    return self;
}

@end
