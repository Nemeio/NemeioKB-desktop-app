using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nemeio.Tools.Testing.Update.Core.Update.Tester
{
    public interface IUpdateTesterFactory
    {
        Task<IEnumerable<IUpdateTester>> CreateTesters(string environment, string outputPath, Uri serverUri, UpdateVersionRange range);
    }
}
