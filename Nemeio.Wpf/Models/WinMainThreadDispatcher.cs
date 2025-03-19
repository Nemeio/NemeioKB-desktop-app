using System;
using Nemeio.Core.Tools.Dispatcher;

namespace Nemeio.Wpf.Models
{
    public sealed class WinMainThreadDispatcher : IDispatcher
    {
        public void Invoke(Action action) => App.Current.Dispatcher.Invoke(action);
    }
}
