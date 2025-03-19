using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Nemeio.Core;
using Nemeio.Keyboard.Communication.Mac.Watchers;

namespace Nemeio.Keyboard.Communication.Mac.Utils
{
    internal unsafe class IoKitUsbExtern
    {
        private enum CFNumberType
        {
            kCFNumberSInt32Type = 3,
            kCFNumberCharType = 7,
            kCFNumberShortType = 8,
            kCFNumberIntType = 9,
            kCFNumberLongType = 10,
            kCFNumberLongLongType = 11,
            kCFNumberFloatType = 12,
            kCFNumberDoubleType = 13
        };

        private enum CFStringEncoding
        {
            kCFStringEncodingUTF8 = 0x08000100,
        }

        private const int KERN_SUCCESS = 0;
        private const int kNilOptions = 0;

        private const int kIOReturnSuccess = 0;

        private const uint kIORegistryIterateRecursively = 0x00000001;
        private const uint kIORegistryIterateParents = 0x00000002;

        private const string kIOServicePlane = "IOService";
        private const string kUSBProductID = "idProduct";
        private const string kUSBVendorID = "idVendor";
        private const string kUSBDeviceReleaseNumber = "bcdDevice";
        private const string kIOUSBDeviceClassName = "IOUSBDevice";
        private const string kIODialinDeviceKey = "IODialinDevice";

        private const string kIOPublishNotification = "IOServicePublish";
        private const string kIOFirstPublishNotification = "IOServiceFirstPublish";
        private const string kIOMatchedNotification = "IOServiceMatched";
        private const string kIOFirstMatchNotification = "IOServiceFirstMatch";
        private const string kIOTerminatedNotification = "IOServiceTerminate";
        private const string kIOWillTerminateNotification = "IOServiceWillTerminate";

        private const string kIOSerialBSDServiceValue = "IOSerialBSDClient"; // Service Matching That is the 'IOProviderClass'.
        private const string _ioKitFrameworkPath = "/System/Library/Frameworks/IOKit.framework/IOKit";
        private const string _coreFoundationFrameworkPath = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
        private const string _applicationServicesFrameworkPath = "/System/Library/Frameworks/ApplicationServices.framework/Versions/Current/ApplicationServices";

        private static readonly IntPtr kIOMasterPortDefault = IntPtr.Zero;
        private static readonly IntPtr kCFAllocatorDefault = IntPtr.Zero;
        private static readonly IntPtr RunLoopModeDefault = __CFStringMakeConstantString("kCFRunLoopDefaultMode");

        [DllImport(_applicationServicesFrameworkPath)]
        private static extern IntPtr __CFStringMakeConstantString(string cStr);

        [DllImport(_coreFoundationFrameworkPath)]
        private static extern IntPtr CFRunLoopGetCurrent();

        [DllImport(_coreFoundationFrameworkPath)]
        private static extern IntPtr CFRunLoopGetMain();

        [DllImport(_coreFoundationFrameworkPath)]
        private static extern void CFRunLoopRun();

        [DllImport(_coreFoundationFrameworkPath)]
        private static extern IntPtr CFRetain(IntPtr p);

        [DllImport(_coreFoundationFrameworkPath)]
        private static extern ulong CFGetTypeID(IntPtr cf);

        [DllImport(_ioKitFrameworkPath)]
        private static extern int IORegistryEntryCreateIterator(IntPtr entry, string plane, uint options, out IntPtr iterator);

        [DllImport(_ioKitFrameworkPath)]
        private static extern int IORegistryEntryGetNameInPlane(IntPtr entry, string plane, char* name);

        [DllImport(_ioKitFrameworkPath)]
        private static extern bool IOObjectConformsTo(IntPtr obj, string className);

        [DllImport(_ioKitFrameworkPath)]
        private static extern int IOObjectRelease(IntPtr obj);

        [DllImport(_ioKitFrameworkPath)]
        private static extern int CFRelease(IntPtr obj);

        [DllImport(_ioKitFrameworkPath)]
        private static extern IntPtr IORegistryEntrySearchCFProperty(IntPtr entry, string plane, IntPtr key, IntPtr allocator, uint options);

        [DllImport(_ioKitFrameworkPath)]
        private static extern bool CFNumberGetValue(IntPtr number, CFNumberType theType, out IntPtr valuePtr);

        [DllImport(_ioKitFrameworkPath)]
        private static extern bool CFStringGetCString(IntPtr theString, char* buffer, long bufferSize, CFStringEncoding encoding);

        [DllImport(_ioKitFrameworkPath)]
        private static extern IntPtr IOServiceAddMatchingNotification(IntPtr notifyPort, string notificationType, IntPtr matching, IOServiceMatchingCallback callback, IntPtr refCon, out IntPtr notification);

        [DllImport(_ioKitFrameworkPath)]
        private static extern IntPtr IONotificationPortCreate(IntPtr masterPort);

        [DllImport(_ioKitFrameworkPath)]
        private static extern IntPtr IOServiceMatching(string name);

        [DllImport(_ioKitFrameworkPath)]
        private static extern IntPtr IOIteratorNext(IntPtr iterator);

