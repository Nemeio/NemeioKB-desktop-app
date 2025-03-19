using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Keyboard.Configurations
{
    public interface IConfigurationHolder
    {
        IScreen Screen { get; }
        IList<LayoutIdWithHash> LayoutIdWithHashs{ get; }
        LayoutId SelectedLayoutId { get; }

        event EventHandler OnSelectedLayoutChanged;

        Task StartSynchronizationAsync();
        Task EndSynchronizationAsync();
    }
}
