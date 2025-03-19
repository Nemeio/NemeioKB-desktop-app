using System;

namespace Nemeio.Presentation.PackageUpdater.UI
{
    public class PackageUpdaterMessage
    {
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string MainAction { get; private set; }
        public string SecondaryAction { get; private set; }

        public PackageUpdaterMessage(string title, string subtitle, string mainAction, string secondaryAction)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Subtitle = subtitle ?? throw new ArgumentNullException(nameof(subtitle));
            MainAction = mainAction ?? throw new ArgumentNullException(nameof(mainAction));
            SecondaryAction = secondaryAction ?? throw new ArgumentNullException(nameof(secondaryAction));
        }
    }
}
