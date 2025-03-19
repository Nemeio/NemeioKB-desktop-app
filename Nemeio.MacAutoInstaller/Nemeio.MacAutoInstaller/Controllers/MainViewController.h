//
//  MainViewController.h
//  Nemeio.MacAutoInstaller
//
//  Created by Witekio on 11/10/2019.
//  Copyright © 2019 Witekio. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import "Fetcher.h"

@interface MainViewController : NSViewController
@property (strong) ErrorMessageProvider *errorMessageProvider;
@property (strong) Fetcher *fetcher;
@property (strong) NSView *bodyView;
@property (strong) NSTextField *headerText;
@property (strong) NSProgressIndicator *progressIndicator;
@property (strong) NSButton *cancelButton;
@property (strong) NSMutableDictionary *views;
@property (strong) NSString *currentVersion;
- (void)startDownload:(NSString*)keyboardId;
@end
