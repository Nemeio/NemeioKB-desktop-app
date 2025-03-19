using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication;

namespace Nemeio.Keyboard.Communication.Adapters
{
    public abstract class KeyboardIOAdapter : IKeyboardIO
    {
        private readonly ILogger _logger;

        public virtual int BytesToRead { get; protected set;  }

        public KeyboardIOAdapter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<KeyboardIOAdapter>();
        }

        public event EventHandler OnDataReceived;

        public abstract Task ConnectAsync(string identifier);

        public abstract Task DisconnectAsync();

        public abstract int Read(byte[] buffer, int offset, int count);

        public abstract void Write(byte[] buffer, int offset, int count);

        protected void RaiseOnDataRecevied() => OnDataReceived?.Invoke(this, EventArgs.Empty);
    }
}
