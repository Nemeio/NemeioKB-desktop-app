using System;

namespace Nemeio.Core.Tools.Dispatcher
{
    public interface IDispatcher
    {
        void Invoke(Action action);
    }
}