        [DllImport(_ioKitFrameworkPath)]
        private static extern IntPtr IONotificationPortGetRunLoopSource(IntPtr notify);

        [DllImport(_ioKitFrameworkPath)]
        private static extern void CFRunLoopAddSource(IntPtr rl, IntPtr source, IntPtr mode);

        [DllImport(_ioKitFrameworkPath)]
        private static extern void CFDictionarySetValue(IntPtr theDict, IntPtr key, IntPtr value);

        [DllImport(_ioKitFrameworkPath)]
        private static extern IntPtr CFNumberCreate(IntPtr allocator, CFNumberType theType, ref Int32 valuePtr);

        [DllImport(_ioKitFrameworkPath)]
        private static extern int IOServiceGetMatchingServices(IntPtr masterPort, IntPtr matching, out IntPtr iterator);

        // class instantiation and methods: formerly part of MacUsbWatcher
        private readonly object _lock = new object();
        private readonly IOServiceMatchingCallback _addDeviceDelegate;
        private readonly IOServiceMatchingCallback _removeDeviceDelegate;
        private IntPtr _iteratorAdded;
        private IntPtr _iteratorRemoved;
        private IntPtr _plugMatchingDict;
        private IntPtr _unplugMatchingDict;

        internal delegate void IOServiceMatchingCallback(IntPtr refCon, IntPtr iterator);

        internal IoKitUsbExtern(IOServiceMatchingCallback addDeviceDelegate, IOServiceMatchingCallback removeDeviceDelegate)
        {
            _addDeviceDelegate = addDeviceDelegate;
            _removeDeviceDelegate = removeDeviceDelegate;
        }

        public void Start()
        {
            var notifyPort = IONotificationPortCreate(kIOMasterPortDefault);
            var notificationRunLoopSource = IONotificationPortGetRunLoopSource(notifyPort);
            CFRunLoopAddSource(CFRunLoopGetCurrent(), notificationRunLoopSource, RunLoopModeDefault);
            _plugMatchingDict = AddPlugNotification(notifyPort, _addDeviceDelegate, out _iteratorAdded);
            _unplugMatchingDict = AddUnplugNotification(notifyPort, _removeDeviceDelegate, out _iteratorRemoved);
            IoKitHid.CFRunLoopRun();
        }

        public void Stop()
        {
            CFRelease(_iteratorAdded);
            CFRelease(_iteratorRemoved);
            IOObjectRelease(_plugMatchingDict);
            IOObjectRelease(_unplugMatchingDict);
        }

        public void Iterate(IntPtr iterator, Action<IntPtr, string> action)
        {
            IntPtr device;
            while ((device = IOIteratorNext(iterator)) != IntPtr.Zero)
            {
                try
                {

                    if (TryGetNemeioUsbDevice(device, out var macUsbDevice))
                    {
                        lock (_lock)
                        {
                            if (action != null)
                            {
                                action(device, macUsbDevice);
                            }
                        }
                    }
                }
                finally
                {
                    IOObjectRelease(device);
                }
            }
        }

        private static IntPtr AddPlugNotification(IntPtr notifyPort, IOServiceMatchingCallback deviceAddedDelegate, out IntPtr iteratorAdded)
        {
            var plugMatchingDict = IOServiceMatching(kIOSerialBSDServiceValue);
            IOServiceAddMatchingNotification(notifyPort, kIOMatchedNotification, plugMatchingDict, deviceAddedDelegate, IntPtr.Zero, out iteratorAdded);
            deviceAddedDelegate(IntPtr.Zero, iteratorAdded);
            return plugMatchingDict;
        }

        private static IntPtr AddUnplugNotification(IntPtr notifyPort, IOServiceMatchingCallback deviceRemovedDelegate, out IntPtr iteratorRemoved)
        {
            var unplugMatchingDict = IOServiceMatching(kIOSerialBSDServiceValue);
            IOServiceAddMatchingNotification(notifyPort, kIOWillTerminateNotification, unplugMatchingDict, deviceRemovedDelegate, IntPtr.Zero, out iteratorRemoved);
            deviceRemovedDelegate(IntPtr.Zero, iteratorRemoved);
            return unplugMatchingDict;
        }

        private static bool TryGetNemeioUsbDevice(IntPtr device, out string usbDeviceString)
        {
            var isNemeioDevice = false;
            var createIteratorResult = IORegistryEntryCreateIterator(device, kIOServicePlane, kIORegistryIterateParents | kIORegistryIterateRecursively, out var iterator);

            if (createIteratorResult == KERN_SUCCESS)
            {
                var service = IOIteratorNext(iterator);

                var hasNemeioVendorId = GetIntDeviceProperty(service, kUSBVendorID) == NemeioConstants.VendorId;
                var hasNemeioProductId = GetIntDeviceProperty(service, kUSBProductID) == NemeioConstants.ProductIdWithInstaller || GetIntDeviceProperty(service, kUSBProductID) == NemeioConstants.ProductIdWithoutInstaller;

                usbDeviceString = (service != IntPtr.Zero && hasNemeioVendorId && hasNemeioProductId)
                    ? GetUsbDeviceString(service)
                    : null;

                IOObjectRelease(service);
                IOObjectRelease(iterator);

                isNemeioDevice = usbDeviceString != null;
            }
            else
            {
                usbDeviceString = null;
            }

            return isNemeioDevice;
        }

