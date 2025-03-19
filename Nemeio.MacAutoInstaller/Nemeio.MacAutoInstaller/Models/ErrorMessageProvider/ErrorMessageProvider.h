//
//  ErrorMessageProvider.h
//  Nemeio.MacAutoInstaller
//
//  Created by Kevin Sibue on 11/02/2020.
//  Copyright © 2020 Kevin Sibue. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

typedef enum
{
    //  Common
    MAC_AUTO_INSTALLER_SUCCESS                          = 0x05000000,
    
    //  Keyboard Errors
    MAC_AUTO_INSTALLER_KEYBOARD_NOT_FOUND               = 0x05000101,
    
    //  Connectivity Errors
    MAC_AUTO_INSTALLER_INTERNET_NOT_AVAILABLE           = 0x05000201,
    MAC_AUTO_INSTALLER_SERVER_TIMEOUT                   = 0x05000202,
    
    //  Download Errors
    MAC_AUTO_INSTALLER_DOWNLOAD_FAILED                  = 0x05000301,
    MAC_AUTO_INSTALLER_DOWNLOAD_CANCEL_BY_USER          = 0x05000302,
    MAC_AUTO_INSTALLER_INVALID_CHECKSUM                 = 0x05000303,
    MAC_AUTO_INSTALLER_GET_DOWNLOAD_INFO_FAILED         = 0x05000304,
    MAC_AUTO_INSTALLER_UPDATE_NOT_FOUND                 = 0x05000305,
    
    //  File System Errors
    MAC_AUTO_INSTALLER_FILE_SYSTEM_WRITE_DENIED         = 0x05000401,
} ErrorCode;

@interface ErrorMessageProvider : NSObject

@property (strong, nonatomic) NSDictionary *errorMessages;

- (instancetype)init;
- (NSString*)getErrorMessage: (ErrorCode)code;

@end

NS_ASSUME_NONNULL_END
