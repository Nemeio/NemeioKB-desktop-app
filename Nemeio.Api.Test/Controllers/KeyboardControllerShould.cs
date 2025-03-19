using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MvvmCross.Platform.IoC;
using Nemeio.Api.Dto.In.Events;
using Nemeio.Api.Dto.Out;
using Nemeio.Api.Keyboard.Parameters.Dto.Out;
using Nemeio.Core.Errors;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Battery;
using Nemeio.Core.Keyboard.FactoryReset;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.Parameters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Nemeio.Api.Test.Controllers
{
    public interface IApiKeyboard : INemeio, IParametersHolder, IBatteryHolder, IFactoryResetHolder { }

    [TestFixture]
    internal class KeyboardControllerShould : BaseControllerShould
    {
        private IKeyboardController _keyboardController;
        private KeyboardParameters _keyboardParameters;

        public override void IocSetup(IMvxIoCProvider iocProvider, ILoggerFactory loggerFactory)
        {
            _keyboardController = Mock.Of<IKeyboardController>();
            _keyboardParameters = new KeyboardParameters()
            {
                InactiveTime = 12,
                SleepTime = 60,
                InactiveTimeUSBDisconnected = 60,
                SleepTimeUSBDisconnected = 120,
                PowerOffTimeUSBDisconnected = 300,
                LedPowerMaxLevel = 50,
                LedPowerInactiveLevel = 15,
                BrigthnessStep = 5,
                ButtonLongPressDelay = 1500,
                ButtonRepeatLongPressDelay = 250,
                CleanRefreshPeriod = 1,
                DemoMode = false,
                LowBatteryBlinkOnDelayMs = 100,
                LowBatteryBlinkOffDelayMs = 900,
                LowBatteryLevelThresholdPercent = 20,
                BleBlinkOnDelayMs = 200,
                BleBlinkOffDelayMs = 1800,
                HighQualityPercent = false,
                BrightnessStepList = new List<byte>() { 0, 10, 20, 30, 40, 50, 60 }
            };

            Ioc.RegisterSingleton<IKeyboardController>(_keyboardController);
        }

        [Test]
        public async Task KeyboardController_GetKeyboardParameters_WhenKeyboardNotPlugged_ReturnApiKeyboardNotPlugged()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/keyboard/parameters";

            var response = await _client.GetAsync(url);
            await CheckRequestIsError<KeyboardParametersOutDto>(response, ErrorCode.ApiKeyboardNotPlugged);
        }

        [Test]
        public async Task KeyboardController_GetKeyboardParameters_ReturnSuccess()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/keyboard/parameters";

            var response = await _client.GetAsync(url);
            await CheckRequestIsError<KeyboardParametersOutDto>(response, ErrorCode.ApiKeyboardNotPlugged);
        }

        [Test]
        public async Task KeyboardController_GetKeyboardParameter_WhenKeyboardNotPlugged_ReturnApiKeyboardNotPlugged()
        {
            base.Setup();

            Test_CreateAndPlugKeyboard();

            var url = $"{GetServerUrl()}/api/keyboard/parameters";
            var response = await _client.GetAsync(url);
            var responseBody = await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
            var responseResult = responseBody.Result;

            responseResult.InactiveTime.Value.Should().Be(12);
            responseResult.SleepTime.Value.Should().Be(60);
            responseResult.PowerOffTimeUSBDisconnected.Value.Should().Be(300);
            responseResult.LedPowerMaxLevel.Value.Should().Be(50);
            responseResult.LedPowerInactiveLevel.Value.Should().Be(15);
            responseResult.BrigthnessStep.Value.Should().Be(5);
            responseResult.ButtonLongPressDelay.Value.Should().Be(1500);
            responseResult.ButtonRepeatLongPressDelay.Value.Should().Be(250);
            responseResult.CleanRefreshPeriod.Value.Should().Be(1);
            responseResult.DemoMode.Value.Should().Be(false);
            responseResult.LowBatteryBlinkOnDelayMs.Value.Should().Be(100);
            responseResult.LowBatteryBlinkOffDelayMs.Value.Should().Be(900);
            responseResult.LowBatteryLevelThresholdPercent.Value.Should().Be(20);
            responseResult.BleBlinkOnDelayMs.Value.Should().Be(200);
            responseResult.BleBlinkOffDelayMs.Value.Should().Be(1800);
            responseResult.HighQualityModifier.Value.Should().Be(false);
            responseResult.BrightnessStepList.Value.Should().BeEquivalentTo(new List<byte>() { 0, 10, 20, 30, 40, 50, 60 });
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_InactiveTime_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<uint>()
            { 
                Value = _keyboardParameters.SleepTime - 10
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.InactiveTime, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_InactiveTime_WhenHigherThanSleepTime_ReturnError()
        {
            base.Setup();

            var parameterInDto = new EventInDto<uint>()
            { 
                Value = _keyboardParameters.SleepTime 
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.InactiveTime, parameterInDtoJson);

            await CheckRequestIsError<KeyboardParametersOutDto>(response, ErrorCode.ApiPostKeyboardParametersInvalidParameters);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_SleepTime_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<uint>() { Value = 200 };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.SleepTime, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_SleepTime_WhenLowerThanInactiveTime_ReturnError()
        {
            base.Setup();

            var parameterInDto = new EventInDto<uint>()
            {
                Value = _keyboardParameters.InactiveTime
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.SleepTime, parameterInDtoJson);

            await CheckRequestIsError<KeyboardParametersOutDto>(response, ErrorCode.ApiPostKeyboardParametersInvalidParameters);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_SleepTime_WhenEqualToPowerManagementTimingMaxValue_ReturnSuccess()
        {
            base.Setup();

            const int PowerManagementTimingMaxValue = int.MaxValue / 1000000;

            var parameterInDto = new EventInDto<uint>()
            {
                Value = PowerManagementTimingMaxValue
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.SleepTime, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_SleepTime_WhenHigherThanPowerManagementTimingMaxValue_ReturnError()
        {
            base.Setup();

            const int PowerManagementTimingMaxValue = int.MaxValue / 1000000;

            var parameterInDto = new EventInDto<uint>()
            { 
                Value = PowerManagementTimingMaxValue + 1
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.SleepTime, parameterInDtoJson);

            await CheckRequestIsError<KeyboardParametersOutDto>(response, ErrorCode.ApiPostKeyboardParametersInvalidParameters);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_InactiveTimeUSBDisconnected_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<uint>()
            { 
                Value = _keyboardParameters.SleepTimeUSBDisconnected - 10 
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.InactiveTimeUSBDisconnected, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_InactiveTimeUSBDisconnectede_WhenLowerThanSleepTimeUSBDisconnected_ReturnError()
        {
            base.Setup();

            var parameterInDto = new EventInDto<uint>()
            {
                Value = _keyboardParameters.SleepTimeUSBDisconnected
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.InactiveTimeUSBDisconnected, parameterInDtoJson);

            await CheckRequestIsError<KeyboardParametersOutDto>(response, ErrorCode.ApiPostKeyboardParametersInvalidParameters);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_SleepTimeUSBDisconnected_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<uint>() 
            { 
                Value = _keyboardParameters.InactiveTimeUSBDisconnected + 1
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.SleepTimeUSBDisconnected, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_SleepTimeUSBDisconnected_WhenLowerThanInactiveTimeUSBDisconnected_ReturnError()
        {
            base.Setup();

            var parameterInDto = new EventInDto<uint>()
            {
                Value = _keyboardParameters.InactiveTimeUSBDisconnected
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.SleepTimeUSBDisconnected, parameterInDtoJson);

            await CheckRequestIsError<KeyboardParametersOutDto>(response, ErrorCode.ApiPostKeyboardParametersInvalidParameters);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_SleepTimeUSBDisconnected_WhenHigherThanPowerOffTimeUSBDisconnected_ReturnError()
        {
            base.Setup();

            var parameterInDto = new EventInDto<uint>()
            {
                Value = _keyboardParameters.PowerOffTimeUSBDisconnected
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.SleepTimeUSBDisconnected, parameterInDtoJson);

            await CheckRequestIsError<KeyboardParametersOutDto>(response, ErrorCode.ApiPostKeyboardParametersInvalidParameters);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_PowerOffTimeUSBDisconnected_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<uint>() 
            { 
                Value = _keyboardParameters.SleepTimeUSBDisconnected + 1
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.PowerOffTimeUSBDisconnected, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_PowerOffTimeUSBDisconnected_WhenLowerThanSleepTimeUSBDisconnected_ReturnError()
        {
            base.Setup();

            var parameterInDto = new EventInDto<uint>()
            {
                Value = _keyboardParameters.SleepTimeUSBDisconnected
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.PowerOffTimeUSBDisconnected, parameterInDtoJson);

            await CheckRequestIsError<KeyboardParametersOutDto>(response, ErrorCode.ApiPostKeyboardParametersInvalidParameters);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_PowerOffTimeUSBDisconnected_WhenEqualToPowerManagementTimingMaxValue_ReturnSuccess()
        {
            base.Setup();

            const int PowerManagementTimingMaxValue = int.MaxValue / 1000000;

            var parameterInDto = new EventInDto<uint>()
            {
                Value = PowerManagementTimingMaxValue
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.PowerOffTimeUSBDisconnected, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_PowerOffTimeUSBDisconnected_WhenHigherThanPowerManagementTimingMaxValue_ReturnError()
        {
            base.Setup();

            const int PowerManagementTimingMaxValue = int.MaxValue / 1000000;

            var parameterInDto = new EventInDto<uint>()
            {
                Value = PowerManagementTimingMaxValue + 1
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.PowerOffTimeUSBDisconnected, parameterInDtoJson);

            await CheckRequestIsError<KeyboardParametersOutDto>(response, ErrorCode.ApiPostKeyboardParametersInvalidParameters);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_LedPowerMaxLevel_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<byte>() 
            { 
                Value = 0
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.LedPowerMaxLevel, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_LedPowerInactiveLevel_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<ushort>() { Value = 95 };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.LedPowerInactiveLevel, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_BrightnessStep_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<ushort>() { Value = 200 };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.BrightnessStep, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_BrightnessStepList_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<List<byte>>() 
            { 
                Value = new List<byte>() 
                { 
                    0,
                    10,
                    20, 
                    100
                } 
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.BrightnessStepList, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_BrightnessStepList_WhenOneValueIsAboveMaximum_Failed()
        {
            base.Setup();

            var parameterInDto = new EventInDto<List<byte>>()
            {
                Value = new List<byte>()
                {
                    0,
                    10,
                    20,
                    110
                }
            };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.BrightnessStepList, parameterInDtoJson);

            await CheckRequestIsError<KeyboardParametersOutDto>(response, ErrorCode.ApiPostKeyboardParametersInvalidParameters);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_ButtonLongPressDelay_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<ushort>() { Value = 200 };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.ButtonLongPressDelay, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_ButtonRepeatLongPressDelay_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<ushort>() { Value = 200 };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.ButtonRepeatLongPressDelay, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_CleanRefreshPeriod_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<ushort>() { Value = 200 };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.CleanRefreshPeriod, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_DemoMode_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<ushort>() { Value = 200 };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.DemoMode, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_LowBatteryBlinkOnDelay_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<ushort>() { Value = 200 };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.LowBatteryBlinkOnDelay, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_LowBatteryBlinkOffDelay_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<ushort>() { Value = 200 };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.LowBatteryBlinkOffDelay, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_LowBatteryLevelThreshold_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<ushort>() { Value = 100 };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.LowBatteryLevelThreshold, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_BleBlinkOnDelay_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<ushort>() { Value = 200 };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.BleBlinkOnDelay, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_BleBlinkOffDelay_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<ushort>() { Value = 200 };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.BleBlinkOffDelay, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        [Test]
        public async Task KeyboardController_SetKeyboardParameter_BlackBackgroundCleanRefreshPeriod_ReturnSuccess()
        {
            base.Setup();

            var parameterInDto = new EventInDto<byte>() { Value = 10 };
            var parameterInDtoJson = JsonConvert.SerializeObject(parameterInDto);

            var response = await Test_SetKeyboardParameter(EventType.BlackBackgroundCleanRefreshPeriod, parameterInDtoJson);

            await CheckRequestIsSuccess<KeyboardParametersOutDto>(response);
        }

        private async Task<HttpResponseMessage> Test_SetKeyboardParameter(EventType eventType, string data)
        {
            Test_CreateAndPlugKeyboard();

            var url = $"{GetServerUrl()}/api/keyboard/parameters";

            var inDto = new KeyboardParameterInDto()
            {
                Type = eventType,
                Data = data
            };
            var inDtoJson = JsonConvert.SerializeObject(inDto);
            var inDtoJsonContent = CreateJsonContent(inDtoJson);

            return await _client.PostAsync(url, inDtoJsonContent);
        }

        [Test]
        public async Task KeyboardController_StartFactoryReset_WhenKeyboardUnplugged_ReturnError()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/keyboard/factoryReset";
            var response = await _client.PostAsync(url, null);

            await CheckRequestIsError<object>(response, ErrorCode.ApiKeyboardNotPlugged);
        }

        [Test]
        public async Task KeyboardController_StartFactoryReset_WhenKeyboardPlugged_ReturnSuccess()
        {
            base.Setup();

            //  We plug the keyboard
            Test_CreateAndPlugKeyboard();
            
            var url = $"{GetServerUrl()}/api/keyboard/factoryReset";
            var response = await _client.PostAsync(url, null);

            await CheckRequestIsSuccess<object>(response);
        }

        private void Test_CreateAndPlugKeyboard()
        {
            var mockNemeio = Mock.Of<IApiKeyboard>();
            Mock.Get(mockNemeio)
                .Setup(x => x.Parameters)
                .Returns(_keyboardParameters);

            Test_PlugKeyboard(_keyboardController, mockNemeio);
        }

        private void Test_PlugKeyboard(IKeyboardController keyboardController, IApiKeyboard nemeio)
        {
            Mock.Get(keyboardController)
                .Setup(mock => mock.Nemeio)
                .Returns(nemeio);

            Mock.Get(keyboardController)
                .Setup(mock => mock.Connected)
                .Returns(true);
        }
    }
}
