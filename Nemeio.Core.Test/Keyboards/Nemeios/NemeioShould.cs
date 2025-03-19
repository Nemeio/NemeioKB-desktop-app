using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.DataModels;
using Nemeio.Core.Keyboard.Builds;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Communication.Exceptions;
using Nemeio.Core.Keyboard.CommunicationMode;
using Nemeio.Core.Keyboard.Errors;
using Nemeio.Core.Keyboard.FactoryReset;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Parameters;
using Nemeio.Core.Keyboard.SerialNumber;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Keyboard.Version;
using Nemeio.Core.Tools.Retry;
using NUnit.Framework;
using static Nemeio.Core.Keyboard.Nemeio;

namespace Nemeio.Core.Test.Keyboards.Nemeios
{
    [TestFixture]
    public class NemeioShould
    {
        private class TestableAbstractNemeio : Core.Keyboard.Nemeio
        {
            public bool InitKeyboardAsyncCalled { get; private set; }
            public bool StopCalled { get; private set; }
            public NemeioTrigger LastTriggerRaised { get; private set; }

            public TestableAbstractNemeio(ILoggerFactory loggerFactory, string identifier, Version protocolVersion, CommunicationType type, IKeyboardCommandExecutor commandExecutor, IMonitorFactory monitorFactory, IKeyboardCrashLogger crashLogger, IRetryHandler retryHandler)
                : base(loggerFactory, identifier, protocolVersion, type, commandExecutor, monitorFactory, crashLogger, retryHandler) 
            {
                _stateMachine.OnUnhandledTrigger((state, trigger) =>
                {
                    LastTriggerRaised = trigger;
                });
            }

            public Task ForceCallInitOnEntryAsync() => InitOnEntryAsync();

            protected override async Task InitKeyboardAsync()
            {
                await Task.Yield();

                InitKeyboardAsyncCalled = true;
            }

            public override void Stop()
            {
                base.Stop();

                StopCalled = true;
            }
        }

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
        private IRetryHandler _retryHandler;

        private TestableAbstractNemeio _nemeio;

        [SetUp]
        public void SetUp()
        {
            _loggerFactory = new LoggerFactory();
            _identifier = "ThisIsMyTestIdentifier";
            _type = CommunicationType.Serial;
            _commandExecutor = Mock.Of<IKeyboardCommandExecutor>();
            _retryHandler = new RetryHandler(_loggerFactory);

            _versionMonitor = Mock.Of<IVersionMonitor>();
            _serialNumberMonitor = Mock.Of<ISerialNumberMonitor>();
            _keyboardFailuresMonitor = Mock.Of<IKeyboardFailuresMonitor>();
            _parametersMonitor = Mock.Of<IParametersMonitor>();
            _communicationModeMonitor = Mock.Of<ICommunicationModeMonitor>();
            _factoryResetMonitor = Mock.Of<IFactoryResetMonitor>();

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

            _crashLogger = Mock.Of<IKeyboardCrashLogger>();
            _nemeio = new TestableAbstractNemeio(_loggerFactory, _identifier, new Version("2.0"),  _type, _commandExecutor, _monitorFactory, _crashLogger, _retryHandler);
        }

        public static Array KeyboardErrorCodes() => Enum.GetValues(typeof(KeyboardErrorCode));

        [Test]
        public void Nemeio_Constructor_Ok()
        {
            _nemeio.Identifier.Should().Be(_identifier);
            _nemeio.CommunicationType.Should().Be(_type);
            _nemeio.State.Should().Be(NemeioState.Connected);
            _nemeio.Started.Should().BeFalse();
        }

