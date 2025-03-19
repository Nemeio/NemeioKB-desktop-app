using System;

namespace Nemeio.Core.Keyboard.Updates.Progress
{
    public interface IUpdateProgressMonitor
    {
        event EventHandler<InProgressUpdateProgressEventArgs> OnUpdateProgressChanged;
        event EventHandler<FailedUpdateProgressEventArgs> OnUpdateFailed;
        event EventHandler<RollbackUpdateProgressEventArgs> OnRollbackProgressChanged;
    }
}
