using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Applications;
using Nemeio.Core.Applications.Manifest;
using Nemeio.Core.DataModels;

namespace Nemeio.Core.Keyboard.Builds
{
    public class NemeioBuilder : INemeioBuilder
    {
        private readonly ILogger _logger;
        private readonly INemeioFactory _nemeioFactory;
        private readonly IApplicationManifest _applicationManifest;

        public NemeioBuilder(ILoggerFactory loggerFactory, INemeioFactory nemeioFactory, IApplicationManifest applicationManifest)
        {
            _logger = loggerFactory.CreateLogger<NemeioBuilder>();
            _nemeioFactory = nemeioFactory ?? throw new ArgumentNullException(nameof(nemeioFactory));
            _applicationManifest = applicationManifest ?? throw new ArgumentNullException(nameof(applicationManifest));
        }

        public async Task<Nemeio> BuildAsync(Keyboard keyboard)
        {
            Nemeio nemeio = null;

            if (keyboard == null)
            {
                throw new ArgumentNullException(nameof(keyboard));
            }

            //  Before trying to communicate with Nemeio
            //  we check if communication protocol is lower or equal to ours
            var applicationCommunicationProtocolVersion = new System.Version(ApplicationController.CommunicationProtocolVersion);

            if (keyboard.ProtocolVersion.Minor > applicationCommunicationProtocolVersion.Minor)
            {
                //  We cannot communicate with keyboard
                //  App must be updated

                throw new InitializationFailedException(keyboard.Identifier);
            }
            else if (keyboard.ProtocolVersion.Minor < applicationCommunicationProtocolVersion.Minor)
            {
                nemeio = _nemeioFactory.CreateUpdater(keyboard);
            }
            else
            {
                //  Application and keyboard have the same communication protocol
                //  we can start discussing.

                //  Check keyboard version to create runner or updater
                var versionChecker = _nemeioFactory.CreateVersionChecker(keyboard);

                try
                {
                    await versionChecker.InitializeAsync();

                    if (versionChecker.Versions == null)
                    {
                        throw new InvalidOperationException("Keyboard initialization failed");
                    }

                    var needKeyboardUpdate = false;
                    var manifest = _applicationManifest.FirmwareManifest;
                    if (manifest != null)
                    {
                        needKeyboardUpdate = NeedUpdate(versionChecker.Versions.Stm32, manifest.Cpu.Version) 
                                                || NeedUpdate(versionChecker.Versions.Nrf, manifest.BluetoothLE.Version) 
                                                || NeedUpdate(versionChecker.Versions.Ite, manifest.Ite.Version);
                    }

                    await versionChecker.StopAsync();
                    await versionChecker.DisconnectAsync();

                    if (needKeyboardUpdate)
                    {
                        nemeio = _nemeioFactory.CreateUpdater(keyboard);
                    }
                    else
                    {
                        nemeio = _nemeioFactory.CreateRunner(keyboard);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Init checker failed");

                    nemeio = null;

                    throw new InitializationFailedException(keyboard.Identifier);
                }
            }

            return nemeio;
        }

        private bool NeedUpdate(VersionProxy current, System.Version toCompare)
        {
            var currentVersion = new VersionProxy(current);
            var toCompareVersion = new VersionProxy(toCompare);

            return toCompareVersion.IsHigherThan(currentVersion);
        }
    }
}