        [Test]
        public async Task Nemeio_Stop_Ok()
        {
            var commandExecutorStopCalled = false;

            Mock.Get(_commandExecutor)
                .Setup(x => x.StopAsync())
                .Callback(() => commandExecutorStopCalled = true)
                .Returns(Task.Delay(1));

            var fakeSerialNumber = "82948571934F";
            var fakeByteArraySerialNumber = Encoding.ASCII.GetBytes(fakeSerialNumber);

            Mock.Get(_versionMonitor)
                .Setup(x => x.AskVersions())
                .Returns(new FirmwareVersions());

            Mock.Get(_serialNumberMonitor)
                .Setup(x => x.AskSerialNumber())
                .Returns(new NemeioSerialNumber(fakeByteArraySerialNumber));

            Mock.Get(_parametersMonitor)
                .Setup(x => x.GetParameters())
                .Returns(new KeyboardParameters());

            Mock.Get(_keyboardFailuresMonitor)
                .Setup(x => x.AskKeyboardFailures())
                .Returns(new List<KeyboardFailure>()
                {
                    new KeyboardFailure(KeyboardEventId.None, new uint[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0, 0, 0, 0)
                });

            await _nemeio.ForceCallInitOnEntryAsync();

            await _nemeio.StopAsync();

            commandExecutorStopCalled.Should().BeTrue();
            _nemeio.Started.Should().BeFalse();
        }

        [Test]
        public async Task Nemeio_InitOnEntryAsync_Ok()
        {
            var fakeSerialNumber = "82948571934F";
            var fakeByteArraySerialNumber = Encoding.ASCII.GetBytes(fakeSerialNumber);

            Mock.Get(_versionMonitor)
                .Setup(x => x.AskVersions())
                .Returns(new FirmwareVersions());

            Mock.Get(_serialNumberMonitor)
                .Setup(x => x.AskSerialNumber())
                .Returns(new NemeioSerialNumber(fakeByteArraySerialNumber));

            Mock.Get(_parametersMonitor)
                .Setup(x => x.GetParameters())
                .Returns(new KeyboardParameters());

            Mock.Get(_keyboardFailuresMonitor)
                .Setup(x => x.AskKeyboardFailures())
                .Returns(new List<KeyboardFailure>()
                {
                    new KeyboardFailure(KeyboardEventId.None, new uint[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0, 0, 0, 0)
                });

            await _nemeio.ForceCallInitOnEntryAsync();

            _nemeio.Versions.Should().NotBeNull();
            _nemeio.SerialNumber.Should().NotBeNull();
            _nemeio.Parameters.Should().NotBeNull();
            _nemeio.Failures.Should().NotBeNull();
            _nemeio.Name.Should().NotBeNull();
            _nemeio.Name.Should().Be("934F");
        }

        [TestCaseSource("KeyboardErrorCodes")]
        public void Nemeio_InitOnEntryAsync_WhenFetchVersionFail_BecauseOfKeyboardException_CallStop(KeyboardErrorCode errorCode)
        {
            Mock.Get(_versionMonitor)
                .Setup(x => x.AskVersions())
                .Throws(new KeyboardException(errorCode));

            Assert.ThrowsAsync<InitializationFailedException>(() => _nemeio.ForceCallInitOnEntryAsync());

            _nemeio.StopCalled.Should().BeTrue();
        }

        [Test]
        public void Nemeio_InitOnEntryAsync_WhenFetchVersionFail_BecauseOfKeyboardCommunication_CallStop()
        {
            Mock.Get(_versionMonitor)
                .Setup(x => x.AskVersions())
                .Throws(new TestableKeyboardCommunicationException());

            Assert.ThrowsAsync<InitializationFailedException>(() => _nemeio.ForceCallInitOnEntryAsync());

            _nemeio.StopCalled.Should().BeTrue();
        }

        [TestCaseSource("KeyboardErrorCodes")]
        public void Nemeio_InitOnEntryAsync_WhenFetchSerialNumberFail_BecauseOfKeyboardException_CallStop(KeyboardErrorCode errorCode)
        {
            Mock.Get(_serialNumberMonitor)
                .Setup(x => x.AskSerialNumber())
                .Throws(new KeyboardException(errorCode));

            Assert.ThrowsAsync<InitializationFailedException>(() => _nemeio.ForceCallInitOnEntryAsync());

            _nemeio.StopCalled.Should().BeTrue();
        }

        [Test]
        public void Nemeio_InitOnEntryAsync_WhenFetchSerialNumberFail_BecauseOfKeyboardCommunication_CallStop()
        {
            Mock.Get(_serialNumberMonitor)
                .Setup(x => x.AskSerialNumber())
                .Throws(new TestableKeyboardCommunicationException());

            Assert.ThrowsAsync<InitializationFailedException>(() => _nemeio.ForceCallInitOnEntryAsync());

            _nemeio.StopCalled.Should().BeTrue();
        }

        [TestCaseSource("KeyboardErrorCodes")]
        public void Nemeio_InitOnEntryAsync_WhenFetchParametersFail_BecauseOfKeyboardException_CallStop(KeyboardErrorCode errorCode)
        {
            Mock.Get(_parametersMonitor)
                .Setup(x => x.GetParameters())
                .Throws(new KeyboardException(errorCode));

            Assert.ThrowsAsync<InitializationFailedException>(() => _nemeio.ForceCallInitOnEntryAsync());

            _nemeio.StopCalled.Should().BeTrue();
        }

        [Test]
        public void Nemeio_InitOnEntryAsync_WhenFetchParametersFail_BecauseOfKeyboardCommunication_CallStop()
        {
            Mock.Get(_parametersMonitor)
                .Setup(x => x.GetParameters())
                .Throws(new TestableKeyboardCommunicationException());

            Assert.ThrowsAsync<InitializationFailedException>(() => _nemeio.ForceCallInitOnEntryAsync());

            _nemeio.StopCalled.Should().BeTrue();
        }

        [TestCaseSource("KeyboardErrorCodes")]
        public void Nemeio_InitOnEntryAsync_WhenFetchCrashesFail_BecauseOfKeyboardException_CallStop(KeyboardErrorCode errorCode)
        {
            Mock.Get(_keyboardFailuresMonitor)
                .Setup(x => x.AskKeyboardFailures())
                .Throws(new KeyboardException(errorCode));

            Assert.ThrowsAsync<InitializationFailedException>(() => _nemeio.ForceCallInitOnEntryAsync());

            _nemeio.StopCalled.Should().BeTrue();
        }

        [Test]
        public void Nemeio_InitOnEntryAsync_WhenFetchCrashesFail_BecauseOfKeyboardCommunication_CallStop()
        {
            Mock.Get(_keyboardFailuresMonitor)
                .Setup(x => x.AskKeyboardFailures())
                .Throws(new TestableKeyboardCommunicationException());

            Assert.ThrowsAsync<InitializationFailedException>(() => _nemeio.ForceCallInitOnEntryAsync());

            _nemeio.StopCalled.Should().BeTrue();
        }

        [Test]
        public void Nemeio_InitializeAsync_WhenFailed_CallStop_AndPropagateException_Ok()
        {
            Mock.Get(_commandExecutor)
                .Setup(x => x.Initialize())
                .Throws(new InvalidOperationException("This is a fake exception for unit tests"));

            Assert.ThrowsAsync<InitializationFailedException>(() => _nemeio.ForceCallInitOnEntryAsync());

            _nemeio.StopCalled.Should().BeTrue();
        }

        [Test]
        public async Task Nemeio_DisconnectAsync_Ok()
        {
            _nemeio.State.Should().Be(NemeioState.Connected);

            await _nemeio.DisconnectAsync();

            _nemeio.LastTriggerRaised.Should().Be(NemeioTrigger.KeyboardUnplugged);
        }

        [Test]
        public void Nemeio_UpdateParametersAsync_WhenParameterIsNull_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _nemeio.UpdateParametersAsync(null));
        }

