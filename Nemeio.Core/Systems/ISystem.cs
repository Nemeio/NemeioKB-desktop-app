using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nemeio.Core.EventArguments;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems.Applications;
using Nemeio.Core.Systems.Sessions;
using Nemeio.Core.Systems.Sleep;
using Nemeio.Core.Tools;

namespace Nemeio.Core.Systems
{
    public interface ISystem : IStoppable
    {
        event EventHandler OnSelectedLayoutChanged;
        event EventHandler OnLayoutsChanged;
        event EventHandler OnForegroundApplicationChanged;
        event EventHandler OnSessionStateChanged;
        event EventHandler OnSleepModeChanged;
        event EventHandler<RemovedByDeletionEventArgs> OnRemovedByHidDeletion;


        SessionState SessionState { get; }
        OsLayoutId DefaultLayout { get; }
        OsLayoutId SelectedLayout { get; }
        IList<OsLayoutId> Layouts { get; }
        Application ForegroundApplication { get; }
        SleepMode SleepMode { get; }

        Task EnforceSystemLayoutAsync(OsLayoutId layoutId);
        void PressKeys(IList<string> keys);
        void NotifyCustomRemovedByHid(List<ILayout> deletedHid, List<ILayout> customLayoutsToDisable);
        void CheckForegroundApplication();
    }
}
