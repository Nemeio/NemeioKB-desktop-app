using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Mono.Unix;
using Nemeio.Keyboard.Communication.Linux.CommandLine;

namespace Nemeio.Keyboard.Communication.Linux.Devices
{
    internal sealed class TtyProvider
    {
        private readonly ILogger _logger;

        public TtyProvider(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TtyProvider>();
        }

        public List<TtyDevice> GetDevices()
        {
            const string systemTtyPath = "/sys/class/tty";

            var devices = new List<TtyDevice>();
            var directories = Directory.GetDirectories(systemTtyPath);

            foreach (var directory in directories)
            {
                try
                {
                    var symlinkFileInfo = new UnixSymbolicLinkInfo(directory);
                    var directoryName = Path.GetFileName(directory);
                    var filePath = symlinkFileInfo.ContentsPath;

                    var device = new TtyDevice(directoryName, filePath);

                    devices.Add(device);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Failed to get symlink for directory <{directory}>");
                }
            }

            return devices;
        }
    }
}
