using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemeio.WinAutoInstaller.Models
{
    [Flags]
    public enum ErrorCode
    {
        //  Common
        WinAutoInstallerSuccess                         = 0x04000000,

        //  Keyboard Errors
        WinAutoInstallerKeyboardNotFound                = 0x04000101,

        //  Connectivity Errors
        WinAutoInstallerInternetNotAvailable            = 0x04000201,
        WinAutoInstallerServerTimeout                   = 0x04000202,

        //  Download Errors
        WinAutoInstallerDownloadFailed                  = 0x04000301,
        WinAutoInstallerDownloadCancelByUser            = 0x04000302,
        WinAutoInstallerInvalidChecksum                 = 0x04000303,
        WinAutoInstallerGetDownloadInfoFailed           = 0x04000304,
        WinAutoInstallerUpdateNotFound                  = 0x04000305,

        //  File System Errors
        WinAutoInstallerFileSystemWriteDenied           = 0x04000401
    }
}
