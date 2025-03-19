using Nemeio.WinAutoInstaller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemeio.WinAutoInstaller.EventArgs
{
    public class FetcherDownloadStartedEventArgs
    {
        public SoftwareInfo Software { get; private set; }

        public FetcherDownloadStartedEventArgs(SoftwareInfo software)
        {
            Software = software;
        }
    }
}
