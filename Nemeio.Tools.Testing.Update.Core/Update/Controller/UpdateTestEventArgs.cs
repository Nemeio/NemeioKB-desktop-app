using System;
using Nemeio.Tools.Testing.Update.Core.Reports;

namespace Nemeio.Tools.Testing.Update.Core.Update.Controller
{
    public class UpdateTestEventArgs
    {
        public UpdateTestReport Report { get; private set; }
        
        public UpdateTestEventArgs(UpdateTestReport report) 
        {
            Report = report ?? throw new ArgumentNullException(nameof(report));
        }
    }
}
