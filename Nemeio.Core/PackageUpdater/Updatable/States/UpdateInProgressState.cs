using System;
using Nemeio.Core.Keyboard.Updates.Progress;

namespace Nemeio.Core.PackageUpdater.Updatable.States
{
    public class UpdateInProgressState : UpdateState
    {
        public UpdateProgress Progress { get; private set; } = new UpdateProgress(UpdateComponent.Unknown, 0);

        public void UpdateInstallProgress(UpdateProgress progress)
        {
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
        }

        public void UpdateInstallProgress(UpdateComponent component, int percent)
        {
            Progress = new UpdateProgress(component, percent);
        }
    }
}
