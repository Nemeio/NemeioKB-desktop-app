using System;
using Nemeio.Core.DataModels.Locale;

namespace Nemeio.Core.EventArguments
{
    public class NotificationReleaseEventArgs : EventArgs
    {
        public StringId Message { get; private set; }

        public NotificationReleaseEventArgs(StringId releaseMessage)
        {
            Message = releaseMessage;
        }
    }
}
