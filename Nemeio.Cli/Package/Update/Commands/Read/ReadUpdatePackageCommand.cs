using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Keyboards.Commands.Version;
using Nemeio.Core.FileSystem;
using Nemeio.Core.PackageUpdater.Firmware;

namespace Nemeio.Cli.Package.Update.Commands.Read
{
    internal sealed class ReadUpdatePackageCommand : Command
    {
        private readonly IFile _packageFile;
        private readonly ILogger _logger;

        public ReadUpdatePackageCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, CancellationTokenSource source, IFile packageFile) 
            : base(loggerFactory, outputWriter, source)
        {
            _logger = loggerFactory.CreateLogger<ReadUpdatePackageCommand>();
            _packageFile = packageFile ?? throw new ArgumentNullException(nameof(packageFile));
        }

        public override async Task ExecuteAsync()
        {
            try
            {
                var packageFile = new PackageUpdateFile(_packageFile);

                var package = await packageFile.LoadPackageAsync();

                //  In this case we not manage Waveform version
                var output = new VersionOutput(
                    stm: CreateVersionFromHeader(package.Stm32Header), 
                    nrf: CreateVersionFromHeader(package.NrfHeader), 
                    ite: CreateVersionFromHeader(package.IteHeader), 
                    waveform: string.Empty
                );

                _outputWriter.WriteObject(output);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Read update package failed");
            }
        }

        private string CreateVersionFromHeader(FirmwarePackageFirmwareHeader header) => $"{header.MajorVersion}.{header.MinorVersion}";
    }
}
