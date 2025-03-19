//
//  AudioHelper.m
//  ExtendedTools
//
//  Created by Kevin Sibue on 17/06/2020.
//  Copyright © 2020 Nemeio. All rights reserved.
//

#import "AudioHelper.h"
#import <CoreAudio/CoreAudio.h>
#import <AudioToolbox/AudioServices.h>

@implementation AudioHelper

#define    VOLUME_MUTE_THRESHOLD    0.000f            //if the volume should be set under this value, the device will be muted

+ (void) setSystemVolume: (float)inVolume {
    
    float                               newValue = inVolume;
    AudioObjectPropertyAddress          theAddress;
    AudioDeviceID                       defaultDevID;
    OSStatus                            theError = noErr;
    UInt32                              muted;
    Boolean                             canSetVol = YES, muteValue;
    Boolean                             hasMute = YES, canMute = YES;
    
    defaultDevID = obtainDefaultOutputDevice();
    
    //  device not found: return without trying to set
    if (defaultDevID == kAudioObjectUnknown) {
        NSLog(@"Device unknown");
        return;
    }
    
    //  check if the new value is in the correct range - normalize it if not
    newValue = inVolume > 1.0f ? 1.0f : (inVolume < 0.0f ? 0.0f : inVolume);
    if (newValue != inVolume) {
        NSLog(@"Tentative volume (%5.2f) was out of range; reset to %5.2f", inVolume, newValue);
    }
    
    theAddress.mElement = kAudioObjectPropertyElementMaster;
    theAddress.mScope = kAudioDevicePropertyScopeOutput;
    
    //  set the selector to mute or not by checking if under threshold
    muteValue = (newValue < VOLUME_MUTE_THRESHOLD);
    
    //  check if a mute command is available
    if (muteValue)
    {
        theAddress.mSelector = kAudioDevicePropertyMute;
        hasMute = AudioObjectHasProperty(defaultDevID, &theAddress);
        if (hasMute)
        {
            theError = AudioObjectIsPropertySettable(defaultDevID, &theAddress, &canMute);
            if (theError != noErr || !canMute)
            {
                canMute = NO;
                NSLog(@"Should mute device 0x%0x but did not succeed",defaultDevID);
            }
        }
        else
        {
            canMute = NO;
        }
    }
    else
    {
        theAddress.mSelector = kAudioHardwareServiceDeviceProperty_VirtualMasterVolume;
    }
    
    // **** now manage the volume following what we found ****
    
    //  be sure the device has a volume command
    if (! AudioObjectHasProperty(defaultDevID, &theAddress))
    {
        NSLog(@"The device 0x%0x does not have a volume to set", defaultDevID);
        return;
    }
    
    //  be sure the device can set the volume
    theError = AudioObjectIsPropertySettable(defaultDevID, &theAddress, &canSetVol);
    if ( theError!=noErr || !canSetVol )
    {
        NSLog(@"The volume of device 0x%0x cannot be set", defaultDevID);
        return;
    }
    
    //  if under the threshold then mute it, only if possible - done/exit
    if (muteValue && hasMute && canMute)
    {
        muted = 1;
        theError = AudioObjectSetPropertyData(defaultDevID, &theAddress, 0, NULL, sizeof(muted), &muted);
        if (theError != noErr)
        {
            NSLog(@"The device 0x%0x was not muted",defaultDevID);
            return;
        }
    }
    else        //  else set it
    {
        theError = AudioObjectSetPropertyData(defaultDevID, &theAddress, 0, NULL, sizeof(newValue), &newValue);
        if (theError != noErr)
        {
            NSLog(@"The device 0x%0x was unable to set volume", defaultDevID);
        }
            //  if device is able to handle muting, maybe it was muted, so unlock it
        if (hasMute && canMute)
        {
            theAddress.mSelector = kAudioDevicePropertyMute;
            muted = 0;
            theError = AudioObjectSetPropertyData(defaultDevID, &theAddress, 0, NULL, sizeof(muted), &muted);
            if (theError != noErr) {
                NSLog(@"Unable to set volume for device 0x%0x", defaultDevID);
            }
        }
    }
    
}

+ (void) increaseSystemVolume: (float)byAmount {
    [AudioHelper setSystemVolume:systemVolume() + byAmount];
}

+ (void) decreaseSystemVolume: (float)byAmount {
    [AudioHelper setSystemVolume:systemVolume() - byAmount];
}

