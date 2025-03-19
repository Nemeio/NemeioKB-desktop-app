using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Keyboard.LayoutsIds;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Services.Layouts;
using Nemeio.Keyboard.Communication.Protocol.v1.Utils;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class LayoutsHashMonitor : ResponseMonitor, ILayoutHashMonitor
    {
        enum LayoutListResponseId : byte
        {
            LayoutCount = 0x00,
            LayoutId = 0x01
        }

        private const int GuidStringLength = 36;

        public LayoutsHashMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
               : base(loggerFactory, commandFactory, commandExecutor, errorConverter) 
        {
            commandExecutor.RegisterNotification(CommandId.LayoutIds, this);
        }

        public IEnumerable<LayoutHash> AskLayoutHashes()
        {
            _logger.LogInformation("AskLayoutHashes");

            var command = _commandFactory.CreateLayoutIdsCommand();
            var responses = ExecuteCommand(command);

            CheckResponsesAndThrowIfNeeded(responses);

            var firstFrameCount = responses.First().Frame.Payload.Copy(1, sizeof(int));
            var waitCount = KeyboardProtocolHelpers.ToInt32(firstFrameCount);

            if (waitCount <= 0)
            {
                return new List<LayoutHash>();
            }

            var hashes = responses
                .Skip(1)
                .Take(waitCount)
                .Select(x => new LayoutHash(x.Frame.Payload.Copy(1, GuidStringLength)))
                .ToList();

            return hashes;
        }

        private bool _mustCalcEnd = true;
        private int _countDown;

        public override bool CanRelease(IFrame frame, Exception exception)
        {
            var canRelease = false;

            if (exception != null)
            {
                canRelease = true;
            }
            else
            {
                if (_mustCalcEnd && frame.Payload[0] == (byte)LayoutListResponseId.LayoutCount)
                {
                    _mustCalcEnd = false;

                    var count = frame.Payload.Copy(1, sizeof(int));
                    _countDown = KeyboardProtocolHelpers.ToInt32(count);

                    canRelease = _countDown == 0;
                }
                else
                {
                    _countDown -= 1;

                    canRelease = _countDown == 0;
                }

                _logger.LogTrace($"LayoutsHashMonitor Count Down <{_countDown}> <{Thread.CurrentThread.ManagedThreadId}>");
                _logger.LogTrace($"LayoutsHashMonitor CanRelease <{canRelease}>");
            }

            return canRelease;
        }
    }
}
