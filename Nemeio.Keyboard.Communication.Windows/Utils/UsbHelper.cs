using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Nemeio.Keyboard.Communication.Windows.Utils
{
    internal static class UsbHelper
    {
        internal static IEnumerable<DeviceProperties> GetUSBDevicesProperties()
        {
            const string usbType = "USB";

            var hDevInfo = Win32Wrapper.SetupDiGetClassDevs(IntPtr.Zero, usbType, IntPtr.Zero, (int)(Win32Wrapper.DIGCF.DIGCF_PRESENT | Win32Wrapper.DIGCF.DIGCF_ALLCLASSES));
            try
            {
                uint i = 0;
                bool success = true;
                do
                {
                    success = ReadDeviceProperties(hDevInfo, i++, out var deviceProperties);
                    yield return deviceProperties;

                } while (success);
            }
            finally
            {
                Win32Wrapper.SetupDiDestroyDeviceInfoList(hDevInfo);
            }
        }

        private static bool ReadDeviceProperties(IntPtr hDevInfo, uint memberIndex, out DeviceProperties deviceProperties)
        {
            deviceProperties = new DeviceProperties()
            {
                Properties = new Dictionary<string, string>()
            };

            Win32Wrapper.SP_DEVINFO_DATA deviceInfoData = new Win32Wrapper.SP_DEVINFO_DATA();
            deviceInfoData.cbSize = (uint)Marshal.SizeOf(typeof(Win32Wrapper.SP_DEVINFO_DATA));

            if (Win32Wrapper.SetupDiEnumDeviceInfo(hDevInfo, memberIndex, ref deviceInfoData))
            {
                deviceProperties.HardwareId = ReadProperty(hDevInfo, deviceInfoData, Win32Wrapper.SPDRP.SPDRP_HARDWAREID);

                var propertyKeys = (Win32Wrapper.SPDRP[])Enum.GetValues(typeof(Win32Wrapper.SPDRP));
                foreach (var propertyKey in propertyKeys)
                {
                    var propertyValue = ReadProperty(hDevInfo, deviceInfoData, propertyKey);
                    deviceProperties.Properties.Add(propertyKey.ToString(), propertyValue);
                }

                return true;
            }

            return false;
        }

        private static string ReadProperty(IntPtr hDevInfo, Win32Wrapper.SP_DEVINFO_DATA devInfoData, Win32Wrapper.SPDRP property)
        {
            uint requiredPropertySize = 0;
            uint nullPtr = 0;

            //First query for the size of the Hardware ID, so we know the size of the needed buffer to allocate to store the data.
            Win32Wrapper.SetupDiGetDeviceRegistryProperty(hDevInfo, ref devInfoData, (uint)property, ref nullPtr, IntPtr.Zero, 0, ref requiredPropertySize);

            IntPtr intPtrBuffer = Marshal.AllocHGlobal((int)requiredPropertySize);
            try
            {
                if (Win32Wrapper.SetupDiGetDeviceRegistryProperty(hDevInfo, ref devInfoData, (uint)property, ref nullPtr, intPtrBuffer, requiredPropertySize, ref nullPtr))
                {
                    return Marshal.PtrToStringAuto(intPtrBuffer);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(intPtrBuffer);
            }

            return null;
        }
    }
}
