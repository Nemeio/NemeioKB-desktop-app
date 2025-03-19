//
//  AppDelegate.m
//  Nemeio.MacAutoInstaller
//
//  Created by Witekio on 11/10/2019.
//  Copyright © 2019 Witekio. All rights reserved.
//

#import <AppKit/AppKit.h>
#import <IOKit/IOKitLib.h>
#import <IOKit/usb/IOUSBLib.h>
#import <IOKit/serial/IOSerialKeys.h>

#import "AppDelegate.h"
#import "MainViewController.h"
#import "USBDevice.h"

@interface AppDelegate ()

@property (strong) NSWindow *window;
@property (strong) MainViewController *mainViewController;

- (void)setupWindow;
- (void)startApp;
- (BOOL)showYesNoQuestion:(NSString*)message;
- (void)showMessage:(NSString*)message;

@end

@implementation AppDelegate

- (void)applicationDidFinishLaunching:(NSNotification *)aNotification {
    [self setupWindow];
    [self startApp];
}

- (void)applicationWillTerminate:(NSNotification *)aNotification {
    // Insert code here to tear down your application
}

- (void)setupWindow {
    
    NSLog(@"AppDelegate.setupWindow");
    
    _mainViewController = [[MainViewController alloc] init];
    
    NSRect frame = NSMakeRect(0, 0, 600, 200);
    NSWindowStyleMask windowStyle = NSWindowStyleMaskClosable | NSWindowStyleMaskTitled;
    
    NSView *mainView = _mainViewController.view;
    
    NSMutableDictionary *views = [NSMutableDictionary dictionary];
    [views setObject: mainView  forKey: @"mainView"];
    
    _window = [[NSWindow alloc] initWithContentRect:frame styleMask:windowStyle backing:NSBackingStoreBuffered defer:NO];
    [_window setTitle:@"Nemeio"];
    [_window.contentView addSubview:mainView];
    
    NSArray<NSLayoutConstraint *> *horizontalMainViewConstraint = [NSLayoutConstraint
                                                                   constraintsWithVisualFormat:@"H:|[mainView]|"
                                                                   options:NSLayoutFormatAlignAllLeading
                                                                   metrics:nil
                                                                   views:views];
    NSArray<NSLayoutConstraint *> *verticalMainViewConstraint = [NSLayoutConstraint
                                                                 constraintsWithVisualFormat:@"V:|[mainView]|"
                                                                 options:NSLayoutFormatAlignAllLeading
                                                                 metrics:nil
                                                                 views:views];
    [_window.contentView addConstraints:horizontalMainViewConstraint];
    [_window.contentView addConstraints:verticalMainViewConstraint];
    
    [_window.contentView setNeedsLayout:YES];
    [_window.contentView layoutSubtreeIfNeeded];
    [_window layoutIfNeeded];
    
}

- (void)startApp {
    
    NSLog(@"Application start ...");
    
    NSString *appParentDirectory = [[[NSBundle mainBundle] bundlePath] stringByDeletingLastPathComponent];
    NSLog(@"Running folder : %s", [appParentDirectory UTF8String]);
    
    NSString *currentSerialNumber = [self getExecutingDriveSerialNumber:appParentDirectory];
    if (currentSerialNumber == nil) {
        [self noKeyboardFound];
    }
    
    NSMutableSet *devices = [self getNemeioDevices];
    if ([devices count] > 0) {
        
        USBDevice *usbDevice = [self getKeyboard:currentSerialNumber path:devices];
        if (usbDevice == nil) {
            [self noKeyboardFound];
        }
        
        NSString *keyboardId = [usbDevice serialNumber];
        
        if ([keyboardId length] == 0) {
            NSString *message = @"No identifier found for keyboard!";
            NSLog(@"%@", message);
            [self showMessage:message];
            [NSApp terminate:self];
        }
        
        BOOL isYes = [self showYesNoQuestion: [NSString stringWithFormat:@"Do you want to download Nemeio for keyboard %@ ?", keyboardId]];
        if (isYes) {
            [_window center];
            [_window makeKeyAndOrderFront:NSApp];
            [_mainViewController startDownload:keyboardId];
        } else {
            NSLog(@"User refused. Stop app");
            [NSApp terminate:self];
        }
        
    } else {
        [self noKeyboardFound];
    }
    
}

- (void) noKeyboardFound {
    NSString *message = @"No Nemeio keyboard found!";
    NSLog(@"%@", message);
    [self showMessage:message];
    [NSApp terminate:self];
}

- (BOOL)showYesNoQuestion:(NSString*)message {
    
    NSAlert *alert = [NSAlert new];
    alert.messageText = message;
    [alert addButtonWithTitle:@"Yes"];
    [alert addButtonWithTitle:@"No"];
    
    return [alert runModal] == NSAlertFirstButtonReturn;
}

- (void)showMessage:(NSString*)message {
    
    NSAlert *alert = [NSAlert new];
    alert.messageText = message;
    [alert addButtonWithTitle:@"Ok"];
    
    [alert runModal];
}

