using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemeio.WinAutoInstaller.EventArgs
{
    public class NetworkConnectivityEventArgs
    {
        public bool IsAvailable { get; private set; }

        public NetworkConnectivityEventArgs(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }
    }
}
