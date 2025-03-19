namespace Nemeio.Tools.Firmware.PackageBuilder.Applications
{
    internal class ApplicationStartupSettings
    {
        internal string ManifestFilePath { get; set; }
        internal string InputFileDirectoryPath { get; set; }
        internal string OuputFilePath { get; set; }

        public override string ToString()
        {
            return $"ManifestFilePath: <{ManifestFilePath}>, InputFileDirectoryPath: <{InputFileDirectoryPath}>, OuputFilePath: <{OuputFilePath}>";
        }
    }
}
