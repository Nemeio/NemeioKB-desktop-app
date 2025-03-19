//
//  PlatformInfo.m
//  Nemeio.MacAutoInstaller
//
//  Created by Witekio on 15/10/2019.
//  Copyright © 2019 Witekio. All rights reserved.
//

#import "PlatformInfo.h"
#import "SoftwareInfo.h"

@implementation PlatformInfo

- (instancetype) initWithInfos: (NSDictionary*)infos {
    self = [super init];
    if (self) {
        _softwares = [[NSMutableArray alloc] init];
        for (NSString *key in infos) {
            NSDictionary *value = infos[key][0];
            SoftwareInfo *newSoftwareInfo = [[SoftwareInfo alloc]
                                             initWithInfos:value[@"platform"]
                                             andVersion:value[@"version"]
                                             andUrl:[[NSURL alloc] initWithString:value[@"url"]]
                                             andChecksum:value[@"checksum"]];
            [_softwares addObject: newSoftwareInfo];
        }
    }
    return self;
}

@end
