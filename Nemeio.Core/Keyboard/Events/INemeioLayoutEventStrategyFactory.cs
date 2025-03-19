using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.Sessions.Strategies;

namespace Nemeio.Core.Keyboard.Sessions
{
    public interface INemeioLayoutEventStrategyFactory
    {
        INemeioLayoutEventStrategy CreateStrategy(INemeio nemeio, ILoggerFactory loggerFactory);
    }
}
