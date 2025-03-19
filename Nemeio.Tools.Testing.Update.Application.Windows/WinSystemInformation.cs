using System;
using Nemeio.Core.JsonModels;
using Nemeio.Tools.Testing.Update.Application.Application;
using Nemeio.Tools.Testing.Update.Core.System;

namespace Nemeio.Tools.Testing.Update.Application.Windows
{
    public class WinSystemInformation : ISystemInformation
    {
        public PlatformArchitecture GetCurrentPlatform()
        {
            return Environment.Is64BitOperatingSystem ? PlatformArchitecture.x64 : PlatformArchitecture.x86;
        }

        public Nemeio.Core.Services.OperatingSystem GetRunningSystem() => Nemeio.Core.Services.OperatingSystem.Windows;
    }
}
