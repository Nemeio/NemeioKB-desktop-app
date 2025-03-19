using System;

namespace Nemeio.Core.Settings
{
    public interface ISetting<T>
    {
        event EventHandler OnChanged;

        T Value { get; set;  }
    }
}
