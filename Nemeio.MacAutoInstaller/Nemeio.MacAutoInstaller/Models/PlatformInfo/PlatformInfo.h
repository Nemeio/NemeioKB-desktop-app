//
//  PlatformInfo.h
//  Nemeio.MacAutoInstaller
//
//  Created by Witekio on 15/10/2019.
//  Copyright © 2019 Witekio. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface PlatformInfo : NSObject

@property (strong, nonatomic) NSMutableArray *softwares;

- (instancetype) initWithInfos: (NSDictionary*)infos;

@end