        [TestCaseSource("KeyboardErrorCodes")]
        public void Nemeio_UpdateParametersAsync_WhenCommunicationFail_Throws(KeyboardErrorCode errorCode)
        {
            var keyboardParameters = new KeyboardParameters();

            Mock.Get(_parametersMonitor)
                .Setup(x => x.SetParameters(It.IsAny<KeyboardParameters>()))
                .Throws(new KeyboardException(errorCode));

            Assert.ThrowsAsync<SetParametersFailedException>(() => _nemeio.UpdateParametersAsync(keyboardParameters));
        }

        [Test]
        public async Task Nemeio_UpdateParametersAsync_Ok()
        {
            var keyboardParameters = new KeyboardParameters()
            {
                DemoMode = true
            };

            await _nemeio.UpdateParametersAsync(keyboardParameters);

            _nemeio.Parameters.Should().NotBeNull();
            _nemeio.Parameters.Should().BeEquivalentTo(keyboardParameters);
        }

        [Test]
        public void Nemeio_SetHidModeAsync_Ok()
        {
            Assert.DoesNotThrowAsync(() => _nemeio.SetHidModeAsync());
        }

        [TestCaseSource("KeyboardErrorCodes")]
        public void Nemeio_SetHidModeAsync_WhenCommunicationFail_BecauseOfKeyboardException_Throws(KeyboardErrorCode errorCode)
        {
            Mock.Get(_communicationModeMonitor)
                .Setup(x => x.SetCommunicationMode(It.IsAny<KeyboardCommunicationMode>()))
                .Throws(new KeyboardException(errorCode));

            Assert.ThrowsAsync<SetCommunicationModeFailedException>(() => _nemeio.SetHidModeAsync());
        }

