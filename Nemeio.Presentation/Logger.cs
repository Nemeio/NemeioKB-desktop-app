using System;
using System.IO;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Nemeio.Presentation
{
    public static class Logger
    {
        private static readonly bool UseUDPLogger = true;
        public static ILoggerFactory GetLoggerFactory()
        {
            //  Need to create automatically folder at startup: follow up IDocument path definition
            var logFolderPath = NemeioConstants.LogPath;

            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }

            var frameworkSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);

            var loggerFactory = new LoggerFactory();
            var loggerFilename = NemeioConstants.LogFileName + "-" + Environment.UserName + "-" + NemeioConstants.LogExtension;
            var serilogger = UseUDPLogger ? new LoggerConfiguration()
                .MinimumLevel.Verbose().WriteTo.Udp("localhost", 7071, AddressFamily.InterNetworkV6)
                .WriteTo.Debug()
                .MinimumLevel.Override("Nemeio", frameworkSwitch)
                .WriteTo.File(
                    path: Path.Combine(logFolderPath, loggerFilename),
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.ffff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day
                    )
                .CreateLogger()
                :
                 new LoggerConfiguration()
                .WriteTo.Debug()
                .MinimumLevel.Override("Nemeio", frameworkSwitch)
                .WriteTo.File(
                    path: Path.Combine(logFolderPath, loggerFilename),
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.ffff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day
                    )
                .CreateLogger(); //NOSONAR Historic Extension maintained Up To Date

            loggerFactory.AddSerilog(serilogger);
            return loggerFactory;
        }
    }
}
