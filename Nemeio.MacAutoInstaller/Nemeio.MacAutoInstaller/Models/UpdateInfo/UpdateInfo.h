//
//  UpdateInfo.h
//  Nemeio.MacAutoInstaller
//
//  Created by Witekio on 15/10/2019.
//  Copyright © 2019 Witekio. All rights reserved.
//

#import "PlatformInfo.h"
#import <Foundation/Foundation.h>

@interface UpdateInfo : NSObject

@property (strong, nonatomic) PlatformInfo *osx;

- (instancetype) initWithJsonString: (NSData *)json;
    
@end
