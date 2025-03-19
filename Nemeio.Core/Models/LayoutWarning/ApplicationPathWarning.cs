using System.IO;

namespace Nemeio.Core.Models.LayoutWarning
{
    public class ApplicationPathWarning : LayoutWarning
    {
        public override LayoutWarningType Type => LayoutWarningType.LinkApplicationPath;

        public string ApplicationPath { get; private set; }

        public string ApplicationName { get; private set; }

        public ApplicationPathWarning(string applicationPath)
        {
            ApplicationPath = applicationPath;
            ApplicationName = Path.GetFileNameWithoutExtension(applicationPath);
        }
    }
}
