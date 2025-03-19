using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemeio.WinAutoInstaller.EventArgs
{
    public class InstallerDownloadProgressChangedEventArgs
    {
        public int Percent { get; private set; }

        public InstallerDownloadProgressChangedEventArgs(int percent) => Percent = percent;
    }
}
