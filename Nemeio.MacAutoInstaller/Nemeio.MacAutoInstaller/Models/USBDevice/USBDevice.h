//
//  USBDevice.h
//  Nemeio.MacAutoInstaller
//
//  Created by Kevin Sibue on 17/01/2020.
//  Copyright © 2020 Kevin Sibue. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface USBDevice : NSObject

@property (strong, nonatomic) NSString *path;
@property (strong, nonatomic) NSString *serialNumber;

- (instancetype) initWithInfos: (NSString *)path andSerialNumber: (NSString *)serialNumber;

@end

NS_ASSUME_NONNULL_END
