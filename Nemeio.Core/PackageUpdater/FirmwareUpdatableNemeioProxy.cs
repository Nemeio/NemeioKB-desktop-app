using System;
using System.Threading.Tasks;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.Nemeios.Proxy;
using Nemeio.Core.Keyboard.Updates;
using Nemeio.Core.Keyboard.Updates.Progress;
using Nemeio.Core.PackageUpdater.Firmware;

namespace Nemeio.Core.PackageUpdater
{
    public sealed class FirmwareUpdatableNemeioProxy : KeyboardProxy, IUpdateHolder
    {
        private readonly IUpdateHolder _updateHolder;

        public event EventHandler<FailedUpdateProgressEventArgs> OnUpdateFailed;
        public event EventHandler<RollbackUpdateProgressEventArgs> OnRollbackProgressChanged;
        public event EventHandler<InProgressUpdateProgressEventArgs> OnUpdateProgressChanged;

        public FirmwareUpdatableNemeioProxy(INemeio nemeio) 
            : base(nemeio)
        {
            _updateHolder = nemeio as IUpdateHolder;
            _updateHolder.OnUpdateProgressChanged += UpdateHolder_OnUpdateProgressChanged;
            _updateHolder.OnRollbackProgressChanged += UpdateHolder_OnRollbackProgressChanged;
            _updateHolder.OnUpdateFailed += UpdateHolder_OnUpdateFailed;
        }

        private void UpdateHolder_OnUpdateFailed(object sender, FailedUpdateProgressEventArgs e)
        {
            OnUpdateFailed?.Invoke(sender, e);
        }

        private void UpdateHolder_OnRollbackProgressChanged(object sender, RollbackUpdateProgressEventArgs e)
        {
            OnRollbackProgressChanged?.Invoke(sender, e);
        }

        private void UpdateHolder_OnUpdateProgressChanged(object sender, InProgressUpdateProgressEventArgs e)
        {
            OnUpdateProgressChanged?.Invoke(sender, e);
        }

        public Task UpdateAsync(IFirmware package) => _updateHolder.UpdateAsync(package);
    }
}
