using System;
using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Settings
{
    public abstract class Setting<T> : ISetting<T>
    {
        private static readonly object _lock = new object();
        private T _value;

        protected readonly ILogger _logger;

        public T Value 
        {
            get => _value;
            set
            {
                lock(_lock)
                {
                    if (IsValid(value))
                    {
                        _value = value;

                        OnChanged?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        _logger.LogWarning($"Value <{value}> is not valid. We don't change current setting");
                    }
                }
            }
        }

        public event EventHandler OnChanged;

        public Setting(ILogger logger, T value)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _value = value;
        }

        public virtual bool IsValid(T value)
        {
            //  By default all value are allowed
            //  You can subclass if needed

            return true;
        }
    }
}
