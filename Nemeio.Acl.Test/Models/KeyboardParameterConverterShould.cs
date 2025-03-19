using System;
using FluentAssertions;
using NUnit.Framework;

namespace Nemeio.Acl.Test.Models
{
    //  FIXME
    /*[TestFixture]
    public class KeyboardParameterConverterShould
    {
        [Test]
        public void KeyboardParameterConverter_01_01_Parse_WithInvalidArraySize_ThrowsArgumentOutOfRangeException()
        {
            var inputByteArray = new byte[KeyboardParameterConverter.ByteArraySize - 2];

            Assert.Throws<ArgumentOutOfRangeException>(() => KeyboardParameterConverter.Parse(inputByteArray));
        }

        [Test]
        public void KeyboardParameterConverter_01_02_Parse_WithSample_WorksOk()
        {
            var payloadSample = new byte[KeyboardParameterConverter.ByteArraySize]
            {
                0, 0, 0, 12, 0, 0, 0, 120, 0, 0, 0, 12, 0, 0, 0, 60, 0, 0, 1, 44, 50, 15, 5, 5, 220, 0, 250, 1, 0, 3, 0, 0, 100, 3, 132, 20, 0, 200, 7, 8
            };

            var result = KeyboardParameterConverter.Parse(payloadSample);

            result.InactiveTime.Should().Be(12);
            result.SleepTime.Should().Be(120);
            result.InactiveTimeUSBDisconnected.Should().Be(12);
            result.SleepTimeUSBDisconnected.Should().Be(60);
            result.PowerOffTimeUSBDisconnected.Should().Be(300);
            result.LedPowerMaxLevel.Should().Be(50);
            result.LedPowerInactiveLevel.Should().Be(15);
            result.BrigthnessStep.Should().Be(5);
            result.ButtonLongPressDelay.Should().Be(1500);
            result.ButtonRepeatLongPressDelay.Should().Be(250);
            result.CleanRefreshPeriod.Should().Be(1);
            result.DisplayLowPowerDelay.Should().Be(3);
            result.DemoMode.Should().Be(false);
            result.LowBatteryBlinkOnDelayMs.Should().Be(100);
            result.LowBatteryBlinkOffDelayMs.Should().Be(900);
            result.LowBatteryLevelThresholdPercent.Should().Be(20);
            result.BleBlinkOnDelayMs.Should().Be(200);
            result.BleBlinkOffDelayMs.Should().Be(1800);
        }

        [Test]
        public void KeyboardParameterConverter_02_01_ToByteArray_WithNullParameter_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => KeyboardParameterConverter.ToByteArray(null));
        }

        [Test]
        public void KeyboardParameterConverter_02_02_ToByteArray_WithSample_WorksOk()
        {
            var keyboardParameters = new KeyboardParameters()
            {
                InactiveTime = 12,
                SleepTime = 60,
                InactiveTimeUSBDisconnected = 12,
                SleepTimeUSBDisconnected = 120,
                PowerOffTimeUSBDisconnected = 300,
                LedPowerMaxLevel = 50,
                LedPowerInactiveLevel = 15,
                BrigthnessStep = 5,
                ButtonLongPressDelay = 1500,
                ButtonRepeatLongPressDelay = 250,
                CleanRefreshPeriod = 1,
                DisplayLowPowerDelay = 3,
                DemoMode = false,
                LowBatteryBlinkOnDelayMs = 100,
                LowBatteryBlinkOffDelayMs = 900,
                LowBatteryLevelThresholdPercent = 20,
                BleBlinkOnDelayMs = 200,
                BleBlinkOffDelayMs = 1800
            };

            var result = KeyboardParameterConverter.ToByteArray(keyboardParameters);

            result.Length.Should().Be(KeyboardParameterConverter.ByteArraySize);
        }
    }*/
}
