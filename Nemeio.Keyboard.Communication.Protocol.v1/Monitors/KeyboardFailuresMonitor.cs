using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Keyboard.Communication.Protocol.v1.Utils;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public enum KeyboardFailuresCommandType : byte
    {
        Start = 0,
        Receive = 1,
        Stop = 2,
    }

    public class KeyboardFailuresMonitor : ReceiveDataMonitor<KeyboardFailure>, IKeyboardFailuresMonitor
    {
        private static object _lockAskKeyboardFailures = new object();

        public KeyboardFailuresMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter) 
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter) 
        {
            commandExecutor.RegisterNotification(CommandId.ReceiveData, this);
        }

        public IList<KeyboardFailure> AskKeyboardFailures()
        {
            lock (_lockAskKeyboardFailures)
            {
                var keyboardFailures = AskData();

                return keyboardFailures.ToList();
            }
        }

        public override IEnumerable<KeyboardFailure> ParseReceivedData(byte[] receivedData) => KeyboardProtocolHelpers.ParseSysFailLogDataFromLittleEndianStream(receivedData);
        public override IKeyboardCommand CreateStartCommand() => _commandFactory.CreateKeyboardFailuresStartCommand();
        public override IKeyboardCommand CreatePulpCommand(uint sizeToReceive, uint offset) => _commandFactory.CreateKeyboardFailuresPulpCommand(sizeToReceive, offset);
        public override IKeyboardCommand CreateEndCommand() => _commandFactory.CreateKeyboardFailuresEndCommand();

        protected override void ParseStartPayload(byte[] payload)
        {
            // Check for empty SysFailLog: will return FileSystemError if inquired without existing file dump (start up case)
            if (payload[0] == (byte)KeyboardErrorCode.FileSystemFailure)
            {
                return;
            }

            base.ParseStartPayload(payload);
        }
    }
}
