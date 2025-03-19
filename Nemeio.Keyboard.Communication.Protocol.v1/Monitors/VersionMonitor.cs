using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.DataModels;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Keyboard.Version;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class VersionMonitor : ResponseMonitor, IVersionMonitor
    {
        private const string DefaultVersionErrorString = "0.0";
        private const int VersionLength = 16;
        private const char VersionEscapeChar = '\0';
        public const int PayloadSize = 67;

        public VersionMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.Versions, this);
        }

        public FirmwareVersions AskVersions()
        {
            var versionCommand = _commandFactory.CreateVersionCommand();
            var responses = ExecuteCommand(versionCommand);

            CheckResponsesAndThrowIfNeeded(responses);

            const int ErrorCodeSize = 1;

            var index = 0;
            var version = new FirmwareVersions();
            var payload = responses.First().Frame.Payload;



            if (payload[index] == (byte)KeyboardErrorCode.Success)
            {
                version.Stm32 = GetVersionOf(payload, index + ErrorCodeSize);
            }
            index += ErrorCodeSize;
            index += VersionLength;

            if (payload[index] == (byte)KeyboardErrorCode.Success)
            {
                version.Nrf = GetVersionOf(payload, index + ErrorCodeSize);
            }
            else
            { version.Nrf = new VersionProxy(DefaultVersionErrorString); }
            index += ErrorCodeSize;
            index += VersionLength;

            if (payload[index] == (byte)KeyboardErrorCode.Success)
            {
                index += ErrorCodeSize;

                var ite = GetStringVersion(payload, index);
                if (!string.IsNullOrEmpty(ite))
                {
                    version.Ite = ParseIteVersion(ite);
                    version.ScreenType = ParseScreenType(ite);
                }
                else
                {
                    version.Ite = new VersionProxy(DefaultVersionErrorString);
                    version.ScreenType = ScreenType.Undefined;
                }

                index += VersionLength;
                version.Waveform = GetStringVersion(payload, index);
            }

            return version;
        }

        private string GetStringVersion(byte[] payload, int index)
        {
            var subPart = payload.SubArray(index, VersionLength);

            return Encoding.UTF8.GetString(subPart, 0, subPart.Length)
                .Split(VersionEscapeChar)
                .First();
        }

        private VersionProxy GetVersionOf(byte[] payload, int index)
        {
            var stringVersion = GetStringVersion(payload, index);

            return new VersionProxy(stringVersion);
        }

        private ScreenType ParseScreenType(string iteVersion)
        {
            var screenType = iteVersion.EndsWith("T6") ? ScreenType.Holitech : ScreenType.Eink;

            return screenType;
        }

        private VersionProxy ParseIteVersion(string iteVersion)
        {

            //  e.g. LDLC_v.0.8
            var versionPrefix = "LDLC_v.";
            if (string.IsNullOrEmpty(iteVersion) || iteVersion.Length < versionPrefix.Count())
                return new VersionProxy(DefaultVersionErrorString);

            var computedVersion = iteVersion.Remove(0, versionPrefix.Count());

            return new VersionProxy(computedVersion);
        }
    }
}
