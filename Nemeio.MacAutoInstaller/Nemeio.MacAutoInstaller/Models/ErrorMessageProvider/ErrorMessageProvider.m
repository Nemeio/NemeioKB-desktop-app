//
//  ErrorMessageProvider.m
//  Nemeio.MacAutoInstaller
//
//  Created by Kevin Sibue on 11/02/2020.
//  Copyright © 2020 Kevin Sibue. All rights reserved.
//

#import "ErrorMessageProvider.h"

@implementation ErrorMessageProvider

const NSString* DEFAULT_ERROR_MESSAGE = @"An error occured. Please try again later.";

- (instancetype)init {
    _errorMessages = [NSMutableDictionary dictionaryWithObjectsAndKeys:
      @"No keyboard found.", [NSNumber numberWithInt:MAC_AUTO_INSTALLER_KEYBOARD_NOT_FOUND],
      @"An Internet connection is required to download Nemeio. Please try again later.", [NSNumber numberWithInt:MAC_AUTO_INSTALLER_INTERNET_NOT_AVAILABLE],
      @"No response from the server within the allotted time. Please try again later.", [NSNumber numberWithInt:MAC_AUTO_INSTALLER_SERVER_TIMEOUT],
      @"Download failed.", [NSNumber numberWithInt:MAC_AUTO_INSTALLER_DOWNLOAD_FAILED],
      @"You have stopped the installer.", [NSNumber numberWithInt:MAC_AUTO_INSTALLER_DOWNLOAD_CANCEL_BY_USER],
      @"Downloaded file is corrupted. Please try again.", [NSNumber numberWithInt:MAC_AUTO_INSTALLER_INVALID_CHECKSUM],
      @"Can't request installer server. Please try again later.", [NSNumber numberWithInt:MAC_AUTO_INSTALLER_GET_DOWNLOAD_INFO_FAILED],
      @"Unauthorized to write on file system.", [NSNumber numberWithInt:MAC_AUTO_INSTALLER_FILE_SYSTEM_WRITE_DENIED],
      @"No installer found. Please contact support.", [NSNumber numberWithInt:MAC_AUTO_INSTALLER_UPDATE_NOT_FOUND],
    nil];
    return self;
}

- (NSString*)getErrorMessage: (ErrorCode)code {
    NSNumber *codeNumber = [NSNumber numberWithInt: code];
    NSString *message = [_errorMessages objectForKey: codeNumber];
    if (!message) {
        return DEFAULT_ERROR_MESSAGE;
    }
    return message;
}

@end
