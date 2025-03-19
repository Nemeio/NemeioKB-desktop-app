using System;

namespace Nemeio.Cli.Application
{
    [Flags]
    internal enum ApplicationExitCode : uint
    {
        None = 0,
        UnknownFailure = 1,
        KeyboardDisconnected = 2,
        CanceledCommand = 4,
        FetchVersionFailed = 8,
        UpdateKeyboardFailed = 16,
        FactoryResetFailed = 32,
        DeleteProtectedLayout = 64,
        LayoutNotFound = 128,
        DeleteLayoutFailed = 256,
        InvalidParameter = 512,
        ListLayoutFailed = 1024,
        ApplyLayoutFailed = 2048,
        AddLayoutFailed = 4096,
        ChangeLayoutFailed = 8192,
        GetParametersFailed = 16324,
        SetParametersFailed = 32648
    }
}
