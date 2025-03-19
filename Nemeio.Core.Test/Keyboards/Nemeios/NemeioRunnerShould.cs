using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.DataModels;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Keyboard.Battery;
using Nemeio.Core.Keyboard.Builds;
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
using Nemeio.Core.Keyboard.GetLayouts;
using Nemeio.Core.Keyboard.KeepAlive;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Keys;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Nemeios.Runner;
using Nemeio.Core.Keyboard.Parameters;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Keyboard.SerialNumber;
using Nemeio.Core.Keyboard.Version;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Batteries;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Tools;
using Nemeio.Core.Tools.Retry;
using NUnit.Framework;

namespace Nemeio.Core.Test.Keyboards.Nemeios
{
    [TestFixture]
    public class NemeioRunnerShould
    {
        private class TestableKeyboardCommunicationException : KeyboardCommunicationException { }

        private ILoggerFactory _loggerFactory;
        private ILogger<NemeioRunner> _logger;
        private ITimer _keepAliveTimer;
        private ITimer _batteryTimer;
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
        private IKeepAliveMonitor _keepAliveMonitor;
        private IAddConfigurationMonitor _addConfigurationMonitor;
        private IDeleteConfigurationMonitor _deleteConfigurationMonitor;
        private IApplyConfigurationMonitor _applyConfigurationMonitor;
        private IGetLayoutsMonitor _layoutHashMonitor;
        private IConfigurationChangedMonitor _configurationChangedMonitor;
        private IKeyPressedMonitor _keyPressedMonitor;
        private IBatteryMonitor _batteryMonitor;
        private IRetryHandler _retryHandler;

        private NemeioRunner _runner;

        public static Array KeyboardErrorCodes() => Enum.GetValues(typeof(KeyboardErrorCode));