+ (void) applyMute: (BOOL)mute {
    
    AudioDeviceID                   defaultDevID = kAudioObjectUnknown;
    AudioObjectPropertyAddress      theAddress;
    Boolean                         hasMute, canMute = YES;
    OSStatus                        theError = noErr;
    UInt32                          muted = 0;
    
    defaultDevID = obtainDefaultOutputDevice();
    if (defaultDevID == kAudioObjectUnknown) {            //device not found
        NSLog(@"Device unknown");
        return;
    }
    
    theAddress.mElement = kAudioObjectPropertyElementMaster;
    theAddress.mScope = kAudioDevicePropertyScopeOutput;
    theAddress.mSelector = kAudioDevicePropertyMute;
    muted = mute ? 1 : 0;
    
    hasMute = AudioObjectHasProperty(defaultDevID, &theAddress);
    
    if (hasMute)
    {
        theError = AudioObjectIsPropertySettable(defaultDevID, &theAddress, &canMute);
        if (theError == noErr && canMute)
        {
            theError = AudioObjectSetPropertyData(defaultDevID, &theAddress, 0, NULL, sizeof(muted), &muted);
            if (theError != noErr) NSLog(@"Cannot change mute status of device 0x%0x", defaultDevID);
        }
    }
    
}

+ (BOOL) isMuted {
    
    AudioDeviceID                       defaultDevID = kAudioObjectUnknown;
    AudioObjectPropertyAddress          theAddress;
    Boolean                             hasMute, canMute = YES;
    OSStatus                            theError = noErr;
    UInt32                              muted = 0;
    UInt32                              mutedSize = 4;
    
    defaultDevID = obtainDefaultOutputDevice();
    if (defaultDevID == kAudioObjectUnknown) {            //device not found
        NSLog(@"Device unknown");
        return false;           // works, but not the best return code for this
    }
    
    theAddress.mElement = kAudioObjectPropertyElementMaster;
    theAddress.mScope = kAudioDevicePropertyScopeOutput;
    theAddress.mSelector = kAudioDevicePropertyMute;
    
    hasMute = AudioObjectHasProperty(defaultDevID, &theAddress);
    
    if (hasMute)
    {
        theError = AudioObjectIsPropertySettable(defaultDevID, &theAddress, &canMute);
        if (theError == noErr && canMute)
        {
            theError = AudioObjectGetPropertyData(defaultDevID, &theAddress, 0, NULL, &mutedSize, &muted);
            if (muted) {
                return true;
            }
        }
    }
    
    return false;
    
}

float systemVolume()
{
    AudioDeviceID                defaultDevID = kAudioObjectUnknown;
    UInt32                        theSize = sizeof(Float32);
    OSStatus                    theError;
    Float32                        theVolume = 0;
    AudioObjectPropertyAddress    theAddress;
    
    defaultDevID = obtainDefaultOutputDevice();
    if (defaultDevID == kAudioObjectUnknown) return 0.0;        //device not found: return 0
    
    theAddress.mSelector = kAudioHardwareServiceDeviceProperty_VirtualMasterVolume;
    theAddress.mScope = kAudioDevicePropertyScopeOutput;
    theAddress.mElement = kAudioObjectPropertyElementMaster;
    
    //be sure that the default device has the volume property
    if (! AudioObjectHasProperty(defaultDevID, &theAddress) ) {
        NSLog(@"No volume control for device 0x%0x",defaultDevID);
        return 0.0;
    }
    
    //now read the property and correct it, if outside [0...1]
    theError = AudioObjectGetPropertyData(defaultDevID, &theAddress, 0, NULL, &theSize, &theVolume);
    if ( theError != noErr )    {
        NSLog(@"Unable to read volume for device 0x%0x", defaultDevID);
        return 0.0;
    }
    theVolume = theVolume > 1.0 ? 1.0 : (theVolume < 0.0 ? 0.0 : theVolume);
    
    return theVolume;
}

AudioDeviceID obtainDefaultOutputDevice()
{
    AudioDeviceID theAnswer = kAudioObjectUnknown;
    UInt32 theSize = sizeof(AudioDeviceID);
    AudioObjectPropertyAddress theAddress;
    
    theAddress.mSelector = kAudioHardwarePropertyDefaultOutputDevice;
    theAddress.mScope = kAudioObjectPropertyScopeGlobal;
    theAddress.mElement = kAudioObjectPropertyElementMaster;
    
    //first be sure that a default device exists
    if (! AudioObjectHasProperty(kAudioObjectSystemObject, &theAddress) )    {
        NSLog(@"Unable to get default audio device");
        return theAnswer;
    }
    //get the property 'default output device'
    OSStatus theError = AudioObjectGetPropertyData(kAudioObjectSystemObject, &theAddress, 0, NULL, &theSize, &theAnswer);
    if (theError != noErr) {
        NSLog(@"Unable to get output audio device");
        return theAnswer;
    }
    return theAnswer;
}


@end
