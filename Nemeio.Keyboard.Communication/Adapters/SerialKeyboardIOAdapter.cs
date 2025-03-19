using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Tools.Retry;
using RJCP.IO.Ports;

namespace Nemeio.Keyboard.Communication.Adapters
{
    public class SerialKeyboardIOAdapter : KeyboardIOAdapter
    {
        private const int ReadTimeout = 5000;
        private const int WriteTimeout = 5000;
        private const int ConnectionMaxRetryCount = 3;

        private readonly ILogger _logger;
        private readonly IRetryHandler _retryHandler;

        private TimeSpan TimeBeforeOpenPort = new TimeSpan(0, 0, 0, 0, 200);

        private SerialPortStream _serialPort;

        public override int BytesToRead 
        { 
            get => _serialPort?.BytesToRead ?? 0; 
            protected set 
            { 
                // Nothing to do here 
            }
        }

        public SerialKeyboardIOAdapter(ILoggerFactory loggerFactory, IRetryHandler retryHandler)
            : base(loggerFactory) 
        {
            _logger = loggerFactory.CreateLogger<SerialKeyboardIOAdapter>();
            _retryHandler = retryHandler ?? throw new ArgumentNullException(nameof(retryHandler));
        }

        public override async Task ConnectAsync(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            var connectAction = new AsyncRetryAction("SerialConnection", ConnectionMaxRetryCount, () => TryConnectAsync(identifier));

            try
            {
                await _retryHandler.ExecuteAsync(connectAction);
            }
            catch (RetryFailedException exception)
            {
                _logger.LogError(exception, "Connect with retry failed");

                throw exception.InnerException;
            }
        }

        private async Task TryConnectAsync(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            //  We always wait a bit before opening a new port
            //  because if we try to stop and re-open the same port
            //  the system can refuse it with error
            //  System.UnauthorizedAccessException: Access Denied: COMX
            //  https://social.msdn.microsoft.com/Forums/en-US/22795cd9-1a75-48e5-9345-56a611cf399d/serialportopen-throws-unauthorized-access-exception?forum=netfxbcl

            await Task.Delay(TimeBeforeOpenPort);

            _serialPort = CreateSerialPortStream(identifier);

            _serialPort.DataReceived += OnSerialDataReceived;
            _serialPort.Open();
        }

        public virtual SerialPortStream CreateSerialPortStream(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            var serialPort = new SerialPortStream(identifier)
            {
                ReadTimeout = ReadTimeout,
                WriteTimeout = WriteTimeout
            };

            return serialPort;
        }

        public override async Task DisconnectAsync()
        {
            await Task.Yield();

            _logger.LogInformation($"SerialKeyboardIOAdapter DisconnectAsync");

            if (_serialPort != null)
            {
                _serialPort.DataReceived -= OnSerialDataReceived;

                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
                
                if (!_serialPort.IsDisposed)
                {
                    _serialPort.Dispose();
                }

                _serialPort = null;

                _logger.LogInformation($"SerialKeyboardIOAdapter Closed");
            }
            else 
            {
                _logger.LogWarning($"SerialKeyboardIOAdapter Try to closed not open / created port");
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _serialPort.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _serialPort.Write(buffer, offset, count);
        }

        #region Events

        private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e) => RaiseOnDataRecevied();

        #endregion
    }
}
