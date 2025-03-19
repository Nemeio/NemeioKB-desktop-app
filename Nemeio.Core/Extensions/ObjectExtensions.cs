using System;

namespace Nemeio.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsOSXPlatform(this object obj) => IsOSXPlatform();

        public static bool IsWindowsPlatform(this object obj) => !IsOSXPlatform(obj);

        public static bool IsOSXPlatform() => Environment.OSVersion.Platform == PlatformID.MacOSX ||
                                              Environment.OSVersion.Platform == PlatformID.Unix;
    }
}
