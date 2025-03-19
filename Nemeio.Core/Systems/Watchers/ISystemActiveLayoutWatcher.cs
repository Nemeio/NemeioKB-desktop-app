using System;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Tools;

namespace Nemeio.Core.Systems.Watchers
{
    public interface ISystemActiveLayoutWatcher : IStoppable
    {
        OsLayoutId CurrentSystemLayoutId { get; }

        event EventHandler OnSystemLayoutChanged;
        void CheckActiveAppLayout();
    }
}
