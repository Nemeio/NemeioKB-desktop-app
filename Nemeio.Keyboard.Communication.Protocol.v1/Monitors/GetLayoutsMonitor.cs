using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.GetLayouts;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class GetLayoutsMonitor : ReceiveDataMonitor<LayoutIdWithHash>, IGetLayoutsMonitor
    {
        private static object _lockMonitor = new object();

        public GetLayoutsMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.ReceiveData, this);
        }

        public IEnumerable<LayoutIdWithHash> AskLayoutIds()
        {
            lock (_lockMonitor)
            {
                var idWithHash = AskData();

                return idWithHash;
            }
        }

        public override IKeyboardCommand CreateStartCommand() => _commandFactory.CreateGetLayoutsStartCommand();
        public override IKeyboardCommand CreatePulpCommand(uint sizeToReceive, uint offset) => _commandFactory.CreateGetLayoutsPulpCommand(sizeToReceive, offset);
        public override IKeyboardCommand CreateEndCommand() => _commandFactory.CreateGetLayoutsEndCommand();
        public override IEnumerable<LayoutIdWithHash> ParseReceivedData(byte[] receivedData)
        {
            const int LayoutIdLength = 36;
            const int LayoutHashLength = 36;
            _logger.LogDebug(receivedData.ToString(), null);
            var idWithHashs = BufferSplit(receivedData, LayoutIdLength + LayoutHashLength)
                .Select(LayoutIdWithHash =>
                {
                    var layoutIdWithHashStr = Encoding.UTF8.GetString(LayoutIdWithHash);
                    return new LayoutIdWithHash(new LayoutId(layoutIdWithHashStr.Substring(0, LayoutIdLength)),
                        new LayoutHash(layoutIdWithHashStr.Substring(LayoutIdLength, LayoutHashLength)));

                });

            return idWithHashs;
        }

        private IEnumerable<byte[]> BufferSplit(byte[] buffer, int blockSize)
        {
            byte[][] blocks = new byte[(buffer.Length + blockSize - 1) / blockSize][];

            for (int i = 0, j = 0; i < blocks.Length; i++, j += blockSize)
            {
                blocks[i] = new byte[Math.Min(blockSize, buffer.Length - j)];
                Array.Copy(buffer, j, blocks[i], 0, blocks[i].Length);
            }

            return blocks;
        }
    }
}
