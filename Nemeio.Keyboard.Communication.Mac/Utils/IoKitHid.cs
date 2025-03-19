using System;
using System.Runtime.InteropServices;

namespace Nemeio.Keyboard.Communication.Mac.Utils
{
    public static class IoKitHid
    {
        public delegate void IOHIDDeviceCallback(IntPtr ctx, IntPtr res, IntPtr sender, IntPtr device);
        public delegate void IOHIDValueCallback(IntPtr ctx, IntPtr res, IntPtr sender, IntPtr val);

        public const uint kIOHIDManagerOptionNone = 0x0;
        public const uint kIOHIDManagerOptionUsePersistentProperties = 0x1;
        public const uint kHIDPage_GenericDesktop = 0x01;
        public const uint kHIDPage_KeyboardOrKeypad = 0x07;
        public const uint kHIDUsage_GD_Keyboard = 0x06;
        public const uint kHIDUsage_GD_Mouse = 0x02;

        public static readonly IntPtr RunLoopModeDefault = CFSTR("kCFRunLoopDefaultMode");
        public static readonly IntPtr kIOHIDVendorIDKey = CFSTR("VendorID");
        public static readonly IntPtr IOHIDVendorIDSourceKey = CFSTR("VendorIDSource");
        public static readonly IntPtr kIOHIDProductIDKey = CFSTR("ProductID");
        public static readonly IntPtr IOHIDVersionNumberKey = CFSTR("VersionNumber");
        public static readonly IntPtr IOHIDManufacturerKey = CFSTR("Manufacturer");
        public static readonly IntPtr kIOHIDProductKey = CFSTR("Product");
        public static readonly IntPtr IOHIDDeviceUsageKey = CFSTR("DeviceUsage");
        public static readonly IntPtr IOHIDDeviceUsagePageKey = CFSTR("DeviceUsagePage");
        public static readonly IntPtr IOHIDDeviceUsagePairsKey = CFSTR("DeviceUsagePairs");

        public static IntPtr CFSTR(string cStr) => __CFStringMakeConstantString(cStr);

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        public static extern IntPtr IOHIDManagerCreate(IntPtr allocator, UInt32 options);

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        public static extern IntPtr IOHIDManagerOpen(IntPtr manager, IntPtr options);

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        public static extern void IOHIDManagerRegisterDeviceMatchingCallback(IntPtr manager, IOHIDDeviceCallback callback, IntPtr context);

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        public static extern void IOHIDManagerRegisterDeviceRemovalCallback(IntPtr manager, IOHIDDeviceCallback callback, IntPtr context);

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        public static extern void IOHIDManagerScheduleWithRunLoop(IntPtr manager, IntPtr runLoop, IntPtr runLoopMode);

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        public static extern ulong IOHIDManagerGetTypeID();

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        public static extern IntPtr IOHIDDeviceOpen(IntPtr device, IntPtr options);

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        public static extern bool IOHIDDeviceConformsTo(IntPtr device, uint usagePage, uint usage);

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        public static extern IntPtr IOHIDDeviceGetProperty(IntPtr device, IntPtr key);

        [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
        public static extern ulong CFGetTypeID(IntPtr cf);

        [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
        public static extern IntPtr CFRunLoopGetCurrent();

        [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
        public static extern IntPtr CFRunLoopGetMain();

        [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
        public static extern void CFRunLoopRun();

        [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
        public static extern IntPtr CFRetain(IntPtr p);

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        public static extern void IOHIDManagerSetDeviceMatching(IntPtr manager, IntPtr matching);

        [DllImport("/System/Library/Frameworks/ApplicationServices.framework/Versions/Current/ApplicationServices")]
        static extern IntPtr __CFStringMakeConstantString(string cStr);

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        public static extern IntPtr IOHIDManagerCopyDevices(IntPtr manager);
    }
}
