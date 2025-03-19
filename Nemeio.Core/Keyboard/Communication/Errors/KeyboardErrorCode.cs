namespace Nemeio.Core.Keyboard.Communication.Errors
{
    public enum KeyboardErrorCode : byte
    {
        Success = 0,
        Unexpected = 1,
        MalformedData = 2,
        FileSystemFailure = 3,
        State = 4,
        InvalidContent = 5,
        NotFound = 6,
        DrawConfiguration = 7,
        ProtectedConfiguration = 8,
        WrongCommunicationMode = 9,
        StreamDataProtocol = 10,
        SendDataNomenclature = 11,
        FirmwareUpdateFailed = 12,
        FirmwareUpdateNoFlash = 13,
        FirmwareUpdateVersionInvalid = 14,
        FirmwareUpdateWriteFailed = 15,
        BleChipCommunicationError = 16,
        BatteryNotReady = 17,
        BatteryFuelGauge = 18,
        DisplayNotReady = 19
    }
}
