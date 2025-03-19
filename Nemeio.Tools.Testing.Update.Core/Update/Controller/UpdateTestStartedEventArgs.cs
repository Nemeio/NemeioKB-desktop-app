using System;
using Nemeio.Tools.Testing.Update.Core.Update.Tester;

namespace Nemeio.Tools.Testing.Update.Core.Update.Controller
{
    public class UpdateTestStartedEventArgs
    {
        public IUpdateTester Tester { get; private set; }

        public UpdateTestStartedEventArgs(IUpdateTester tester)
        {
            Tester = tester ?? throw new ArgumentNullException(nameof(tester));
        }
    }
}