- (NSString*) getExecutingDriveSerialNumber:(NSString*)executionPath {
    
    NSString *result = nil;
    NSArray *mountedRemovableMedia = [[NSFileManager defaultManager] mountedVolumeURLsIncludingResourceValuesForKeys:nil options:NSVolumeEnumerationSkipHiddenVolumes];
    
    for(NSURL *volURL in mountedRemovableMedia)
    {
        int                 err = 0;
        DADiskRef           disk;
        DASessionRef        session;
        CFDictionaryRef     descDict;
        session = DASessionCreate(NULL);
        
        if (session == NULL) {
            err = EINVAL;
        }
        
        if (err == 0) {
            disk = DADiskCreateFromVolumePath(NULL,session,(CFURLRef)volURL);
            if (session == NULL) {
                err = EINVAL;
            }
        }
        
        if (err == 0) {
            descDict = DADiskCopyDescription(disk);
            if (descDict == NULL) {
                err = EINVAL;
            }
        }
        
        if (err == 0) {
            
            io_service_t ioService = DADiskCopyIOMedia( disk );
            CFStringRef key = CFSTR("USB Serial Number");
            CFStringRef currentDriveSerialNumber = IORegistryEntrySearchCFProperty(ioService, kIOServicePlane, key, NULL,
                                                      kIORegistryIterateParents | kIORegistryIterateRecursively);
            NSString *pathPrefix = @"file://";
            NSString *pathSuffix = @"/";
            NSUInteger numberOfCharToKeep = [[volURL absoluteString] length] - ([pathPrefix length] + [pathSuffix length]);
            
            NSString *volumeUrl = [[volURL absoluteString] substringWithRange:NSMakeRange([pathPrefix length], numberOfCharToKeep)];
            
            if ([executionPath containsString: volumeUrl]) {
                result = (__bridge NSString *)(currentDriveSerialNumber);
            }
            
        }
        
        if (descDict != NULL) {
            CFRelease(descDict);
        }
        
        if (disk != NULL) {
            CFRelease(disk);
        }
        
        if (session != NULL) {
            CFRelease(session);
        }
        
    }
    
    if (result != nil) {
        NSLog(@"Current drive found <%s>", [result UTF8String]);
    } else {
        NSLog(@"Drive not found");
    }
    
    return result;
    
}

- (USBDevice*) getKeyboard:(NSString*)currentSerialNumber path:(NSMutableSet*)devices {
    NSLog(@"Search device with id <%s>", [currentSerialNumber UTF8String]);
    for (USBDevice* device in devices) {
        NSLog(@"Test for device with serial number <%s>", [[device serialNumber] UTF8String]);
        if ([[device serialNumber] isEqualToString:currentSerialNumber]) {
            NSLog(@"Device found");
            return device;
        }
    }
    NSLog(@"No device found");
    return nil;
}

- (NSMutableSet*) getNemeioDevices {
    
    NSMutableSet *devicesFound = [[NSMutableSet alloc ]init];
    
    CFMutableDictionaryRef matchingDict;
    io_iterator_t iter;
    kern_return_t kr;
    io_service_t device;
    
    matchingDict = IOServiceMatching(kIOUSBDeviceClassName);
    if (matchingDict == NULL) {
        return nil;
    }
    
    kr = IOServiceGetMatchingServices(kIOMasterPortDefault, matchingDict, &iter);
    if (kr != KERN_SUCCESS) {
        return nil;
    }
    
    while ((device = IOIteratorNext(iter))) {
        USBDevice *nemeioUsbDevice = [self isNemeioDevice:device];
        if (nemeioUsbDevice != nil) {
            [devicesFound addObject:nemeioUsbDevice];
        }
        IOObjectRelease(device);
    }
    
    IOObjectRelease(iter);
    
    return devicesFound;
    
}

- (USBDevice*) isNemeioDevice:(io_object_t)device {
    
    NSNumber *nemeioVendorId = [NSNumber numberWithUnsignedInteger:0x0483];
    NSNumber *nemeioProductId = [NSNumber numberWithUnsignedInteger:0x1234];
    
    CFMutableDictionaryRef usbProperties = 0;
    
    if (IORegistryEntryCreateCFProperties(device, &usbProperties, kCFAllocatorDefault, kNilOptions) != KERN_SUCCESS) {
        IOObjectRelease(device);
        return nil;
    }
    
    NSDictionary *properties = CFBridgingRelease(usbProperties);
    
    NSNumber *vendorID = properties[(__bridge NSString *)CFSTR(kUSBVendorID)];
    NSNumber *productID = properties[(__bridge NSString *)CFSTR(kUSBProductID)];
    NSString *serialNumber = properties[(__bridge NSString *)CFSTR(kUSBSerialNumberString)];
    NSString *devicePath = [self getDevicePath:device];
    
    IOObjectRelease(device);
    
    if ([vendorID isEqualToNumber:nemeioVendorId] && [productID isEqualToNumber:nemeioProductId]) {
        return [[USBDevice alloc] initWithInfos:devicePath andSerialNumber:serialNumber];
    }
    
    return nil;
    
}

- (NSString*) getDevicePath:(io_object_t)device {
    
    char serviceName[128];
    IORegistryEntryGetNameInPlane(device, kIOServicePlane, serviceName);
    CFStringRef dialinDeviceKey = IORegistryEntrySearchCFProperty(device, kIOServicePlane, CFSTR(kIODialinDeviceKey), nil, kIORegistryIterateRecursively);
    
    if (dialinDeviceKey == nil) {
        return nil;
    }
    
    CFStringGetCString(dialinDeviceKey, serviceName, 128, kCFStringEncodingUTF8);
    
    return (__bridge NSString *)dialinDeviceKey;
    
}

@end
