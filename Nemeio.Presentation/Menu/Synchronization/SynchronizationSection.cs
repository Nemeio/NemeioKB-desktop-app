namespace Nemeio.Presentation.Menu.Synchronization
{
    public sealed class SynchronizationSection
    {
        public string Title { get; private set; }
        public string ProgressDescription { get; private set; }
        public bool Visible { get; private set; }

        public SynchronizationSection(string title, string progressDescription, bool visible)
        {
            Title = title;
            ProgressDescription = progressDescription;
            Visible = visible;
        }
    }
}
