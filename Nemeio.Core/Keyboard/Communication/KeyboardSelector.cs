using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Keyboard.Communication
{
    public class KeyboardSelector : IKeyboardSelector
    {
        private readonly ILogger _logger;

        public KeyboardSelector(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<KeyboardSelector>();
        }

        public Keyboard SelectKeyboard(IEnumerable<Keyboard> keyboards)
        {
            _logger.LogInformation($"SelectKeyboard : <{keyboards.Count()}> keyboards");

            var serialDevices = keyboards.Where(x => x.Communication == CommunicationType.Serial);
            if (serialDevices.Any())
            {
                var selectedDevice = serialDevices.First();

                _logger.LogInformation($"Select serial keyboard <{selectedDevice.Identifier}>");

                return selectedDevice;
            }

            var bluetoothLEDevices = keyboards.Where(x => x.Communication == CommunicationType.BluetoothLE);
            if (bluetoothLEDevices.Any())
            {
                var selectedDevice = bluetoothLEDevices.First();

                _logger.LogInformation($"Select bluetooth LE keyboard <{selectedDevice.Identifier}>");

                return selectedDevice;
            }

            _logger.LogInformation($"No keyboard selected");

            return null;
        }
    }
}
