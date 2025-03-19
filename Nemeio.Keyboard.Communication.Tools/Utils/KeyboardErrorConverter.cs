using System;
using Nemeio.Core.Errors;
using Nemeio.Core.Keyboard.Communication.Errors;

namespace Nemeio.Keyboard.Tools.Utils
{
    public class KeyboardErrorConverter : IKeyboardErrorConverter
    {
        public ErrorCode Convert(KeyboardErrorCode keyboardErrorCode)
        {
            switch (keyboardErrorCode)
            {
                case KeyboardErrorCode.Success:
                    return ErrorCode.Success;
                case KeyboardErrorCode.Unexpected:
                    return ErrorCode.AclKeyboardResponseUnexpected;
                case KeyboardErrorCode.MalformedData:
                    return ErrorCode.AclKeyboardResponseDataMalformed;
                case KeyboardErrorCode.FileSystemFailure:
                    return ErrorCode.AclKeyboardResponseFileSystemFailure;
                case KeyboardErrorCode.State:
                    return ErrorCode.AclKeyboardResponseStateInvalid;
                case KeyboardErrorCode.InvalidContent:
                    return ErrorCode.AclKeyboardResponseContentInvalid;
                case KeyboardErrorCode.NotFound:
                    return ErrorCode.AclKeyboardResponseNotFound;
                case KeyboardErrorCode.DrawConfiguration:
                    return ErrorCode.AclKeyboardResponseDrawConfiguration;
                case KeyboardErrorCode.ProtectedConfiguration:
                    return ErrorCode.AclKeyboardResponseConfigurationProtected;
                case KeyboardErrorCode.StreamDataProtocol:
                    return ErrorCode.AclKeyboardResponseStreamDataProtocolFailed;
                case KeyboardErrorCode.SendDataNomenclature:
                    return ErrorCode.AclKeyboardResponseSendDataNomenclatureFailed;
                case KeyboardErrorCode.FirmwareUpdateFailed:
                    return ErrorCode.AclKeyboardResponseFirmwareUpdateFailed;
                case KeyboardErrorCode.FirmwareUpdateNoFlash:
                    return ErrorCode.AclKeyboardResponseFirmwareUpdateMissingFlash;
                case KeyboardErrorCode.FirmwareUpdateVersionInvalid:
                    return ErrorCode.AclKeyboardResponseFirmwareUpdateVersionInvalid;
                case KeyboardErrorCode.FirmwareUpdateWriteFailed:
                    return ErrorCode.AclKeyboardResponseFirmwareUpdateWriteFailed;
                case KeyboardErrorCode.BleChipCommunicationError:
                    return ErrorCode.AclKeyboardResponseBleChipCommunicationError;
                case KeyboardErrorCode.BatteryNotReady:
                    return ErrorCode.AclKeyboardResponseBatteryIsNoReady;
                case KeyboardErrorCode.BatteryFuelGauge:
                    return ErrorCode.AclKeyboardResponseBatteryFuelGaugeFailed;
                case KeyboardErrorCode.DisplayNotReady:
                    return ErrorCode.AclKeyboardResponseDisplayNotReady;
                case KeyboardErrorCode.WrongCommunicationMode:
                    return ErrorCode.AclKeyboardResponseUnexpected;
                default:
                    throw new InvalidOperationException($"Unknown value <{keyboardErrorCode}>");
            }
        }
    }
}
