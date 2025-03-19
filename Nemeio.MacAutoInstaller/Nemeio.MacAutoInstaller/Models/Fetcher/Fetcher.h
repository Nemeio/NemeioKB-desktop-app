//
//  Fetcher.h
//  Nemeio.MacAutoInstaller
//
//  Created by Witekio on 11/10/2019.
//  Copyright © 2019 Witekio. All rights reserved.
//

#import "ErrorMessageProvider.h"
#import "SoftwareInfo.h"
#import <Foundation/Foundation.h>

@protocol FetcherDelegate
- (void)onDownloadStarted:(NSString*)version;
- (void)onDownloadProgress:(float)progress;
- (void)onDownloadFinished:(ErrorCode)withCode path: (NSString * _Nullable)path;
@end

@interface Fetcher : NSObject <NSURLConnectionDataDelegate> {
    id delegate;
}
@property long expectedDataSize;
@property (strong) NSMutableData * _Nullable receivedData;
@property (strong) NSURLRequest * _Nullable urlRequest;
@property (strong) NSURLConnection * _Nullable urlConnection;
@property (strong) SoftwareInfo * _Nullable currentSoftware;
@property (strong, nonatomic) NSString * _Nullable keyboardId;
- (void)setDelegate:(id _Nonnull )delegate;
- (void)setKeyboardId:(NSString* _Nonnull)keyboardId;
- (void)start;
- (void)abort;
@end
