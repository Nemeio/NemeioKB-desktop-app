using System.Collections.Generic;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Keyboard.GetLayouts
{
    public interface IGetLayoutsMonitor
    {
        IEnumerable<LayoutIdWithHash> AskLayoutIds();
    }
}
