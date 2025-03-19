using System;
using System.Threading.Tasks;
using Nemeio.Core.Keyboard.Updates.Progress;
using Nemeio.Core.PackageUpdater.Firmware;

namespace Nemeio.Core.Keyboard.Updates
{
    public interface IUpdateHolder
    {
        event EventHandler<InProgressUpdateProgressEventArgs> OnUpdateProgressChanged;
        event EventHandler<FailedUpdateProgressEventArgs> OnUpdateFailed;
        event EventHandler<RollbackUpdateProgressEventArgs> OnRollbackProgressChanged;

        Task UpdateAsync(IFirmware package);
    }
}
