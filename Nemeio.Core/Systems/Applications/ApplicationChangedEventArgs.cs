namespace Nemeio.Core.Systems.Applications
{
    public sealed class ApplicationChangedEventArgs
    {
        public Application Application { get; private set; }

        public ApplicationChangedEventArgs(Application application)
        {
            Application = application;
        }
    }
}
