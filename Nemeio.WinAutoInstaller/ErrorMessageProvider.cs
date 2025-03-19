using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nemeio.WinAutoInstaller.Models;

namespace Nemeio.WinAutoInstaller
{
    public class ErrorMessageProvider
    {
        private Dictionary<ErrorCode, string> _errorMessage;

        public ErrorMessageProvider()
        {
            _errorMessage = new Dictionary<ErrorCode, string>()
            {
                { ErrorCode.WinAutoInstallerKeyboardNotFound, "No keyboard found." },
                { ErrorCode.WinAutoInstallerInternetNotAvailable, "An Internet connection is required to download Nemeio. Please try again later." },
                { ErrorCode.WinAutoInstallerServerTimeout, "No response from the server within the allotted time. Please try again later." },
                { ErrorCode.WinAutoInstallerDownloadFailed, "Download failed." },
                { ErrorCode.WinAutoInstallerDownloadCancelByUser, "You have stopped the installer." },
                { ErrorCode.WinAutoInstallerInvalidChecksum, "Downloaded file is corrupted. Please try again." },
                { ErrorCode.WinAutoInstallerGetDownloadInfoFailed, "Can't request installer server. Please try again later." },
                { ErrorCode.WinAutoInstallerFileSystemWriteDenied, "Unauthorized to write on file system." },
                { ErrorCode.WinAutoInstallerUpdateNotFound, "No installer found. Please contact support." },
            };
        }

        public string GetErrorMessage(ErrorCode code)
        {
            if (_errorMessage.ContainsKey(code))
            {
                return _errorMessage[code];
            }

            return "An error occured. Please try again later.";
        }

        public string GetFullErrorMessage(ErrorCode code)
        {
            var errorCode = (int)code;
            var errorMessage = GetErrorMessage(code);

            var hexadecimalErrorCode = errorCode.ToString("X02");

            return $"[{hexadecimalErrorCode}] {errorMessage}";
        }
    }
}
