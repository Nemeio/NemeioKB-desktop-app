using System;
using Nemeio.Core.Keyboard.Communication;

namespace Nemeio.Presentation.Menu.Connection
{
    public class ConnectionSection
    {
        public string Title { get; private set; }
        public CommunicationType Communication { get; private set; }
        public bool Visible { get; private set; }

        public ConnectionSection(bool visible, string title, CommunicationType communication)
        {
            Visible = visible;
            Title = title;
            Communication = communication;
        }
    }
}
