using System;
using System.IO;

namespace Nemeio.Tools.LayoutConverter.Providers
{
    internal class PathProvider : IPathProvider
    {
        private const string NemeioFolderName = "Nemeio";

        public string GetNemeioApplicationPath()
        {
            var applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            return Path.Combine(applicationDataPath, NemeioFolderName);
        }
    }
}
