using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace Nemeio.WinAutoInstaller
{
    public static class WmiHelper
    {
        private static string NemeioIdentifier = "VEN_NEMEIO&PROD_KEYBOARD";
        private static string CDROMDriveQuery = $"SELECT * FROM Win32_CDROMDrive";

        private static string VIdAndPIdCheck = @"USB\VID_0483&PID_1234\";
        private static string EscapedVIdAndPIdCheck = @"USB\\VID_0483&PID_1234\\";
        private static string UsbCompositeDeviceGuid = "{36fc9e60-c465-11cf-8056-444553540000}";
        private static string UsbControllerDeviceQuery = $"SELECT * FROM Win32_USBControllerDevice";
        private static string PnpEntityQuery = $"SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID = ";

        public static string GetCurrentKeyboardIdentifier()
        {
            // get the first letter of current execution path (assuming we are executing from a drive)
            var driveLetter = Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory).Substring(0, 2).ToLower();
            if (!Regex.IsMatch(driveLetter, "[a-z]{1}:", RegexOptions.IgnoreCase))
            {
                return null;
            }

            // get CD drive to device Id map
            var map = WmiHelper.ListCDRomDriveToDeviceIDMap();

            // and get the result
            if (!map.ContainsKey(driveLetter))
            {
                return null;
            }
            string cdromIdentifier = map[driveLetter];

            // list currently connected keyboard identifiers
            foreach (var id in WmiHelper.ListNemeioDeviceSerialIds())
            {
                if (cdromIdentifier.Contains(id))
                {
                    return id;
                }
            }
            return null;
        }

        /// <summary>
        /// This method lists currently idnetified CDRom drives and return a map of their current letter (lowercased)
        /// and their pending identifier.
        /// </summary>
        /// <returns>Map of CDRom drive letter (lowercased) to associated Device identifier</returns>
        public static Dictionary<string, string> ListCDRomDriveToDeviceIDMap()
        {
            var driveToDeviceId = new Dictionary<string, string>();

            using (var searcher = new ManagementObjectSearcher(CDROMDriveQuery))
            using (var collection = searcher.Get())
            {
                var devices = collection
                    .Cast<ManagementObject>()
                    .Select(item => new KeyValuePair<string, string>((string)item["Drive"], (string)item["DeviceID"]));
                foreach (var item in devices)
                {
                    if (item.Value.Contains(NemeioIdentifier))
                    {
                        driveToDeviceId.Add(item.Key.ToLower(), item.Value);
                    }
                }
            }
            return driveToDeviceId;
        }

        public static IList<string> ListNemeioDeviceSerialIds()
        {
            var nemeioDevices = ListNemeioUsbCompositeUsbDevice();
            var nemeioSerialIds = new List<string>();
            foreach (var device in nemeioDevices)
            {
                var pnpDeviceId = (string)device.GetPropertyValue("PNPDeviceID");
                if (pnpDeviceId.StartsWith(VIdAndPIdCheck))
                {
                    nemeioSerialIds.Add(pnpDeviceId.Remove(0, VIdAndPIdCheck.Length));
                }
            }
            return nemeioSerialIds;
        }

        public static IList<ManagementBaseObject> ListNemeioUsbCompositeUsbDevice()
        {
            var usbDevices = ListUsbDevices(EscapedVIdAndPIdCheck);
            var nemeioUsbCompositeDevices = new List<ManagementBaseObject>();
            foreach(var device in usbDevices)
            {
                var classGuid = (string)device.GetPropertyValue("ClassGuid");
                var pnpDeviceId = (string) device.GetPropertyValue("PNPDeviceID");
                if (classGuid == null || pnpDeviceId == null ||
                    !classGuid.Equals(UsbCompositeDeviceGuid) ||
                    !pnpDeviceId.Contains(VIdAndPIdCheck))
                {
                    continue;
                }
                nemeioUsbCompositeDevices.Add(device);
            }
            return nemeioUsbCompositeDevices;
        }

        static public void TraceManagementBaseObjectList(string filename, IList<ManagementBaseObject> list)
        {
            using (var outputFile = new StreamWriter(filename))
            {
                outputFile.WriteLine($"Total usbDevices found <#:{list.Count}>");

                foreach (var usbDevice in list)
                {
                    outputFile.WriteLine("----- DEVICE -----");
                    foreach (var property in usbDevice.Properties)
                    {
                        outputFile.WriteLine(string.Format("{0}: {1}", property.Name, property.Value));
                    }
                    outputFile.WriteLine("------------------");
                }
            }
        }

        static public void TraceManagementObjectList(string filename, IList<ManagementObject> list)
        {
            using (var outputFile = new StreamWriter(filename))
            {
                outputFile.WriteLine($"Total usbDevices found <#:{list.Count}>");

                foreach (var usbDevice in list)
                {
                    outputFile.WriteLine("----- DEVICE -----");
                    foreach (var property in usbDevice.Properties)
                    {
                        outputFile.WriteLine(string.Format("{0}: {1}", property.Name, property.Value));
                    }
                    outputFile.WriteLine("------------------");
                }
            }
        }

        private static IList<ManagementBaseObject> ListUsbDevices(string filter = null)
        {
            var usbDeviceAddresses = ListUsbDeviceAddresses(filter);
            var usbDevices = new List<ManagementBaseObject>();

            foreach (string usbDeviceAddress in usbDeviceAddresses)
            {
                // query MI for the PNP device info
                // address must be escaped to be used in the query; luckily, the form we extracted previously is already escaped
                using (var searcher = new ManagementObjectSearcher(PnpEntityQuery + usbDeviceAddress))
                using (var collection = searcher.Get())
                {
                    foreach (var device in collection)
                    {
                        usbDevices.Add(device);
                    }
                }
            }
            return usbDevices;
        }

        private static IList<string> ListUsbDeviceAddresses(string filter = null)
        {
            var usbDeviceAddresses = new List<string>();
            using (var searcher = new ManagementObjectSearcher(UsbControllerDeviceQuery))
            using (var collection = searcher.Get())
            {
                var devices = collection
                    .Cast<ManagementObject>()
                    .Select(item => (string)item["Dependent"]);
                foreach (var item in devices)
                {
                    var id = item.Split(new string[] { "DeviceID=" }, 2, StringSplitOptions.None)[1];
                    if (string.IsNullOrEmpty(filter) ||
                        id.Contains(filter))
                    {
                        usbDeviceAddresses.Add(id);
                    }
                }
            }
            return usbDeviceAddresses;
        }
    }
}
