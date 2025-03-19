using System;
using System.Linq;

namespace Nemeio.Keyboard.Communication.Windows.Utils
{
    internal static class FindUsbDescriptor
    {
        /// <summary>
        /// WARNING! This method find the first USB device that match VID and PID.
        /// Must change it when Nemeio app need to allow multiple keyboard as same time
        /// </summary>
        public static string GetUsbVersion(string vendorId, string productId)
        {
            var deviceProperties = UsbHelper.GetUSBDevicesProperties()
                .Where(device => device.HardwareId != null)
                .Where(device => device.HardwareId.IndexOf($"VID_{vendorId}", StringComparison.OrdinalIgnoreCase) >= 0)
                .Where(device => device.HardwareId.IndexOf($"PID_{productId}", StringComparison.OrdinalIgnoreCase) >= 0)
                .FirstOrDefault();

            var version = ExtractVersionFromHardwareId(deviceProperties?.HardwareId);
            return version;
        }

        // HardwareId is in format : 'USB\VID_0483&PID_1234&REV_0200&MI_03'
        private static string ExtractVersionFromHardwareId(string hardwareId)
        {
            var revision = GetRevision(hardwareId);
            var version = ExtractVersionFromRevision(revision);
            return version;
        }

        private static string GetRevision(string hardwareId)
        {
            const char PartSeparator = '&';

            if (!string.IsNullOrEmpty(hardwareId))
            {
                var parts = hardwareId.Split(PartSeparator);
                return parts.Length >= 3 ? parts[2] : null;
            }

            return null;
        }

        // revision is in format : 'REV_0200'
        private static string ExtractVersionFromRevision(string revision)
        {
            const string RevisionPrefix = "REV_";

            if (revision?.StartsWith(RevisionPrefix, StringComparison.OrdinalIgnoreCase) == true)
            {
                var version = revision.Replace(RevisionPrefix, string.Empty);
                var major = int.Parse(version.Substring(0, 2));
                var minor = int.Parse(version.Substring(2, 2));
                return $"{major}.{minor}";
            }

            return null;
        }
    }
}
