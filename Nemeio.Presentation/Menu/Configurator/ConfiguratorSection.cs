namespace Nemeio.Presentation.Menu.Configurator
{
    public sealed class ConfiguratorSection
    {
        public string Title { get; private set; }

        public ConfiguratorSection(string title)
        {
            Title = title;
        }
    }
}
