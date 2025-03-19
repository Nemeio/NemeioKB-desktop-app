using System.Collections.Generic;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Applications
{
    internal class ApplicationStartupSettings
    {
        public IList<string> ImagesPath { get; set; }
        public string OutputPath { get; set; }
        public string CompressionType { get; set; }
        public string JSon { get; set; }

        public override string ToString()
        {
            var images = string.Empty;
            foreach (var path in ImagesPath)
            {
                images += $"- {path}\n";
            }

            return $"\nOutputPath: {OutputPath}\nCompressionType: {CompressionType}\nImagesPath: \n{images}\nJSon: \n{JSon}";
        }
    }
}
