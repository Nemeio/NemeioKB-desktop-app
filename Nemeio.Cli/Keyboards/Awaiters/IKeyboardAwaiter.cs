using System;
using System.Threading;
using System.Threading.Tasks;
using Nemeio.Core.Keyboard;

namespace Nemeio.Cli.Keyboards
{
    internal interface IKeyboardAwaiter : IDisposable
    {
        Task<Nemeio> WaitKeyboardAsync(CancellationTokenSource cancellation);
    }
}
