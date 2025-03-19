using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Managers;

namespace Nemeio.Core.Test.Fakes
{
    public class FakeLanguageManager : LanguageManager
    {
        public string FakeCulture = "fk-FK";

        public FakeLanguageManager(ILoggerFactory loggerFactory) 
            : base(loggerFactory) { }

        private bool IsFake(string filename)
        {
            return filename == $"{FakeCulture}.xml";
        }

        private Assembly GetFakeAssembly()
        {
            return typeof(FakeLanguageManager).Assembly;
        }

        private string GetFakeResourcesPath()
        {
            return "Nemeio.Core.Test.Resources";
        }

        public override bool ResourceExists(string resourceName)
        {
            if (IsFake(resourceName))
            {
                var resourceNames = GetFakeAssembly().GetManifestResourceNames();
                var fileName = $"{GetFakeResourcesPath()}.{resourceName}";

                return resourceNames.Contains(fileName);
            }

            return base.ResourceExists(resourceName);
        }

        public override Stream GetResourceStream(string filename)
        {
            if (IsFake(filename))
            {
                return GetFakeAssembly().GetManifestResourceStream($"{GetFakeResourcesPath()}.{filename}");
            }

            return base.GetResourceStream(filename);
        }
    }
}
