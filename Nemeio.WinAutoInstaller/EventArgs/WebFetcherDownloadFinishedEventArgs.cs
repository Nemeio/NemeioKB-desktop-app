using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nemeio.WinAutoInstaller.Models;

namespace Nemeio.WinAutoInstaller.EventArgs
{
    public class WebFetcherDownloadFinishedEventArgs
    {
        public ErrorCode ErrorCode { get; private set; }
        public byte[] Data { get; private set; }

        public WebFetcherDownloadFinishedEventArgs(ErrorCode code, byte[] data)
        {
            ErrorCode = code;
            Data = data;
        }
    }
}
