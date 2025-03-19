﻿namespace Nemeio.Core.Device
{
    public enum CommandId : byte
    {
        SendData = 0x00,
        KeepAlive = 0x01,
        ApplyConfig = 0x02,
        ConfigChanged = 0x03,
        KeyPressed = 0x04,
        LayoutIds = 0x05,
        DeleteConfig = 0x06,
        Battery = 0x09,
        Versions = 0x0A,
        ReceiveData = 0x0B,
        SetCommMode = 0x0C,
        SerialNumber = 0x0D,
        KeyboardParameters = 0x0E,
        FactoryReset = 0x0F,
        TechnicalError = 0x11,
        UpdateStatus = 0x12,
        CheckIfBatteryPresent = 0x6F,
        ExitFunctionalTests = 0x70,
        GetTestBenchId = 0x71,
        SetTestBenchId = 0x72,
        SetAdvertising = 0x73,
        GetPressedKeys = 0x74,
        DisplayCheckerBoard = 0x75,
        ClearScreen = 0x77,
        CheckComponents = 0x78,
        SetFrontLightPower = 0x79,
        GetBatteryElectricalStatus = 0x7A,
        SetLed = 0x7B,
        ExitElectricalTests = 0x7C,
        SetProvisionning = 0x7E,
    };
}