        [Test]
        public void Nemeio_SetHidModeAsync_WhenCommunicationFail_BecauseOfKeyboardCommunicationException_Throws()
        {
            Mock.Get(_communicationModeMonitor)
                .Setup(x => x.SetCommunicationMode(It.IsAny<KeyboardCommunicationMode>()))
                .Throws(new TestableKeyboardCommunicationException());

            Assert.ThrowsAsync<SetCommunicationModeFailedException>(() => _nemeio.SetHidModeAsync());
        }

        [Test]
        public void Nemeio_SetAdvancedModeAsync_Ok()
        {
            Assert.DoesNotThrowAsync(() => _nemeio.SetAdvancedModeAsync());
        }

        [TestCaseSource("KeyboardErrorCodes")]
        public void Nemeio_SetAdvancedModeAsync_WhenCommunicationFail_BecauseOfKeyboardException_Throws(KeyboardErrorCode errorCode)
        {
            Mock.Get(_communicationModeMonitor)
                .Setup(x => x.SetCommunicationMode(It.IsAny<KeyboardCommunicationMode>()))
                .Throws(new KeyboardException(errorCode));

            Assert.ThrowsAsync<SetCommunicationModeFailedException>(() => _nemeio.SetAdvancedModeAsync());
        }

        [Test]
        public void Nemeio_SetAdvancedModeAsync_WhenCommunicationFail_BecauseOfKeyboardCommunicationException_Throws()
        {
            Mock.Get(_communicationModeMonitor)
                .Setup(x => x.SetCommunicationMode(It.IsAny<KeyboardCommunicationMode>()))
                .Throws(new TestableKeyboardCommunicationException());

            Assert.ThrowsAsync<SetCommunicationModeFailedException>(() => _nemeio.SetAdvancedModeAsync());
        }

        [Test]
        public async Task Nemeio_WantFactoryResetAsync_Ok()
        {
            await _nemeio.WantFactoryResetAsync();

            _nemeio.LastTriggerRaised.Should().Be(NemeioTrigger.WantFactoryReset);
        }

        [Test]
        public async Task Nemeio_ConfirmFactoryResetAsync_Ok()
        {
            await _nemeio.ConfirmFactoryResetAsync();

            _nemeio.LastTriggerRaised.Should().Be(NemeioTrigger.StartFactoryReset);
        }

        [Test]
        public async Task Nemeio_CancelFactoryResetAsync_Ok()
        {
            await _nemeio.CancelFactoryResetAsync();

            _nemeio.LastTriggerRaised.Should().Be(NemeioTrigger.CancelFactoryReset);
        }
    }
}
