using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nemeio.Core.Icon;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.Services.Batteries;
using Nemeio.Core.Services.Layouts;
using Nemeio.Presentation.Menu.Synchronization;
using Nemeio.Presentation.Menu.Tools;

namespace Nemeio.Presentation.Menu
{
    public interface IApplicationMenu
    {
        ObservableValue<Core.Keyboard.Nemeios.INemeio> Keyboard { get; }
        ObservableValue<BatteryInformation> BatteryInformations { get; }
        ObservableValue<ApplicationIconType> Icon { get; }
        ObservableValue<ILayout> SelectedLayout { get;  }
        ObservableValue<IList<ILayout>> Layouts { get; }
        ObservableValue<bool> ApplicationAugmentedHidEnable { get; }
        ObservableValue<bool> Syncing { get; }
        ObservableValue<SynchronizationProgress> SyncProgression { get; }
        ObservableValue<PackageUpdateState> UpdateKind { get; }
        ObservableValue<System.Version> ApplicationVersion { get; }
        ObservableValue<Core.Systems.Applications.Application> ForegroundApplication { get; }

        IApplicationMenuUIDelegate UIDelegate { get; set; }

        event EventHandler OnFactoryResetStarted;
        event EventHandler OnKeyboardInitFailed;

        void Run();
        Task SelectLayoutAsync(ILayout layout);
    }
}
