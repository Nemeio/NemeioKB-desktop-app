using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Communication.Exceptions;
using Nemeio.Core.Keyboard.CommunicationMode;
using Nemeio.Core.Keyboard.Configurations.Add;
using Nemeio.Core.Keyboard.Configurations.Apply;
using Nemeio.Core.Keyboard.Configurations.Changed;
using Nemeio.Core.Keyboard.Configurations.Delete;
using Nemeio.Core.Keyboard.Errors;
using Nemeio.Core.Keyboard.FactoryReset;
using Nemeio.Core.Keyboard.KeepAlive;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Keys;
using Nemeio.Core.Keyboard.LayoutsIds;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Nemeios.Updater;
using Nemeio.Core.Keyboard.Parameters;
using Nemeio.Core.Keyboard.SerialNumber;
using Nemeio.Core.Keyboard.Updates;
using Nemeio.Core.Keyboard.Updates.Progress;
using Nemeio.Core.Keyboard.Version;
using Nemeio.Core.PackageUpdater.Firmware;
using Nemeio.Core.Tools.Retry;
using NUnit.Framework;

namespace Nemeio.Core.Test.Keyboards.Nemeios
{
    [TestFixture]
    public class NemeioUpdaterShould
    {
        private class TestableKeyboardCommunicationException : KeyboardCommunicationException { }

        private ILoggerFactory _loggerFactory;
        private string _identifier;
        private CommunicationType _type;
        private IKeyboardCommandExecutor _commandExecutor;
        private IMonitorFactory _monitorFactory;
        private IKeyboardCrashLogger _crashLogger;

        private IVersionMonitor _versionMonitor;
        private ISerialNumberMonitor _serialNumberMonitor;
        private IKeyboardFailuresMonitor _keyboardFailuresMonitor;
        private IParametersMonitor _parametersMonitor;
        private ICommunicationModeMonitor _communicationModeMonitor;
        private IFactoryResetMonitor _factoryResetMonitor;
        private IUpdateMonitor _updateMonitor;
        private IUpdateProgressMonitor _updateProgressMonitor;
        private IRetryHandler _retryHandler;

        private NemeioUpdater _updater;

        public static Array KeyboardErrorCodes() => Enum.GetValues(typeof(KeyboardErrorCode));

        [SetUp]
        public void SetUp()
        {
            _loggerFactory = new LoggerFactory();
            _identifier = "ThisIsMyTestIdentifier";
            _type = CommunicationType.Serial;
            _commandExecutor = Mock.Of<IKeyboardCommandExecutor>();

            _versionMonitor = Mock.Of<IVersionMonitor>();
            _serialNumberMonitor = Mock.Of<ISerialNumberMonitor>();
            _keyboardFailuresMonitor = Mock.Of<IKeyboardFailuresMonitor>();
            _parametersMonitor = Mock.Of<IParametersMonitor>();
            _communicationModeMonitor = Mock.Of<ICommunicationModeMonitor>();
            _factoryResetMonitor = Mock.Of<IFactoryResetMonitor>();
            _updateMonitor = Mock.Of<IUpdateMonitor>();
            _updateProgressMonitor = Mock.Of<IUpdateProgressMonitor>();
            _retryHandler = Mock.Of<IRetryHandler>();

            _monitorFactory = Mock.Of<IMonitorFactory>();
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateVersionMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_versionMonitor);
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateSerialNumberMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_serialNumberMonitor);
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateKeyboardFailuresMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_keyboardFailuresMonitor);
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateParametersMonitor(new Version("2.0"), It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_parametersMonitor);
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateCommunicationModeMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_communicationModeMonitor);
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateFactoryResetMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_factoryResetMonitor);
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateUpdateMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_updateMonitor);
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateUpdateProgressMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_updateProgressMonitor);

            _crashLogger = Mock.Of<IKeyboardCrashLogger>();
            _updater = new NemeioUpdater(_loggerFactory, _identifier, new Version("2.0"), _type, _commandExecutor, _monitorFactory, _crashLogger, _retryHandler);
        }

        [Test]
        public void NemeioUpdater_UpdateAsync_WithNullParameters_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _updater.UpdateAsync(null));
        }

        [TestCaseSource("KeyboardErrorCodes")]
        public void NemeioUpdate_UpdateAsync_BecauseOfKeyboardException_Throws(KeyboardErrorCode errorCode)
        {
            Mock.Get(_updateMonitor)
                .Setup(x => x.SendFirmware(It.IsAny<byte[]>()))
                .Throws(new KeyboardException(errorCode));

            var firmwarePackage = Mock.Of<IPackageFirmware>();

            Assert.ThrowsAsync<UpdateNemeioFailedException>(() => _updater.UpdateAsync(firmwarePackage));
        }

        [Test]
        public void NemeioUpdate_UpdatetAsync_BecauseOfKeyboardCommunication_Throws()
        {
            Mock.Get(_updateMonitor)
                .Setup(x => x.SendFirmware(It.IsAny<byte[]>()))
                .Throws(new TestableKeyboardCommunicationException());

            var firmwarePackage = Mock.Of<IPackageFirmware>();

            Assert.ThrowsAsync<UpdateNemeioFailedException>(() => _updater.UpdateAsync(firmwarePackage));
        }

        [Test]
        public async Task NemeioUpdate_UpdateAsync_Ok()
        {
            var firmwarePackage = Mock.Of<IPackageFirmware>();

            await _updater.UpdateAsync(firmwarePackage);
        }
    }
}
