using Nemeio.Core.DataModels;

namespace Nemeio.Core.Settings.Parser
{
    public interface ISettingsParser
    {
        DevelopmentSettings Parse(string filePath);
    }
}
