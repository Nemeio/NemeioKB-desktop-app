using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nemeio.WinAutoInstaller.Models;

namespace Nemeio.WinAutoInstaller.EventArgs
{
    public class InstallerDownloadFinishedEventArgs
    {
        public ErrorCode ErrorCode { get; private set; }

        public Uri DownloadPath { get; private set; }

        public InstallerDownloadFinishedEventArgs(ErrorCode code, Uri path)
        {
            ErrorCode = code;
            DownloadPath = path;
        }
    }
}
