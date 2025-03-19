using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Keyboard.Communication.Protocol.v1.Utils;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public abstract class ReceiveDataMonitor<T> : ResponseMonitor
    {
        private uint _sizeToReceive = 0;
        private uint _offset;
        private List<byte> _receiveData;
        private static object _lockMonitor = new object();

        protected ReceiveDataMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter) 
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter) { }

        protected virtual IEnumerable<T> AskData()
        {
            lock (_lockMonitor)
            {
                Reset();

                ExecuteAndParseCommand(CreateStartCommand(), ParseStartPayload);

                //  Check whether data really needs to be collected. 
                //  Premature end if not

                if (_sizeToReceive <= 0)
                {
                    return Enumerable.Empty<T>();
                }

                IKeyboardCommand command;
                while ((command = CreateNewPulpCommand()) != null)
                {
                    ExecuteAndParseCommand(command, ParsePulpReceivePayload);
                }

                return ExecuteAndParseCommandWithResult(
                    CreateEndCommand(),
                    ParseStopPayload
                );
            }
        }

        public abstract IEnumerable<T> ParseReceivedData(byte[] receivedData);
        public abstract IKeyboardCommand CreateStartCommand();
        public abstract IKeyboardCommand CreatePulpCommand(uint sizeToReceive, uint offset);
        public abstract IKeyboardCommand CreateEndCommand();

        private void Reset()
        {
            _sizeToReceive = 0;
            _offset = 0;
            _receiveData = new List<byte>();
        }

        private IKeyboardCommand CreateNewPulpCommand()
        {
            if (_sizeToReceive <= _offset)
            {
                return null;
            }

            return CreatePulpCommand(_sizeToReceive, _offset);
        }

        #region Execute command

        private void ExecuteAndParseCommand(IKeyboardCommand command, Action<byte[]> parse)
        {
            ExecuteAndParseCommandWithResult(command, (val) =>
            {
                parse(val);

                return null;
            });
        }

        private IEnumerable<T> ExecuteAndParseCommandWithResult(IKeyboardCommand command, Func<byte[], IEnumerable<T>> parse)
        {
            var response = ExecuteCommand(command).ToList();
            if (response.Any(x => x.Exception != null))
            {
                //  FIXME: Use custom exception
                throw new AggregateException(response.Select(x => x.Exception));
            }

            return parse(response.First().Frame.Payload);
        }

        #endregion

        #region Parsing

        protected virtual void ParseStartPayload(byte[] payload)
        {
            CheckPayloadStatus(payload[0]);

            var count = payload.Copy(1, sizeof(int));

            _sizeToReceive = KeyboardProtocolHelpers.ToUInt32(count);
            _offset = 0;
        }

        private void ParsePulpReceivePayload(byte[] payload)
        {
            CheckPayloadStatus(payload[0]);
            int dataSize = payload.Length - 1;
            _receiveData.AddRange(payload.Copy(1, dataSize));
            _offset += (uint)dataSize;
        }

        private IEnumerable<T> ParseStopPayload(byte[] payload)
        {
            CheckPayloadStatus(payload[0]);

            var str = _receiveData.Select(l => l.ToString()).Aggregate((l1, l2) => $"{l1}, {l2}");
            var datas = ParseReceivedData(_receiveData.ToArray());

            _sizeToReceive = 0;
            _receiveData.Clear();

            return datas;
        }

        #endregion

        #region Tools

        /// <summary>
        /// Internal method to parse and check status of ReceiveData operation against Success.
        /// Throws an exception otherwise and log the status.
        /// </summary>
        /// <param name="status">Current operation status returned</param>
        /// <exception cref="InvalidOperationException">In case where status is not success</exception>
        private void CheckPayloadStatus(byte status)
        {
            if (status == (byte)KeyboardErrorCode.Success)
            {
                return;
            }

            throw new InvalidOperationException();
        }

        #endregion
    }
}
