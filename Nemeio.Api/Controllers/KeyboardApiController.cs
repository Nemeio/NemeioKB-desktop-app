using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform;
using Nemeio.Api.Dto.In.Events;
using Nemeio.Api.Dto.Out;
using Nemeio.Api.Dto.Out.Battery;
using Nemeio.Api.Exceptions;
using Nemeio.Api.Keyboard.Parameters;
using Nemeio.Api.Keyboard.Parameters.Dto.Out;
using Nemeio.Core.Errors;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Battery;
using Nemeio.Core.Keyboard.FactoryReset;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.Nemeios.Proxy;
using Nemeio.Core.Keyboard.Parameters;
using Nemeio.Core.Services.Batteries;
using Newtonsoft.Json;

namespace Nemeio.Api.Controllers
{
    [Route("api/keyboard")]
    [ApiController]
    public class KeyboardApiController : DefaultController
    {
        private class ApiNemeioProxy : KeyboardProxy, IParametersHolder, IBatteryHolder, IFactoryResetHolder
        {
            private readonly IParametersHolder _parametersHolder;
            private readonly IBatteryHolder _batteryHolder;
            private readonly IFactoryResetHolder _factoryResetHolder;

            public KeyboardParameters Parameters => _parametersHolder.Parameters;
            public BatteryInformation Battery => _batteryHolder.Battery;

            public event EventHandler OnBatteryLevelChanged;

            public ApiNemeioProxy(IKeyboard keyboard) 
                : base(keyboard)
            {
                _parametersHolder = keyboard as IParametersHolder;
                _batteryHolder = keyboard as IBatteryHolder;
                _batteryHolder.OnBatteryLevelChanged += BatteryHolder_OnBatteryLevelChanged;
                _factoryResetHolder = keyboard as IFactoryResetHolder;
            }

            private void BatteryHolder_OnBatteryLevelChanged(object sender, EventArgs e) => OnBatteryLevelChanged?.Invoke(sender, e);

            public Task UpdateParametersAsync(KeyboardParameters parameters) => _parametersHolder.UpdateParametersAsync(parameters);

            public Task WantFactoryResetAsync() => _factoryResetHolder.WantFactoryResetAsync();

            public Task ConfirmFactoryResetAsync() => _factoryResetHolder.ConfirmFactoryResetAsync();

            public Task CancelFactoryResetAsync() => _factoryResetHolder.CancelFactoryResetAsync();

            public Task RefreshParametersAsync() => _parametersHolder.RefreshParametersAsync();
        }

        private readonly IKeyboardController _keyboardController;

        public readonly ILogger _logger;

        public KeyboardApiController()
            : base()
        {
            _logger = Mvx.Resolve<ILoggerFactory>().CreateLogger<KeyboardController>();
            _keyboardController = Mvx.Resolve<IKeyboardController>();
        }

        #region Parameters

        /// <summary>
        /// Allow user to retrieve keyboard's parameters. This method can return error if no keyboard is currently plugged. 
        /// </summary>
        /// <returns>KeyboardParametersOutDto</returns>
        [HttpGet("parameters")]
        public async Task<IActionResult> GetKeyboardParameters()
        {
            var keyboard = GetPluggedKeyboard();
            if (keyboard == null)
            {
                return ErrorResponse(ErrorCode.ApiKeyboardNotPlugged);
            }

            await keyboard.RefreshParametersAsync();

            var outDto = KeyboardParametersOutDto.FromModel(keyboard.Parameters);

            return SuccessResponse(outDto);
        }

        /// <summary>
        /// Allow user to change one keyboard's parameter. This method can return error if no keyboard is currently plugged.
        /// WARNING! Swagger auto-gen documentation is wrong. Please not send "InaciveTime" but the associated value: https://adeneo-embedded.atlassian.net/wiki/spaces/BLDLCK/pages/861339653/Gestion+des+v+nements
        /// </summary>
        /// <param name="eventInDto">Keyboard's parameter description</param>
        [HttpPost("parameters")]
        public async Task<IActionResult> SetKeyboardParameter([FromBody] KeyboardParameterInDto eventInDto)
        {
            var keyboard = GetPluggedKeyboard();
            if (keyboard == null)
            {
                return ErrorResponse(ErrorCode.ApiKeyboardNotPlugged);
            }

            var factory = new KeyboardParameterFactory();
            var parameters = keyboard.Parameters.CreateBackup();
            var parameter = factory.Create(parameters, eventInDto.Type);

            try
            {
                parameter.Update(eventInDto);

                await keyboard.UpdateParametersAsync(parameters);

                return SuccessResponse();
            }
            catch (ArgumentException exception)                 //  Invalid parameters
            {
                _logger.LogError(exception, "PostEvent");

                return ErrorResponse(ErrorCode.ApiPostKeyboardParametersInvalidParameters);
            }
            catch (InvalidOperationException exception)          //  No dispatcher found
            {
                _logger.LogError(exception, "PostEvent");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (InvalidCastException exception)              //  Format not valid (e.g. event type is equal to 14)
            {
                _logger.LogError(exception, "PostEvent");

                return ErrorResponse(ErrorCode.ApiPostKeyboardParametersInvalidFormat);
            }
            catch (KeyboardNotPluggedException exception)       //  No keyboard plugged
            {
                _logger.LogError(exception, "PostEvent");

                return ErrorResponse(ErrorCode.ApiKeyboardNotPlugged);
            }
            catch(JsonSerializationException exception)
            {
                return ErrorResponse(ErrorCode.ApiInvalidParameters);
            }
            catch (Exception exception)
            {
                return ErrorResponse(ErrorCode.ApiKeyboardNotPlugged);
            }
        }

        #endregion

        #region Battery

        /// <summary>
        /// Allow user to retrieve keyboard's battery information. This method can return error if no keyboard is currently plugged.
        /// </summary>
        /// <returns>BatteryInformationOutDto</returns>
        [HttpGet("battery")]
        public IActionResult GetKeyboardBattery()
        {
            var keyboard = GetPluggedKeyboard();
            if (keyboard == null)
            {
                return ErrorResponse(ErrorCode.ApiKeyboardNotPlugged);
            }

            var outDto = BatteryInformationOutDto.FromModel(keyboard.Battery);

            return SuccessResponse(outDto);
        }

        #endregion

        #region Factory Reset

        /// <summary>
        /// Allow user to start a factory reset on keyboard. This method can return error if no keyboard is currently plugged.
        /// </summary>
        /// <returns></returns>
        [HttpPost("factoryReset")]
        public IActionResult StartFactoryReset()
        {
            var keyboard = GetPluggedKeyboard();
            if (keyboard == null || keyboard.CommunicationType != Core.Keyboard.Communication.CommunicationType.Serial)
            {
                return ErrorResponse(ErrorCode.ApiKeyboardNotPlugged);
            }
            //  Run on task to no block configurator page
            Task.Run(async () => await keyboard.WantFactoryResetAsync());

            return SuccessResponse();
        }

        #endregion

        private ApiNemeioProxy GetPluggedKeyboard()
        {
            if (_keyboardController.Nemeio != null)
            {
                var proxy = KeyboardProxy.CastTo<ApiNemeioProxy>(_keyboardController.Nemeio);

                return proxy;
            }

            return null;
        }
    }
}
