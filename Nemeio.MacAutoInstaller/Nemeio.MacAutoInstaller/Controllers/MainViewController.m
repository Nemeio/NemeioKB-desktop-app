//
//  MainViewController.m
//  Nemeio.MacAutoInstaller
//
//  Created by Witekio on 11/10/2019.
//  Copyright © 2019 Witekio. All rights reserved.
//

#import "Fetcher.h"
#import "MainViewController.h"

@implementation MainViewController

const CGFloat DefaultControlWidth       = 32;
const CGFloat DefaultControlHeight      = 32;
const CGFloat DefaultWindowWidth        = 512;
const CGFloat DefaultWindowHeight       = 128;

NSString *const DownloadText = @"Downloading";

- (void)loadView {
    self.view = [[NSView alloc] initWithFrame:NSMakeRect(0, 0, DefaultWindowWidth, DefaultWindowHeight)];
}

//  MARK : Lifecycle methods

- (void)viewDidLoad {
    
    [super viewDidLoad];
    [self createViewContent];
    [self applyConstraints];
    
    _currentVersion = @"";
    _errorMessageProvider = [[ErrorMessageProvider alloc] init];
    _fetcher = [[Fetcher alloc] init];
    [_fetcher setDelegate:self];
    
}

//  MARK : Methods

- (void)startDownload:(NSString*)keyboardId {
    if (_fetcher != nil) {
        [_fetcher setKeyboardId:keyboardId];
        [_fetcher start];
    }
}

//  MARK : Fetcher Delegate

- (void)onDownloadStarted:(NSString*)version {
    dispatch_async(dispatch_get_main_queue(), ^{
        self.currentVersion = version;
    });
}

- (void)onDownloadFinished:(ErrorCode)withCode path:(NSString*)path {
    dispatch_async(dispatch_get_main_queue(), ^{
        if (withCode == MAC_AUTO_INSTALLER_SUCCESS) {
            //  Workaround to show file instead of launch it
            //  unless DMG is not code signed
            //  real code : [[NSWorkspace sharedWorkspace] openFile:path];
            NSLog(@"Download finished with success");
            NSLog(@"Open file at path : %@", path);
            [[NSWorkspace sharedWorkspace] selectFile:path inFileViewerRootedAtPath:nil];
        } else {
            NSString *errorMessage = [self->_errorMessageProvider getErrorMessage: withCode];
            NSLog(@"[0x%02x] %@", withCode, errorMessage);
            [self showAlert: errorMessage];
        }
        [NSApp terminate:self];
    });
}

- (void)onDownloadProgress:(float)progress {
    dispatch_async(dispatch_get_main_queue(), ^{
        int progressPercent = (int)(progress*100);
        [self.headerText setStringValue:[NSString stringWithFormat:@"%@ %@ ... (%d%%)",DownloadText, _currentVersion, progressPercent]];
        [self.progressIndicator setDoubleValue:progress];
    });
}

//  MARK : Utils methods

- (void)showAlert:(NSString*)title {
    NSAlert *alert = [NSAlert new];
    [alert setMessageText:title];
    [alert addButtonWithTitle:@"Ok"];
    [alert runModal];
}

- (void) createViewContent {
    
    NSLog(@"MainViewController.createViewContent");
    
    _bodyView = [[NSView alloc] initWithFrame:NSMakeRect(0, 0, DefaultControlWidth, DefaultControlHeight)];
    _bodyView.wantsLayer = YES;
    
    _headerText = [[NSTextField alloc] initWithFrame:NSMakeRect(0, 0, DefaultControlWidth, DefaultControlHeight)];
    _headerText.wantsLayer = YES;
    [_headerText setStringValue:@"Downloading ..."];
    [_headerText setBordered:NO];
    [_headerText setBezelStyle:NSTextFieldRoundedBezel];
    [_headerText setSelectable:NO];
    [_headerText.layer setBackgroundColor:[NSColor clearColor].CGColor];
    
    _progressIndicator = [[NSProgressIndicator alloc]  initWithFrame:NSMakeRect(0, 0, DefaultControlWidth, DefaultControlHeight)];
    [_progressIndicator setMinValue:0];
    [_progressIndicator setMaxValue:1];
    [_progressIndicator setIndeterminate:NO];
    
    _cancelButton = [[NSButton alloc]  initWithFrame:NSMakeRect(0, 0, DefaultControlWidth, DefaultControlHeight)];
    [_cancelButton setTitle:@"Cancel"];
    [_cancelButton setBordered:YES];
    [_cancelButton setBezelStyle:NSBezelStyleRoundRect];
    [_cancelButton setButtonType:NSButtonTypeMomentaryPushIn];
    [_cancelButton setTarget:self];
    [_cancelButton setAction:@selector(cancelPressed)];
    
    [_bodyView addSubview:_headerText];
    [_bodyView addSubview:_progressIndicator];
    [_bodyView addSubview:_cancelButton];
    
    [self.view addSubview:_bodyView];
    
}

- (void)applyConstraints {
    
    NSLog(@"MainViewController.applyConstraints");
    
    _views = [NSMutableDictionary dictionary];
    [_views setObject: _bodyView  forKey: @"bodyView"];
    [_views setObject: _headerText  forKey: @"headerText"];
    [_views setObject: _progressIndicator  forKey: @"progressIndicator"];
    [_views setObject: _cancelButton  forKey: @"cancelButton"];
    
    _bodyView.translatesAutoresizingMaskIntoConstraints = NO;
    _headerText.translatesAutoresizingMaskIntoConstraints = NO;
    _progressIndicator.translatesAutoresizingMaskIntoConstraints = NO;
    _cancelButton.translatesAutoresizingMaskIntoConstraints = NO;
    
    NSMutableDictionary *views = [NSMutableDictionary dictionary];
    [views setObject: _bodyView  forKey: @"bodyView"];
    
    [self addConstraint:@"H:|[bodyView]|" toView:self.view];
    [self addConstraint:@"V:|[bodyView]|" toView:self.view];
    [self addConstraint:@"H:|-16-[headerText]-16-|" toView:_bodyView];
    [self addConstraint:@"V:|-16-[headerText]" toView:_bodyView];
    [self addConstraint:@"H:|-16-[progressIndicator]-16-|" toView:_bodyView];
    [self addConstraint:@"V:[headerText]-8-[progressIndicator]" toView:_bodyView];
    [self addConstraint:@"H:[cancelButton]-16-|" toView:_bodyView];
    [self addConstraint:@"V:[cancelButton]-16-|" toView:_bodyView];
    
    [self.view setNeedsLayout:YES];
    [self.view layoutSubtreeIfNeeded];
    
}

- (void)addConstraint:(NSString*)visualFormat toView :(NSView*)view {
    
    NSArray<NSLayoutConstraint *> *newConstraint = [NSLayoutConstraint
                                                    constraintsWithVisualFormat:visualFormat
                                                    options:NSLayoutFormatAlignAllLeading
                                                    metrics:nil
                                                    views: _views];
    
    [view addConstraints:newConstraint];
    
}

//  MARK : Actions

- (void)cancelPressed {
    NSLog(@"Cancel pressed");
    [_fetcher abort];
    [NSApp terminate:self];
}

@end

