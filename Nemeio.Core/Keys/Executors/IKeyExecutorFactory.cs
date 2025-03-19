using System.Collections.Generic;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Keyboard.Nemeios;

namespace Nemeio.Core.Keys.Executors
{
    public interface IKeyExecutorFactory
    {
        IEnumerable<KeyExecutor> Create(INemeio nemeio, IEnumerable<KeySubAction> subActions);
    }
}