        [SetUp]
        public void SetUp()
        {
            _loggerFactory = new LoggerFactory();

            _identifier = "ThisIsMyTestIdentifier";
            _type = CommunicationType.Serial;
            _commandExecutor = Mock.Of<IKeyboardCommandExecutor>();

            var screen = Mock.Of<IScreen>();
            Mock.Get(screen)
                .Setup(x => x.TransformKeystrokes(It.IsAny<List<NemeioIndexKeystroke>>()))
                .Returns(new List<NemeioIndexKeystroke>());

            var screenFactory = Mock.Of<IScreenFactory>();
            Mock.Get(screenFactory)
                .Setup(x => x.CreateEinkScreen())
                .Returns(screen);
            Mock.Get(screenFactory)
                .Setup(x => x.CreateHolitechScreen())
                .Returns(screen);

            _versionMonitor = Mock.Of<IVersionMonitor>();
            _serialNumberMonitor = Mock.Of<ISerialNumberMonitor>();
            _keyboardFailuresMonitor = Mock.Of<IKeyboardFailuresMonitor>();
            _parametersMonitor = Mock.Of<IParametersMonitor>();
            _communicationModeMonitor = Mock.Of<ICommunicationModeMonitor>();
            _factoryResetMonitor = Mock.Of<IFactoryResetMonitor>();
            _keepAliveMonitor = Mock.Of<IKeepAliveMonitor>();
            _addConfigurationMonitor = Mock.Of<IAddConfigurationMonitor>();
            _deleteConfigurationMonitor = Mock.Of<IDeleteConfigurationMonitor>();
            _applyConfigurationMonitor = Mock.Of<IApplyConfigurationMonitor>();
            _layoutHashMonitor = Mock.Of<IGetLayoutsMonitor>();
            _configurationChangedMonitor = Mock.Of<IConfigurationChangedMonitor>();
            _keyPressedMonitor = Mock.Of<IKeyPressedMonitor>();
            _batteryMonitor = Mock.Of<IBatteryMonitor>();
            _retryHandler = new RetryHandler(_loggerFactory);

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
                .Setup(x => x.CreateKeepAliveMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_keepAliveMonitor);
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateAddConfigurationMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_addConfigurationMonitor);
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateDeleteConfigurationMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_deleteConfigurationMonitor);
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateApplyConfigurationMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_applyConfigurationMonitor);
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateGetLayoutsMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_layoutHashMonitor);
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateConfigurationChangedMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_configurationChangedMonitor);
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateKeyPressedMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_keyPressedMonitor);
            Mock.Get(_monitorFactory)
                .Setup(x => x.CreateBatteryMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(_batteryMonitor);

            _crashLogger = Mock.Of<IKeyboardCrashLogger>();
            _keepAliveTimer = Mock.Of<ITimer>();
            _batteryTimer = Mock.Of<ITimer>();

            _runner = new NemeioRunner(_loggerFactory, _identifier, new Version("2.0"), _type, _commandExecutor, _monitorFactory, _crashLogger, _keepAliveTimer, _batteryTimer, _retryHandler, screenFactory);
        }

        [TestCaseSource("KeyboardErrorCodes")]
        public void NemeioRunner_InitOnEntryAsync_WhenFetchBatteryFail_BecauseOfKeyboardException_CallStop(KeyboardErrorCode errorCode)
        {
            Mock.Get(_serialNumberMonitor)
                .Setup(x => x.AskSerialNumber())
                .Throws(new KeyboardException(errorCode));

            Assert.ThrowsAsync<InitializationFailedException>(() => _runner.InitializeAsync());

            _runner.Started.Should().BeFalse();
        }

        [Test]
        public void NemeioRunner_InitOnEntryAsync_WhenFetchBatteryFail_BecauseOfKeyboardCommunication_CallStop()
        {
            Mock.Get(_serialNumberMonitor)
                .Setup(x => x.AskSerialNumber())
                .Throws(new TestableKeyboardCommunicationException());

            Assert.ThrowsAsync<InitializationFailedException>(() => _runner.InitializeAsync());

            _runner.Started.Should().BeFalse();
        }

        [TestCaseSource("KeyboardErrorCodes")]
        public void NemeioRunner_InitOnEntryAsync_WhenFetchLayoutHashesFail_BecauseOfKeyboardException_CallStop(KeyboardErrorCode errorCode)
        {
            Mock.Get(_serialNumberMonitor)
                .Setup(x => x.AskSerialNumber())
                .Throws(new KeyboardException(errorCode));

            Assert.ThrowsAsync<InitializationFailedException>(() => _runner.InitializeAsync());

            _runner.Started.Should().BeFalse();
        }

        [Test]
        public void NemeioRunner_InitOnEntryAsync_WhenFetchLayoutHashesFail_BecauseOfKeyboardCommunication_CallStop()
        {
            Mock.Get(_serialNumberMonitor)
                .Setup(x => x.AskSerialNumber())
                .Throws(new TestableKeyboardCommunicationException());

            Assert.ThrowsAsync<InitializationFailedException>(() => _runner.InitializeAsync());

            _runner.Started.Should().BeFalse();
        }

        [Test]
        public void NemeioRunner_UpdateSelectedLayout_AndRaiseOnSelectedLayoutChanged_WhenMonitorNotify_Ok()
        {
            var onSelectedLayoutChangedCalled = false;

            _runner.OnSelectedLayoutChanged += delegate
            {
                onSelectedLayoutChangedCalled = true;
            };

            var selectedLayoutHashOnKeyboard = new LayoutId("D4008462-6412-4931-BA91-82889D048098");

            Mock.Get(_configurationChangedMonitor)
                .Raise(x => x.ConfigurationChanged += null, this, new ConfigurationChangedEventArgs(selectedLayoutHashOnKeyboard));

            onSelectedLayoutChangedCalled.Should().BeTrue();
            _runner.SelectedLayoutId.Should().Be(selectedLayoutHashOnKeyboard);
        }

        [Test]
        public async Task NemeioRunner_RaiseOnKeyPressed_WhenMonitorNotify_Ok()
        {
            var onKeyPressedCalled = false;

            _runner.OnKeyPressed += delegate
            {
                onKeyPressedCalled = true;
            };

            var fakeSerialNumber = "82948571934F";
            var fakeByteArraySerialNumber = Encoding.ASCII.GetBytes(fakeSerialNumber);

            Mock.Get(_batteryMonitor)
                .Setup(x => x.AskBattery())
                .Returns(new BatteryInformation(
                    new BatteryLevel(90),
                    80,
                    new BatteryTime(10),
                    new BatteryTime(10),
                    10,
                    8
                ));

            Mock.Get(_versionMonitor)
                .Setup(x => x.AskVersions())
                .Returns(new FirmwareVersions()
                {
                    Ite = new VersionProxy("1.0"),
                    ScreenType = ScreenType.Holitech
                });

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

            await _runner.InitializeAsync();

            var selectedLayoutHashOnKeyboard = new LayoutId("D4008462-6412-4931-BA91-82889D048098");

            Mock.Get(_configurationChangedMonitor)
                .Raise(x => x.ConfigurationChanged += null, this, new ConfigurationChangedEventArgs(selectedLayoutHashOnKeyboard));

            Mock.Get(_keyPressedMonitor)
                .Raise(x => x.OnKeyPressed += null, this, new KeyPressedEventArgs(new List<NemeioIndexKeystroke>()));

            onKeyPressedCalled.Should().BeTrue();
        }

        [Test]
        public async Task NemeioRunner_AddLayoutAsync_WithNullParameter_DoNothing()
        {
            var addOnMonitorCalled = false;

            Mock.Get(_addConfigurationMonitor)
                .Setup(x => x.SendConfiguration(It.IsAny<ILayout>(), false))
                .Callback(() => addOnMonitorCalled = true);

            await _runner.AddLayoutAsync(null);

            addOnMonitorCalled.Should().BeFalse();
        }

        [Test]
        public async Task NemeioRunner_AddLayoutAsync_WhenStateIsFactoryReset_DoNothing()
        {
            var addOnMonitorCalled = false;

            var fakeSerialNumber = "82948571934F";
            var fakeByteArraySerialNumber = Encoding.ASCII.GetBytes(fakeSerialNumber);

            Mock.Get(_addConfigurationMonitor)
                .Setup(x => x.SendConfiguration(It.IsAny<ILayout>(), false))
                .Callback(() => addOnMonitorCalled = true);

            Mock.Get(_batteryMonitor)
                .Setup(x => x.AskBattery())
                .Returns(new BatteryInformation(
                    new BatteryLevel(90),
                    80,
                    new BatteryTime(10),
                    new BatteryTime(10),
                    10,
                    8
                ));

            Mock.Get(_versionMonitor)
                .Setup(x => x.AskVersions())
                .Returns(new FirmwareVersions()
                {
                    Ite = new VersionProxy("1.0"),
                    ScreenType = ScreenType.Holitech
                });

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

            var layout = Mock.Of<ILayout>();

            await _runner.InitializeAsync();
            await _runner.StartSynchronizationAsync();
            await _runner.EndSynchronizationAsync();
            await _runner.WantFactoryResetAsync();
            await _runner.AddLayoutAsync(layout);

            addOnMonitorCalled.Should().BeFalse();
        }

        [TestCaseSource("KeyboardErrorCodes")]
        public void NemeioRunner_AddLayoutAsync_BecauseOfKeyboardException_Throws(KeyboardErrorCode errorCode)
        {
            Mock.Get(_serialNumberMonitor)
                .Setup(x => x.AskSerialNumber())
                .Throws(new KeyboardException(errorCode));

            Assert.ThrowsAsync<InitializationFailedException>(() => _runner.InitializeAsync());

            _runner.Started.Should().BeFalse();
        }

        [Test]
        public void NemeioRunner_AddLayoutAsync_BecauseOfKeyboardCommunication_Throws()
        {
            Mock.Get(_serialNumberMonitor)
                .Setup(x => x.AskSerialNumber())
                .Throws(new TestableKeyboardCommunicationException());

            Assert.ThrowsAsync<InitializationFailedException>(() => _runner.InitializeAsync());

            _runner.Started.Should().BeFalse();
        }

        [Test]
        public async Task NemeioRunner_AddLayoutAsync_Ok()
        {
            var addOnMonitorCalled = false;

            var fakeSerialNumber = "82948571934F";
            var fakeByteArraySerialNumber = Encoding.ASCII.GetBytes(fakeSerialNumber);

            Mock.Get(_addConfigurationMonitor)
                .Setup(x => x.SendConfiguration(It.IsAny<ILayout>(), false))
                .Callback(() => addOnMonitorCalled = true);

            Mock.Get(_batteryMonitor)
                .Setup(x => x.AskBattery())
                .Returns(new BatteryInformation(
                    new BatteryLevel(90),
                    80,
                    new BatteryTime(10),
                    new BatteryTime(10),
                    10,
                    8
                ));

            Mock.Get(_versionMonitor)
                .Setup(x => x.AskVersions())
                .Returns(new FirmwareVersions()
                {
                    Ite = new VersionProxy("1.0"),
                    ScreenType = ScreenType.Holitech
                });

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

            var toBeAddedId = new LayoutId("7879B0BB-DD87-442A-B63A-C915C3FA1AF2");
            var toBeAddedLayout = Mock.Of<ILayout>();
            var screen = Mock.Of<IScreen>();

            Mock.Get(toBeAddedLayout)
                .Setup(x => x.LayoutId)
                .Returns(toBeAddedId);
            Mock.Get(toBeAddedLayout)
                .Setup(x => x.LayoutImageInfo)
                .Returns(new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen));

            Mock.Get(_layoutHashMonitor)
                .Setup(x => x.AskLayoutIds())
                .Returns(new List<LayoutIdWithHash>()
                {
                    new LayoutIdWithHash(new LayoutId("8879B0BB-DD87-442A-B63A-C915C3FA1AF2"), new LayoutHash("8879B0BB-DD87-442A-B63A-C915C3FA1AF2")),
                    new LayoutIdWithHash(new LayoutId("9879B0BB-DD87-442A-B63A-C915C3FA1AF2"), new LayoutHash("9879B0BB-DD87-442A-B63A-C915C3FA1AF2")),
                });

            await _runner.InitializeAsync();
            await _runner.StartSynchronizationAsync();
            await _runner.EndSynchronizationAsync();

            _runner.LayoutIdWithHashs.Count.Should().Be(2);

            await _runner.AddLayoutAsync(toBeAddedLayout);

            addOnMonitorCalled.Should().BeTrue();
            _runner.LayoutIdWithHashs.Count.Should().Be(3);
            _runner.LayoutIdWithHashs.Select(x => x.Id).Should().Contain(toBeAddedId);
        }


        [Test]
        public async Task NemeioRunner_DeleteLayoutAsync_WithNullParameter_DoNothing()
        {
            var deleteOnMonitorCalled = false;

            Mock.Get(_deleteConfigurationMonitor)
                .Setup(x => x.Delete(It.IsAny<LayoutId>()))
                .Callback(() => deleteOnMonitorCalled = true);

            await _runner.DeleteLayoutAsync(null);

            deleteOnMonitorCalled.Should().BeFalse();
        }

        [Test]
        public async Task NemeioRunner_DeleteLayoutAsync_WhenStateIsFactoryReset_DoNothing()
        {
            var deleteOnMonitorCalled = false;

            var fakeSerialNumber = "82948571934F";
            var fakeByteArraySerialNumber = Encoding.ASCII.GetBytes(fakeSerialNumber);

            Mock.Get(_deleteConfigurationMonitor)
                .Setup(x => x.Delete(It.IsAny<LayoutId>()))
                .Callback(() => deleteOnMonitorCalled = true);

            Mock.Get(_batteryMonitor)
                .Setup(x => x.AskBattery())
                .Returns(new BatteryInformation(
                    new BatteryLevel(90),
                    80,
                    new BatteryTime(10),
                    new BatteryTime(10),
                    10,
                    8
                ));

            Mock.Get(_versionMonitor)
                .Setup(x => x.AskVersions())
                .Returns(new FirmwareVersions()
                {
                    Ite = new VersionProxy("1.0"),
                    ScreenType = ScreenType.Holitech
                });

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

            var layoutId = new LayoutId("7879B0BB-DD87-442A-B63A-C915C3FA1AF2");

            await _runner.InitializeAsync();
            await _runner.StartSynchronizationAsync();
            await _runner.EndSynchronizationAsync();
            await _runner.WantFactoryResetAsync();
            await _runner.DeleteLayoutAsync(layoutId);

            deleteOnMonitorCalled.Should().BeFalse();
        }

        [TestCaseSource("KeyboardErrorCodes")]
        public void NemeioRunner_DeleteLayoutAsync_BecauseOfKeyboardException_Throws(KeyboardErrorCode errorCode)
        {
            Mock.Get(_serialNumberMonitor)
                .Setup(x => x.AskSerialNumber())
                .Throws(new KeyboardException(errorCode));

            Assert.ThrowsAsync<InitializationFailedException>(() => _runner.InitializeAsync());

            _runner.Started.Should().BeFalse();
        }

        [Test]
        public void NemeioRunner_DeleteLayoutAsync_BecauseOfKeyboardCommunication_Throws()
        {
            Mock.Get(_serialNumberMonitor)
                .Setup(x => x.AskSerialNumber())
                .Throws(new TestableKeyboardCommunicationException());

            Assert.ThrowsAsync<InitializationFailedException>(() => _runner.InitializeAsync());

            _runner.Started.Should().BeFalse();
        }

        [Test]
        public async Task NemeioRunner_DeleteLayoutAsync_Ok()
        {
            var deleteOnMonitorCalled = false;

            var fakeSerialNumber = "82948571934F";
            var fakeByteArraySerialNumber = Encoding.ASCII.GetBytes(fakeSerialNumber);

            Mock.Get(_deleteConfigurationMonitor)
                .Setup(x => x.Delete(It.IsAny<LayoutId>()))
                .Callback(() => deleteOnMonitorCalled = true);

            Mock.Get(_batteryMonitor)
                .Setup(x => x.AskBattery())
                .Returns(new BatteryInformation(
                    new BatteryLevel(90),
                    80,
                    new BatteryTime(10),
                    new BatteryTime(10),
                    10,
                    8
                ));

            Mock.Get(_versionMonitor)
                .Setup(x => x.AskVersions())
                .Returns(new FirmwareVersions()
                {
                    Ite = new VersionProxy("1.0"),
                    ScreenType = ScreenType.Holitech
                });

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

            var toBeDeleted = new LayoutIdWithHash(new LayoutId("7879B0BB-DD87-442A-B63A-C915C3FA1AF2"), new LayoutHash("7879B0BB-DD87-442A-B63A-C915C3FA1AF2"));


            Mock.Get(_layoutHashMonitor)
                .Setup(x => x.AskLayoutIds())
                .Returns(new List<LayoutIdWithHash>()
                {
                    toBeDeleted,
                    new LayoutIdWithHash(new LayoutId("8879B0BB-DD87-442A-B63A-C915C3FA1AF2"),new LayoutHash("8879B0BB-DD87-442A-B63A-C915C3FA1AF2")),
                    new LayoutIdWithHash(new LayoutId("9879B0BB-DD87-442A-B63A-C915C3FA1AF2"),new LayoutHash("9879B0BB-DD87-442A-B63A-C915C3FA1AF2"))
                });

            await _runner.InitializeAsync();
            await _runner.StartSynchronizationAsync();
            await _runner.EndSynchronizationAsync();

            _runner.LayoutIdWithHashs.Count.Should().Be(3);

            await _runner.DeleteLayoutAsync(toBeDeleted.Id);

            deleteOnMonitorCalled.Should().BeTrue();
            _runner.LayoutIdWithHashs.Count.Should().Be(2);
            _runner.LayoutIdWithHashs.Should().NotContain(toBeDeleted);
        }

        [Test]
        public async Task NemeioRunner_ApplyLayoutAsync_WithNullParameter_DoNothing()
        {
            var applyOnMonitorCalled = false;

            Mock.Get(_applyConfigurationMonitor)
                .Setup(x => x.Apply(It.IsAny<LayoutId>()))
                .Callback(() => applyOnMonitorCalled = true);

            await _runner.ApplyLayoutAsync(null);

            applyOnMonitorCalled.Should().BeFalse();
        }

        [Test]
        public async Task NemeioRunner_ApplyLayoutAsync_WhenStateIsFactoryReset_DoNothing()
        {
            var applyOnMonitorCalled = false;

            var fakeSerialNumber = "82948571934F";
            var fakeByteArraySerialNumber = Encoding.ASCII.GetBytes(fakeSerialNumber);

            Mock.Get(_applyConfigurationMonitor)
                .Setup(x => x.Apply(It.IsAny<LayoutId>()))
                .Callback(() => applyOnMonitorCalled = true);

            Mock.Get(_batteryMonitor)
                .Setup(x => x.AskBattery())
                .Returns(new BatteryInformation(
                    new BatteryLevel(90),
                    80,
                    new BatteryTime(10),
                    new BatteryTime(10),
                    10,
                    8
                ));

            Mock.Get(_versionMonitor)
                .Setup(x => x.AskVersions())
                .Returns(new FirmwareVersions()
                {
                    Ite = new VersionProxy("1.0"),
                    ScreenType = ScreenType.Holitech
                });

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

            var layout = Mock.Of<ILayout>();

            await _runner.InitializeAsync();
            await _runner.StartSynchronizationAsync();
            await _runner.EndSynchronizationAsync();
            await _runner.WantFactoryResetAsync();
            await _runner.ApplyLayoutAsync(layout);

            applyOnMonitorCalled.Should().BeFalse();
        }

        [TestCaseSource("KeyboardErrorCodes")]
        public void NemeioRunner_ApplyLayoutAsync_BecauseOfKeyboardException_Throws(KeyboardErrorCode errorCode)
        {
            Mock.Get(_applyConfigurationMonitor)
                .Setup(x => x.Apply(It.IsAny<LayoutId>()))
                .Throws(new KeyboardException(errorCode));

            var layout = Mock.Of<ILayout>();

            Assert.ThrowsAsync<ApplyConfigurationFailedException>(() => _runner.ApplyLayoutAsync(layout));
        }

        [Test]
        public void NemeioRunner_ApplyLayoutAsync_BecauseOfKeyboardCommunication_Throws()
        {
            Mock.Get(_applyConfigurationMonitor)
                .Setup(x => x.Apply(It.IsAny<LayoutId>()))
                .Throws(new TestableKeyboardCommunicationException());

            var layout = Mock.Of<ILayout>();

            Assert.ThrowsAsync<ApplyConfigurationFailedException>(() => _runner.ApplyLayoutAsync(layout));
        }

        [Test]
        public void NemeioRunner_ApplyLayoutAsync_Ok()
        {
            var layout = Mock.Of<ILayout>();

            Assert.DoesNotThrowAsync(() => _runner.ApplyLayoutAsync(layout));
        }
    }
}
