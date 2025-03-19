using Microsoft.Extensions.Logging;
using Nemeio.Core.Tools.Retry;
using Nemeio.Keyboard.Communication.Adapters;
using RJCP.IO.Ports;

namespace Nemeio.Keyboard.Communication.Linux.Adapters
{
    public sealed class LinuxSerialKeyboardIOAdapter : SerialKeyboardIOAdapter
    {
        public LinuxSerialKeyboardIOAdapter(ILoggerFactory loggerFactory, IRetryHandler retryHandler) 
            : base(loggerFactory, retryHandler) { }

        public override SerialPortStream CreateSerialPortStream(string identifier)
        {
            var serialPort = base.CreateSerialPortStream(identifier);

            serialPort.BaudRate = 921600;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.Handshake = Handshake.None;
            serialPort.DtrEnable = true;

            return serialPort;
        }
    }
}
