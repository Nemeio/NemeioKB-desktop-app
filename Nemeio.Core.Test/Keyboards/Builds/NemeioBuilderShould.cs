using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Applications;
using Nemeio.Core.Applications.Manifest;
using Nemeio.Core.DataModels;
using Nemeio.Core.Keyboard.Builds;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Tools.Retry;
using NUnit.Framework;

namespace Nemeio.Core.Test.Keyboards.Builds
{
    [TestFixture]
    public class NemeioBuilderShould
    {
        private class TestableNemeio : Keyboard.Nemeio
        {
            public bool InitKeyboardAsyncCalled { get; private set; }
            public bool StopCalled { get; private set; }

            public TestableNemeio(ILoggerFactory loggerFactory, string identifier, System.Version protocolVersion, CommunicationType type, IKeyboardCommandExecutor commandExecutor, IMonitorFactory monitorFactory, IKeyboardCrashLogger crashLogger, IRetryHandler retryHandler, FirmwareVersions versions)
                : base(loggerFactory, identifier, protocolVersion, type, commandExecutor, monitorFactory, crashLogger, retryHandler)
            {
                _stateMachine.Configure(NemeioState.Connected)
                    .Permit(NemeioTrigger.Initialize, NemeioState.Init)
                    .Permit(NemeioTrigger.KeyboardUnplugged, NemeioState.Disconnecting);

                _stateMachine.Configure(NemeioState.Init)
                    .SubstateOf(NemeioState.Connected)
                    .Permit(NemeioTrigger.KeyboardInitialized, NemeioState.Ready)
                    .Permit(NemeioTrigger.KeyboardUnplugged, NemeioState.Disconnecting)
                    .OnEntryAsync(DummyInit);

                _stateMachine.Configure(NemeioState.Ready)
                    .SubstateOf(NemeioState.Connected);

                _stateMachine.Configure(NemeioState.Disconnecting);

                Versions = versions;
            }

            public override void Stop()
            {
                base.Stop();

                StopCalled = true;
            }

            private async Task DummyInit()
            {
                await InitKeyboardAsync();

                await _stateMachine.FireAsync(NemeioTrigger.KeyboardInitialized);
            }

            protected override async Task InitKeyboardAsync()
            {
                await Task.Yield();

                InitKeyboardAsyncCalled = true;
            }
        }

        [Test]
        public void NemeioBuilder_Build_WithNullParameter_Throws()
        {
            var loggerFactory = new LoggerFactory();
            var nemeioFactory = Mock.Of<INemeioFactory>();
            var applicationManifest = Mock.Of<IApplicationManifest>();

            var builder = new NemeioBuilder(loggerFactory, nemeioFactory, applicationManifest);

            Assert.ThrowsAsync<ArgumentNullException>(() => builder.BuildAsync(null));
        }

        [Test]
        public async Task NemeioBuilder_Build_WithNeedUpdateKeyboard_Ok()
        {
            var identifier = "myId";
            var createUpdaterCalled = false;
            var createVersionCheckerCalled = false;

            var nemeioVersions = new FirmwareVersions()
            {
                Stm32 = new VersionProxy("1.0"),
                Nrf = new VersionProxy("1.0"),
                Ite = new VersionProxy("1.0"),
                Waveform = string.Empty
            };

            var loggerFactory = new LoggerFactory();
            var keyboard = new Keyboard.Keyboard(identifier, new Version(ApplicationController.CommunicationProtocolVersion), CommunicationType.Serial, Mock.Of<IKeyboardIO>());
            var testableVersionCheckerNemeio = new TestableNemeio(loggerFactory, identifier, keyboard.ProtocolVersion, It.IsAny<CommunicationType>(), Mock.Of<IKeyboardCommandExecutor>(), Mock.Of<IMonitorFactory>(), Mock.Of<IKeyboardCrashLogger>(), Mock.Of<IRetryHandler>(), nemeioVersions);
            var testableUpdaterNemeio = new TestableNemeio(loggerFactory, identifier, keyboard.ProtocolVersion, It.IsAny<CommunicationType>(), Mock.Of<IKeyboardCommandExecutor>(), Mock.Of<IMonitorFactory>(), Mock.Of<IKeyboardCrashLogger>(), Mock.Of<IRetryHandler>(), nemeioVersions);

            var nemeioFactory = Mock.Of<INemeioFactory>();
            Mock.Get(nemeioFactory)
                .Setup(x => x.CreateVersionChecker(It.IsAny<Keyboard.Keyboard>()))
                .Returns(testableVersionCheckerNemeio)
                .Callback(() => createVersionCheckerCalled = true);

            Mock.Get(nemeioFactory)
                .Setup(x => x.CreateUpdater(It.IsAny<Keyboard.Keyboard>()))
                .Returns(testableUpdaterNemeio)
                .Callback(() => createUpdaterCalled = true);

            var applicationManifest = Mock.Of<IApplicationManifest>();
            Mock.Get(applicationManifest)
                .Setup(x => x.FirmwareManifest)
                .Returns(new FirmwareManifest()
                {
                    Cpu = new FirmwareInformation(new Version("2.0"), "cpu.bin"),
                    BluetoothLE = new FirmwareInformation(new Version("2.0"), "ble.bin"),
                    Ite = new FirmwareInformation(new Version("2.0"), "ite.bin"),
                });

            var builder = new NemeioBuilder(loggerFactory, nemeioFactory, applicationManifest);

            var nemeio = await builder.BuildAsync(keyboard);

            createVersionCheckerCalled.Should().BeTrue();
            createUpdaterCalled.Should().BeTrue();
            testableVersionCheckerNemeio.StopCalled.Should().BeTrue();

            nemeio.Should().NotBeNull();
        }

