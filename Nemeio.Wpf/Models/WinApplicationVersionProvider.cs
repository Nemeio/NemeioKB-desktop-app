using System;
using System.Reflection;
using Nemeio.Core.Versions;

namespace Nemeio.Wpf.Models
{
    public class WinApplicationVersionProvider : IApplicationVersionProvider
    {
        public Version GetVersion() => Assembly.GetExecutingAssembly().GetName().Version;
    }
}
