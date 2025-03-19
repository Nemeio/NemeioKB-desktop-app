//
//  AudioHelper.h
//  ExtendedTools
//
//  Created by Kevin Sibue on 17/06/2020.
//  Copyright © 2020 Nemeio. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface AudioHelper : NSObject

+ (void) setSystemVolume: (float)inVolume;
+ (void) increaseSystemVolume: (float)byAmount;
+ (void) decreaseSystemVolume: (float)byAmount;
+ (void) applyMute: (BOOL)mute;
+ (BOOL) isMuted;

@end

NS_ASSUME_NONNULL_END