        [Test]
        public async Task NemeioBuilder_Build_WithNoNeedUpdateKeyboard_Ok()
        {
            var identifier = "myId";
            var createRunnerCalled = false;
            var createVersionCheckerCalled = false;

            var nemeioVersions = new FirmwareVersions()
            {
                Stm32 = new VersionProxy("2.0"),
                Nrf = new VersionProxy("2.0"),
                Ite = new VersionProxy("2.0"),
                Waveform = string.Empty
            };

            var loggerFactory = new LoggerFactory();
            var keyboard = new Keyboard.Keyboard(identifier, new Version(ApplicationController.CommunicationProtocolVersion), CommunicationType.Serial, Mock.Of<IKeyboardIO>());
            var testableVersionCheckerNemeio = new TestableNemeio(loggerFactory, identifier, keyboard.ProtocolVersion, It.IsAny<CommunicationType>(), Mock.Of<IKeyboardCommandExecutor>(), Mock.Of<IMonitorFactory>(), Mock.Of<IKeyboardCrashLogger>(), Mock.Of<IRetryHandler>(), nemeioVersions);
            var testableRunnerNemeio = new TestableNemeio(loggerFactory, identifier, keyboard.ProtocolVersion, It.IsAny<CommunicationType>(), Mock.Of<IKeyboardCommandExecutor>(), Mock.Of<IMonitorFactory>(), Mock.Of<IKeyboardCrashLogger>(), Mock.Of<IRetryHandler>(), nemeioVersions);

            var nemeioFactory = Mock.Of<INemeioFactory>();
            Mock.Get(nemeioFactory)
                .Setup(x => x.CreateVersionChecker(It.IsAny<Keyboard.Keyboard>()))
                .Returns(testableVersionCheckerNemeio)
                .Callback(() => createVersionCheckerCalled = true);

            Mock.Get(nemeioFactory)
                .Setup(x => x.CreateRunner(It.IsAny<Keyboard.Keyboard>()))
                .Returns(testableRunnerNemeio)
                .Callback(() => createRunnerCalled = true);

            var applicationManifest = Mock.Of<IApplicationManifest>();
            Mock.Get(applicationManifest)
                .Setup(x => x.FirmwareManifest)
                .Returns(new FirmwareManifest()
                {
                    Cpu = new FirmwareInformation(new Version("2.0"), "cpu.bin"),
                    BluetoothLE = new FirmwareInformation(new Version("2.0"), "ble.bin"),
                    Ite = new FirmwareInformation(new Version("2.0"), "ite.bin"),
                });

            var builder = new NemeioBuilder(loggerFactory, nemeioFactory, applicationManifest);

            var nemeio = await builder.BuildAsync(keyboard);

            createVersionCheckerCalled.Should().BeTrue();
            createRunnerCalled.Should().BeTrue();
            testableVersionCheckerNemeio.StopCalled.Should().BeTrue();

            nemeio.Should().NotBeNull();
        }
    }
}