        /// <summary>
        /// Allow to iterate on all USB connected devices
        /// </summary>
        /// <param name="loopAction">Access to the current device. MUST NOT release yourself device object</param>
        private static void IterateOnConnectedUsbDevice(Action<IntPtr> loopAction)
        {
            IntPtr iterator;
            IntPtr device;

            var matchingDict = IOServiceMatching(kIOUSBDeviceClassName);
            if (matchingDict == IntPtr.Zero)
            {
                throw new InvalidOperationException("Can't scan devices");
            }

            var result = IOServiceGetMatchingServices(kIOMasterPortDefault, matchingDict, out iterator);
            if (result != KERN_SUCCESS)
            {
                CFRelease(matchingDict);

                throw new InvalidOperationException("Can't get matching services");
            }

            while ((device = IOIteratorNext(iterator)) != IntPtr.Zero)
            {
                loopAction(device);

                IOObjectRelease(device);
            }

            IOObjectRelease(iterator);
        }

        public static IList<IoUsbDevice> GetConnectedNemeioDevices()
        {
            var devices = new List<IoUsbDevice>();

            try
            {
                IterateOnConnectedUsbDevice((device) =>
                {
                    var vendorId = GetIntDeviceProperty(device, kUSBVendorID);
                    var productId = GetIntDeviceProperty(device, kUSBProductID);

                    var hasNemeioVendorId = vendorId == NemeioConstants.VendorId;
                    var hasNemeioProductId = productId == NemeioConstants.ProductIdWithInstaller || productId == NemeioConstants.ProductIdWithoutInstaller;

                    if (hasNemeioVendorId && hasNemeioProductId)
                    {
                        var usbIdentifier = GetUsbDeviceString(device);
                        var version = GetDeviceVersionProperty(device);

                        var usbDevice = new IoUsbDevice(usbIdentifier, vendorId, productId, new Version(version));

                        devices.Add(usbDevice);
                    }
                });
            }
            catch (InvalidOperationException)
            {
                //  Nothing to do here
                //  We swallow exception
            }

            return devices;
        }

        private static int GetIntDeviceProperty(IntPtr device, string property)
        {
            var propPtr = FindDeviceProperty(device, property);
            if (propPtr == IntPtr.Zero)
            {
                return 0;
            }

            CFNumberGetValue(propPtr, CFNumberType.kCFNumberIntType, out var bsdValue);

            return bsdValue.ToInt32();
        }

        private static string GetStringDeviceProperty(IntPtr device, string property)
        {
            var propPtr = FindDeviceProperty(device, property);
            if (propPtr == IntPtr.Zero)
            {
                return string.Empty;
            }

            var maxValueSize = 128;
            fixed (char* versionLabel = new char[maxValueSize])
            {
                CFStringGetCString(propPtr, versionLabel, maxValueSize, CFStringEncoding.kCFStringEncodingUTF8);
                var propVal = Marshal.PtrToStringUTF8((IntPtr)versionLabel);

                return propVal;
            }
        }

        public static string GetDeviceVersionProperty(IntPtr device)
        {
            var version = GetIntDeviceProperty(device, kUSBDeviceReleaseNumber);

            var versionBytes = BitConverter.GetBytes(version);

            var major = Convert.ToInt16(versionBytes[1]);
            var minor = Convert.ToInt16(versionBytes[0]);

            return $"{major}.{minor}";
        }

        private static IntPtr FindDeviceProperty(IntPtr device, string property)
        {
            var propPtr = IORegistryEntrySearchCFProperty(device, kIOServicePlane, __CFStringMakeConstantString(property), kCFAllocatorDefault, kIORegistryIterateParents | kIORegistryIterateRecursively);

            return propPtr;
        }

        private unsafe static string GetUsbDeviceString(IntPtr device)
        {
            var usbDevice = string.Empty;

            var maxValueSize = 128;
            fixed (char* serviceName = new char[maxValueSize])
            {
                var errorCode = IORegistryEntryGetNameInPlane(device, kIOServicePlane, serviceName);
                if (errorCode == KERN_SUCCESS)
                {
                    var propPtr = IORegistryEntrySearchCFProperty(device, kIOServicePlane, __CFStringMakeConstantString(kIODialinDeviceKey), IntPtr.Zero, kIORegistryIterateRecursively);
                    if (propPtr != IntPtr.Zero)
                    {
                        var succeed = CFStringGetCString(propPtr, serviceName, maxValueSize, CFStringEncoding.kCFStringEncodingUTF8);
                        if (succeed)
                        {
                            usbDevice = Marshal.PtrToStringAnsi((IntPtr)serviceName);
                        }
                    }
                }
            }

            return usbDevice;
        }
    }
}
