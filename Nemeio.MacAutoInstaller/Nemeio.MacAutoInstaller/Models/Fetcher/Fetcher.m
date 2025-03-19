//
//  Fetcher.m
//  Nemeio.MacAutoInstaller
//
//  Created by Witekio on 11/10/2019.
//  Copyright © 2019 Witekio. All rights reserved.
//

#import "Fetcher.h"
#import "UpdateInfo.h"
#import "SoftwareInfo.h"
#import "NSData+MD5.h"
#import <CommonCrypto/CommonDigest.h>

@interface Fetcher ()

@end

@implementation Fetcher

typedef void(^onDownloadedCompletionBlock)(UpdateInfo*);

const NSString *SERVER_URL      = @"https://karmeliet.witekio.com/api/updateInformation";
const NSTimeInterval TIMEOUT    = 30; //  In seconds

- (void)start {
    NSLog(@"Fetcher.start");
    [self downloadJSON:^(UpdateInfo * updateInfo) {
        dispatch_async(dispatch_get_main_queue(), ^{
            //  On OSX we only manage x64
            NSLog(@"JSON return a value, download success");
            self->_currentSoftware = updateInfo.osx.softwares.firstObject;
            if (self->_currentSoftware == nil) {
                NSLog(@"No OSX update found");
                [self->delegate onDownloadFinished:MAC_AUTO_INSTALLER_UPDATE_NOT_FOUND path:nil];
            }
            [self downloadInstaller: self->_currentSoftware.url];
            [self->delegate onDownloadStarted:self->_currentSoftware.version];
        });
    }];
}

- (void)downloadJSON: (onDownloadedCompletionBlock)onDownloaded {
    NSLog(@"Download JSON on server");
    NSString *stringURL = [NSString stringWithFormat:@"%@?keyboardId=%s", SERVER_URL, [_keyboardId UTF8String]];
    NSURL  *url = [NSURL URLWithString:stringURL];
    NSURLSessionTask *downloadTask = [[NSURLSession sharedSession] dataTaskWithURL: url completionHandler:^(NSData * _Nullable data, NSURLResponse * _Nullable response, NSError * _Nullable error) {
        if (error) {
            if ([self isInternetLossError:error]) {
                NSLog(@"Internet connection lost");
                [self->delegate onDownloadFinished:MAC_AUTO_INSTALLER_INTERNET_NOT_AVAILABLE path:nil];
            } else if ([self isTimeoutError:error]) {
                NSLog(@"No internet connection");
                [self->delegate onDownloadFinished:MAC_AUTO_INSTALLER_SERVER_TIMEOUT path:nil];
            } else {
                NSLog(@"JSON return is NULL, download failed!");
                [self->delegate onDownloadFinished:MAC_AUTO_INSTALLER_GET_DOWNLOAD_INFO_FAILED path:nil];
            }
        } else {
            UpdateInfo* infos = [[UpdateInfo alloc] initWithJsonString:data];
            onDownloaded(infos);
        }
    }];
    [downloadTask resume];
}

- (void)setKeyboardId:(NSString* _Nonnull)keyboardId {
    _keyboardId = keyboardId;
}

- (void)downloadInstaller:(NSURL*)installerUrl {
    _urlRequest = [NSURLRequest requestWithURL:installerUrl cachePolicy:NSURLRequestReloadIgnoringLocalCacheData timeoutInterval:TIMEOUT];
    _receivedData = [[NSMutableData alloc] initWithLength:0];
    _urlConnection = [[NSURLConnection alloc] initWithRequest:_urlRequest delegate:self startImmediately:NO];
    [_urlConnection start];
}

- (void)abort {
    NSLog(@"Fetcher.abort");
    [_urlConnection cancel];
    [delegate onDownloadFinished:MAC_AUTO_INSTALLER_DOWNLOAD_CANCEL_BY_USER path:nil];
}

- (void)setDelegate:(id)aDelegate {
    delegate = aDelegate;
}

- (void)connection:(NSURLConnection *)connection didReceiveResponse:(NSURLResponse *)response {
    _expectedDataSize = [response expectedContentLength];
}

- (void)connection:(NSURLConnection *)connection didReceiveData:(NSData *)data {
     [_receivedData appendData:data];
    float progressive = (float)[_receivedData length] / (float)_expectedDataSize;
    [delegate onDownloadProgress:progressive];
}

- (void)connectionDidFinishLoading:(NSURLConnection *)connection {
    
    ErrorCode resultCode = MAC_AUTO_INSTALLER_SUCCESS;
    NSString *path = @"";
    
    BOOL isSuccess = _receivedData.length == _expectedDataSize;
    if (isSuccess) {
        
        NSString *prefix = @"file://";
        NSUInteger prefixLength = [prefix length];
        NSString *url = [[[[NSFileManager defaultManager] homeDirectoryForCurrentUser] absoluteString] stringByAppendingString:@"Downloads/nemeio.dmg"];
        path = [url substringWithRange:NSMakeRange(prefixLength, [url length] - prefixLength)];
        
        BOOL writeSucceed = [[NSFileManager defaultManager] createFileAtPath:path contents:_receivedData attributes:nil];
        if (!writeSucceed) {
            NSLog(@"Write file on filesystem failed!");
            resultCode = MAC_AUTO_INSTALLER_FILE_SYSTEM_WRITE_DENIED;
        } else {
            NSLog(@"Verify checksum");
            NSString* receivedDataHash = [_receivedData MD5];
            BOOL checksumValid = [[_currentSoftware.checksum uppercaseString] isEqualToString: [receivedDataHash uppercaseString]];
            if (!checksumValid) {
                resultCode = MAC_AUTO_INSTALLER_INVALID_CHECKSUM;
            }
            NSLog(@"Checksum is valid ? <%hhd>", isSuccess);
        }
        
    }
    
    [delegate onDownloadFinished:resultCode path:path];
    
}

- (void)connection:(NSURLConnection *)connection didFailWithError:(NSError *)error {
    if ([self isInternetLossError:error]) {
        [delegate onDownloadFinished:MAC_AUTO_INSTALLER_INTERNET_NOT_AVAILABLE path:nil];
        return;
    } else if ([self isTimeoutError:error]) {
        [delegate onDownloadFinished:MAC_AUTO_INSTALLER_SERVER_TIMEOUT path:nil];
        return;
    }
    [delegate onDownloadFinished:MAC_AUTO_INSTALLER_DOWNLOAD_FAILED path:nil];
}

- (BOOL)isInternetLossError:(NSError*)error {
    return error.code == NSURLErrorNetworkConnectionLost || error.code == NSURLErrorNotConnectedToInternet;
}

- (BOOL)isTimeoutError:(NSError*)error {
    return error.code == NSURLErrorTimedOut;
}

@end
