using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Tools;

namespace Nemeio.Keyboard.Communication.Linux.CommandLine
{
    internal sealed class LsSysClassTtyCommandLineExecutor
    {
        private readonly ICommandLineExecutor _commandLineExecutor;

        public LsSysClassTtyCommandLineExecutor(ICommandLineExecutor commandLineExecutor)
        {
            _commandLineExecutor = commandLineExecutor ?? throw new ArgumentNullException(nameof(commandLineExecutor));
        }

        public IList<TtyDevice> GetDevices()
        {
            var lsResult = _commandLineExecutor.Execute("ls", "-l /sys/class/tty");

            //  Split be \n
            var result = lsResult.Split('\n').ToList();

            //  Discard first line (e.g. total X)
            result.RemoveAt(0);

            //  Last line can be empty
            //  Remove if needed
            if (string.IsNullOrEmpty(result.Last()))
            {
                result.RemoveAt(result.Count - 1);
            }

            var devices = new List<TtyDevice>();

            foreach (var data in result)
            {
                var device = Map(data);

                devices.Add(device);
            }

            return devices;
        }

        /// <summary>
        /// Map string data to TtyDevice object
        /// </summary>
        /// <param name="data">(e.g. "lrwxrwxrwx 1 root root 0 févr. 21 13:00 ttyACM0 -> ../../devices/pci0000:00/0000:00:06.0/usb1/1-2/1-2:1.0/tty/ttyACM0")</param>
        private TtyDevice Map(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            var splitData = data.Split(' ').ToList();

            var name = splitData[8];
            var path = splitData[10];

            var device = new TtyDevice(name, path);

            return device;
        }
    }
}
