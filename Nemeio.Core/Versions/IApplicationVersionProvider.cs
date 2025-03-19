using System;

namespace Nemeio.Core.Versions
{
    public interface IApplicationVersionProvider
    {
        Version GetVersion();
    }
}
