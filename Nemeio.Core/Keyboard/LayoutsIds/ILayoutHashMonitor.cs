using System;
using System.Collections.Generic;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Keyboard.LayoutsIds
{
    [Obsolete("This interface is obsolete. Please use IGetLayouts instead.")]
    public interface ILayoutHashMonitor
    {
        IEnumerable<LayoutHash> AskLayoutHashes();
    }
}
