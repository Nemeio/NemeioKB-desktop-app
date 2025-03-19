using System;

namespace Nemeio.Core.Versions
{
    public static class VersionExtensions
    {
        public enum VersionComponent
        {
            Stm32 = 1,
            Nrf = 2,
            WindowsDesktop = 3,
            MacDesktop = 4,
            Configurator = 5,
            WindowsMiniInstaller = 6,
            MacMiniInstaller = 7
        }

        public static VersionComponent GetComponent(this Version version)
        {
            var revision = version.Revision.ToString();
            var firstBuildNumberString = revision[0].ToString();

            var component = (VersionComponent)Enum.Parse(typeof(VersionComponent), firstBuildNumberString);

            return component;
        }
    }
}
