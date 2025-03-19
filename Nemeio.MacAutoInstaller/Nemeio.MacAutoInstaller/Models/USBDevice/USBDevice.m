//
//  USBDevice.m
//  Nemeio.MacAutoInstaller
//
//  Created by Kevin Sibue on 17/01/2020.
//  Copyright © 2020 Kevin Sibue. All rights reserved.
//

#import "USBDevice.h"

@implementation USBDevice

- (instancetype) initWithInfos: (NSString *)path andSerialNumber: (NSString *)serialNumber {
    _path = path;
    _serialNumber = serialNumber;
    return self;
}

@end
