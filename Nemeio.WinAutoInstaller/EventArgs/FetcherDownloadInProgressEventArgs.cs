using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemeio.WinAutoInstaller.EventArgs
{
    public class FetcherDownloadInProgressEventArgs
    {
        public double BytesIn { get; private set; }
        public double TotalBytes { get; private set; }
        public double Percent { get; private set; }

        public FetcherDownloadInProgressEventArgs(double bytesIn, double totalBytes, double percent)
        {
            BytesIn = bytesIn;
            TotalBytes = totalBytes;
            Percent = percent;
        }
    }
}
