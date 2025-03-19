//
//  SoftwareInfo.h
//  Nemeio.MacAutoInstaller
//
//  Created by Witekio on 15/10/2019.
//  Copyright © 2019 Witekio. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface SoftwareInfo : NSObject

@property (strong, nonatomic) NSString *platform;
@property (strong, nonatomic) NSString *version;
@property (strong, nonatomic) NSURL *url;
@property (strong, nonatomic) NSString *checksum;

- (instancetype) initWithInfos: (NSString *)platform andVersion: (NSString *)version andUrl: (NSURL *)url andChecksum: (NSString*)checksum;

@end
