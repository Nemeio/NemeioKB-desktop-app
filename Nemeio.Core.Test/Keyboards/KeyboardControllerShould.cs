using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Builds;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Watchers;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Sessions;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Systems;
using Nemeio.Core.Tools.Retry;
using NUnit.Framework;

namespace Nemeio.Core.Test.Keyboards
{
    [TestFixture]
    public class KeyboardControllerShould
    {
        private class TestableNemeio : Keyboard.Nemeio
        {
            public bool InitKeyboardAsyncCalled { get; private set; }
            public bool StopCalled { get; private set; }

            public TestableNemeio(ILoggerFactory loggerFactory, string identifier, Version protocolVersion, CommunicationType type, IKeyboardCommandExecutor commandExecutor, IMonitorFactory monitorFactory, IKeyboardCrashLogger crashLogger, IRetryHandler retryHandler) 
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
            }

            public override void Stop()
            {
                base.Stop();

                StopCalled = true;
            }

            public override async Task StopAsync()
            {
                await base.StopAsync();

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

                AliveState = Core.Tools.Stoppable.AliveState.Started;
            }
        }

        [Test]
        public void KeyboardController_Constructor_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var keyboardProvider = Mock.Of<IKeyboardProvider>();
            var nemeioBuilder = Mock.Of<INemeioBuilder>();
            var system = Mock.Of<ISystem>();
            var eventStrategyFactory = Mock.Of<INemeioLayoutEventStrategyFactory>();

            Assert.DoesNotThrow(() => new KeyboardController(loggerFactory, keyboardProvider, nemeioBuilder, system, eventStrategyFactory));
        }

        [Test]
        public async Task KeyboardController_RunAsync_WhenSessionIsOpen_AutomaticallyCheckKeyboard()
        {
            var getKeyboardCalled = false;
            var loggerFactory = new LoggerFactory();

            var keyboardProvider = Mock.Of<IKeyboardProvider>();
            Mock.Get(keyboardProvider)
                .Setup(x => x.GetKeyboard(It.IsAny<Func<Keyboard.Keyboard, bool>>()))
                .Returns(default(Keyboard.Keyboard))
                .Callback(() => getKeyboardCalled = true);

            var nemeioBuilder = Mock.Of<INemeioBuilder>();

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.SessionState)
                .Returns(Core.Systems.Sessions.SessionState.Open);

            var eventStrategyFactory = Mock.Of<INemeioLayoutEventStrategyFactory>();

            var keyboardController = new KeyboardController(loggerFactory, keyboardProvider, nemeioBuilder, system, eventStrategyFactory);

            await keyboardController.RunAsync();

            getKeyboardCalled.Should().BeTrue();
        }

        [Test]
        public async Task KeyboardController_RunAsync_WhenSessionIsClosed_NoCheckDone()
        {
            var getKeyboardCalled = false;
            var loggerFactory = new LoggerFactory();

            var keyboardProvider = Mock.Of<IKeyboardProvider>();
            Mock.Get(keyboardProvider)
                .Setup(x => x.GetKeyboard(null))
                .Returns(default(Keyboard.Keyboard))
                .Callback(() => getKeyboardCalled = true);

            var nemeioBuilder = Mock.Of<INemeioBuilder>();

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.SessionState)
                .Returns(Core.Systems.Sessions.SessionState.Lock);

            var eventStrategyFactory = Mock.Of<INemeioLayoutEventStrategyFactory>();

            var keyboardController = new KeyboardController(loggerFactory, keyboardProvider, nemeioBuilder, system, eventStrategyFactory);

            await keyboardController.RunAsync();

            getKeyboardCalled.Should().BeFalse();
        }

        [Test]
        public async Task KeyboardController_RunAsync_WhenSessionIsOpen_AndKeyboardFound_TryToBuildAndStartNemeio()
        {
            var buildCalled = false;
            var onKeyboardConnectedRaised = false;

            var nemeioIdentifier = "MY-TEST-NEMEIO";
            var loggerFactory = new LoggerFactory();

            var keyboard = new Keyboard.Keyboard(nemeioIdentifier, new System.Version("2.0"), CommunicationType.Serial, Mock.Of<IKeyboardIO>());
            var keyboardProvider = Mock.Of<IKeyboardProvider>();
            Mock.Get(keyboardProvider)
                .Setup(x => x.GetKeyboard(It.IsAny<Func<Keyboard.Keyboard, bool>>()))
                .Returns(() => keyboard);

            var testableNemeio = new TestableNemeio(loggerFactory, nemeioIdentifier, keyboard.ProtocolVersion, It.IsAny<CommunicationType>(), Mock.Of<IKeyboardCommandExecutor>(), Mock.Of<IMonitorFactory>(), Mock.Of<IKeyboardCrashLogger>(), Mock.Of<IRetryHandler>());
            var nemeioBuilder = Mock.Of<INemeioBuilder>();
            Mock.Get(nemeioBuilder)
                .Setup(x => x.BuildAsync(It.IsAny<Keyboard.Keyboard>()))
                .ReturnsAsync(testableNemeio)
                .Callback(() => buildCalled = true);

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.SessionState)
                .Returns(Core.Systems.Sessions.SessionState.Open);

            var eventStrategyFactory = Mock.Of<INemeioLayoutEventStrategyFactory>();

            var keyboardController = new KeyboardController(loggerFactory, keyboardProvider, nemeioBuilder, system, eventStrategyFactory);
            keyboardController.OnKeyboardConnected += delegate
            {
                onKeyboardConnectedRaised = true;
            };

            await keyboardController.RunAsync();

            buildCalled.Should().BeTrue();
            keyboardController.Nemeio.Should().NotBeNull();
            onKeyboardConnectedRaised.Should().BeTrue();
        }

        [Test]
        public async Task KeyboardController_AutomaticallyCheckKeyboad_WhenNoKeyboardConnected_AndSessionIsOpen_AndSystemIsAwake_AndProviderHaveNewKeyboard_Ok()
        {
            var buildCalled = false;
            var onKeyboardConnectedRaised = false;

            var nemeioIdentifier = "MY-TEST-NEMEIO";
            var loggerFactory = new LoggerFactory();

            var keyboardProvider = Mock.Of<IKeyboardProvider>();
            var nemeioBuilder = Mock.Of<INemeioBuilder>();
            Mock.Get(nemeioBuilder)
                .Setup(x => x.BuildAsync(It.IsAny<Keyboard.Keyboard>()))
                .ReturnsAsync(default(Keyboard.Nemeio));

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.SessionState)
                .Returns(Core.Systems.Sessions.SessionState.Open);
            Mock.Get(system)
                .Setup(x => x.SleepMode)
                .Returns(Core.Systems.Sleep.SleepMode.Awake);

            var eventStrategyFactory = Mock.Of<INemeioLayoutEventStrategyFactory>();

            var keyboardController = new KeyboardController(loggerFactory, keyboardProvider, nemeioBuilder, system, eventStrategyFactory);
            keyboardController.OnKeyboardConnected += delegate
            {
                onKeyboardConnectedRaised = true;
            };

            await keyboardController.RunAsync();

            var keyboard = new Keyboard.Keyboard(nemeioIdentifier, new System.Version("2.0"), CommunicationType.Serial, Mock.Of<IKeyboardIO>());
            Mock.Get(keyboardProvider)
                .Setup(x => x.GetKeyboard(It.IsAny<Func<Keyboard.Keyboard, bool>>()))
                .Returns(() => keyboard);

            var testableNemeio = new TestableNemeio(loggerFactory, nemeioIdentifier, keyboard.ProtocolVersion, It.IsAny<CommunicationType>(), Mock.Of<IKeyboardCommandExecutor>(), Mock.Of<IMonitorFactory>(), Mock.Of<IKeyboardCrashLogger>(), Mock.Of<IRetryHandler>());
            Mock.Get(nemeioBuilder)
                .Setup(x => x.BuildAsync(It.IsAny<Keyboard.Keyboard>()))
                .ReturnsAsync(testableNemeio)
                .Callback(() => buildCalled = true);

            Mock.Get(keyboardProvider)
                .Raise(x => x.OnKeyboardConnected += null, EventArgs.Empty);

            buildCalled.Should().BeTrue();
            keyboardController.Nemeio.Should().NotBeNull();
            onKeyboardConnectedRaised.Should().BeTrue();
        }

        [Test]
        public async Task KeyboardController_DisconnectNemeio_WhenKeyboardProviderRaiseDisconnectEvent_Ok()
        {
            var testSempahore = new SemaphoreSlim(0, 1);

            var onKeyboardConnectedRaised = false;
            var onKeyboardDisconnectingRaised = false;
            var onKeyboardDisconnectedRaised = false;

            var nemeioIdentifier = "MY-TEST-NEMEIO";
            var loggerFactory = new LoggerFactory();

            var keyboard = new Keyboard.Keyboard(nemeioIdentifier, new System.Version("2.0"), CommunicationType.Serial, Mock.Of<IKeyboardIO>());
            var keyboardProvider = Mock.Of<IKeyboardProvider>();
            Mock.Get(keyboardProvider)
                .Setup(x => x.GetKeyboard(It.IsAny<Func<Keyboard.Keyboard, bool>>()))
                .Returns(() => keyboard);

            var testableNemeio = new TestableNemeio(loggerFactory, nemeioIdentifier, keyboard.ProtocolVersion, It.IsAny<CommunicationType>(), Mock.Of<IKeyboardCommandExecutor>(), Mock.Of<IMonitorFactory>(), Mock.Of<IKeyboardCrashLogger>(), Mock.Of<IRetryHandler>());
            var nemeioBuilder = Mock.Of<INemeioBuilder>();
            Mock.Get(nemeioBuilder)
                .Setup(x => x.BuildAsync(It.IsAny<Keyboard.Keyboard>()))
                .ReturnsAsync(testableNemeio);

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.SessionState)
                .Returns(Core.Systems.Sessions.SessionState.Open);

            var eventStrategyFactory = Mock.Of<INemeioLayoutEventStrategyFactory>();

            var keyboardController = new KeyboardController(loggerFactory, keyboardProvider, nemeioBuilder, system, eventStrategyFactory);
            keyboardController.OnKeyboardConnected += delegate
            {
                onKeyboardConnectedRaised = true;
            };
            keyboardController.OnKeyboardDisconnecting += delegate
            {
                onKeyboardDisconnectingRaised = true;
            };
            keyboardController.OnKeyboardDisconnected += delegate
            {
                onKeyboardDisconnectedRaised = true;

                testSempahore.Release();
            };

            await keyboardController.RunAsync();

            keyboardController.Nemeio.Should().NotBeNull();
            onKeyboardConnectedRaised.Should().BeTrue();

            TestableNemeio nullNemeio = null;
            Mock.Get(nemeioBuilder)
                .Setup(x => x.BuildAsync(It.IsAny<Keyboard.Keyboard>()))
                .ReturnsAsync(nullNemeio);

            Mock.Get(keyboardProvider)
                .Raise(x => x.OnKeyboardDisconnected += null, this, new KeyboardDisconnectedEventArgs(keyboard));

            var released = await testSempahore.WaitAsync(2000);
            if (!released)
            {
                throw new InvalidOperationException("Semaphore not released");
            }

            onKeyboardDisconnectingRaised.Should().BeTrue();
            testableNemeio.StopCalled.Should().BeTrue();
            keyboardController.Nemeio.Should().BeNull();
            onKeyboardDisconnectedRaised.Should().BeTrue();
        }

        [Test]
        public async Task KeyboardController_DisconnectNemeio_WhenKeyboardProviderRaiseDisconnectEvent_ButNotForCurrentNemeio_Ok()
        {
            var onKeyboardConnectedRaised = false;
            var onKeyboardDisconnectingRaised = false;
            var onKeyboardDisconnectedRaised = false;

            var nemeioIdentifier = "MY-TEST-NEMEIO";
            var loggerFactory = new LoggerFactory();

            var keyboard = new Keyboard.Keyboard(nemeioIdentifier, new System.Version("3.0"), CommunicationType.Serial, Mock.Of<IKeyboardIO>());
            var secondKeyboard = new Keyboard.Keyboard("MY-FAKE-NEMEIO", new System.Version("3.0"), CommunicationType.Serial, Mock.Of<IKeyboardIO>());
            var keyboardProvider = Mock.Of<IKeyboardProvider>();
            Mock.Get(keyboardProvider)
                .Setup(x => x.GetKeyboard(It.IsAny<Func<Keyboard.Keyboard, bool>>()))
                .Returns(() => keyboard);

            var testableNemeio = new TestableNemeio(loggerFactory, nemeioIdentifier, keyboard.ProtocolVersion, It.IsAny<CommunicationType>(), Mock.Of<IKeyboardCommandExecutor>(), Mock.Of<IMonitorFactory>(), Mock.Of<IKeyboardCrashLogger>(), Mock.Of<IRetryHandler>());
            var nemeioBuilder = Mock.Of<INemeioBuilder>();
            Mock.Get(nemeioBuilder)
                .Setup(x => x.BuildAsync(It.IsAny<Keyboard.Keyboard>()))
                .ReturnsAsync(testableNemeio);

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.SessionState)
                .Returns(Core.Systems.Sessions.SessionState.Open);

            var eventStrategyFactory = Mock.Of<INemeioLayoutEventStrategyFactory>();

            var keyboardController = new KeyboardController(loggerFactory, keyboardProvider, nemeioBuilder, system, eventStrategyFactory);
            keyboardController.OnKeyboardConnected += delegate
            {
                onKeyboardConnectedRaised = true;
            };
            keyboardController.OnKeyboardDisconnecting += delegate
            {
                onKeyboardDisconnectingRaised = true;
            };
            keyboardController.OnKeyboardDisconnected += delegate
            {
                onKeyboardDisconnectedRaised = true;
            };

            await keyboardController.RunAsync();

            keyboardController.Nemeio.Should().NotBeNull();
            onKeyboardConnectedRaised.Should().BeTrue();

            Mock.Get(keyboardProvider)
                .Raise(x => x.OnKeyboardDisconnected += null, this, new KeyboardDisconnectedEventArgs(secondKeyboard));

            onKeyboardDisconnectingRaised.Should().BeFalse();
            testableNemeio.StopCalled.Should().BeFalse();
            keyboardController.Nemeio.Should().NotBeNull();
            onKeyboardDisconnectedRaised.Should().BeFalse();
        }

        [Test]
        public async Task KeyboardController_DisconnectNemeio_WhenNemeioRaiseStop_Ok()
        {
            var testSempahore = new SemaphoreSlim(0, 1);

            var onKeyboardConnectedRaised = false;
            var onKeyboardDisconnectingRaised = false;
            var onKeyboardDisconnectedRaised = false;

            var nemeioIdentifier = "MY-TEST-NEMEIO";
            var loggerFactory = new LoggerFactory();

            var keyboard = new Keyboard.Keyboard(nemeioIdentifier, new Version("2.0"), CommunicationType.Serial, Mock.Of<IKeyboardIO>());
            var keyboardProvider = Mock.Of<IKeyboardProvider>();
            Mock.Get(keyboardProvider)
                .Setup(x => x.GetKeyboard(It.IsAny<Func<Keyboard.Keyboard, bool>>()))
                .Returns(() => keyboard);

            var testableNemeio = new TestableNemeio(loggerFactory, nemeioIdentifier, keyboard.ProtocolVersion, It.IsAny<CommunicationType>(), Mock.Of<IKeyboardCommandExecutor>(), Mock.Of<IMonitorFactory>(), Mock.Of<IKeyboardCrashLogger>(), Mock.Of<IRetryHandler>());
            var nemeioBuilder = Mock.Of<INemeioBuilder>();
            Mock.Get(nemeioBuilder)
                .Setup(x => x.BuildAsync(It.IsAny<Keyboard.Keyboard>()))
                .ReturnsAsync(testableNemeio);

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.SessionState)
                .Returns(Core.Systems.Sessions.SessionState.Open);

            var eventStrategyFactory = Mock.Of<INemeioLayoutEventStrategyFactory>();

            var keyboardController = new KeyboardController(loggerFactory, keyboardProvider, nemeioBuilder, system, eventStrategyFactory);
            keyboardController.OnKeyboardConnected += delegate
            {
                onKeyboardConnectedRaised = true;
            };
            keyboardController.OnKeyboardDisconnecting += delegate
            {
                onKeyboardDisconnectingRaised = true;
            };
            keyboardController.OnKeyboardDisconnected += delegate
            {
                onKeyboardDisconnectedRaised = true;

                testSempahore.Release();
            };

            await keyboardController.RunAsync();

            keyboardController.Nemeio.Should().NotBeNull();
            onKeyboardConnectedRaised.Should().BeTrue();

            await testableNemeio.InitializeAsync();

            TestableNemeio nullNemeio = null;
            Mock.Get(nemeioBuilder)
                .Setup(x => x.BuildAsync(It.IsAny<Keyboard.Keyboard>()))
                .ReturnsAsync(nullNemeio);

            await keyboardController.Nemeio.StopAsync();

            var released = await testSempahore.WaitAsync(2000);
            if (!released)
            {
                throw new InvalidOperationException("Semaphore not released");
            }

            onKeyboardDisconnectingRaised.Should().BeTrue();
            testableNemeio.StopCalled.Should().BeTrue();
            keyboardController.Nemeio.Should().BeNull();
            onKeyboardDisconnectedRaised.Should().BeTrue();
        }

        [Test]
        public void KeyboardController_CheckKeyboard_WhenSessionIsOpen_AndSystemIsAwake_AndNoNemeioConnected_Ok()
        {
            var onKeyboardConnectedRaised = false;
            var getKeyboardCalled = false;

            var nemeioIdentifier = "MY-TEST-NEMEIO";
            var loggerFactory = new LoggerFactory();

            var keyboard = new Keyboard.Keyboard(nemeioIdentifier, new System.Version("2.0"), CommunicationType.Serial, Mock.Of<IKeyboardIO>());
            var keyboardProvider = Mock.Of<IKeyboardProvider>();
            Mock.Get(keyboardProvider)
                .Setup(x => x.GetKeyboard(It.IsAny<Func<Keyboard.Keyboard, bool>>()))
                .Returns(() => keyboard)
                .Callback(() => getKeyboardCalled = true);

            var testableNemeio = new TestableNemeio(loggerFactory, nemeioIdentifier, keyboard.ProtocolVersion, It.IsAny<CommunicationType>(), Mock.Of<IKeyboardCommandExecutor>(), Mock.Of<IMonitorFactory>(), Mock.Of<IKeyboardCrashLogger>(), Mock.Of<IRetryHandler>());
            var nemeioBuilder = Mock.Of<INemeioBuilder>();
            Mock.Get(nemeioBuilder)
                .Setup(x => x.BuildAsync(It.IsAny<Keyboard.Keyboard>()))
                .ReturnsAsync(testableNemeio);

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.SessionState)
                .Returns(Core.Systems.Sessions.SessionState.Lock);
            Mock.Get(system)
                .Setup(x => x.SleepMode)
                .Returns(Core.Systems.Sleep.SleepMode.Awake);

            var eventStrategyFactory = Mock.Of<INemeioLayoutEventStrategyFactory>();

            var keyboardController = new KeyboardController(loggerFactory, keyboardProvider, nemeioBuilder, system, eventStrategyFactory);
            keyboardController.OnKeyboardConnected += delegate
            {
                onKeyboardConnectedRaised = true;
            };

            keyboardController.Nemeio.Should().BeNull();
            onKeyboardConnectedRaised.Should().BeFalse();

            Mock.Get(system)
                .Setup(x => x.SessionState)
                .Returns(Core.Systems.Sessions.SessionState.Open);

            Mock.Get(system)
                .Raise(x => x.OnSessionStateChanged += null, EventArgs.Empty);

            getKeyboardCalled.Should().BeTrue();
        }

        [Test]
        public async Task KeyboardController_DisconnectNemeio_WhenSessionIsClosed_AndNemeioIsConnected_Ok()
        {
            var testSempahore = new SemaphoreSlim(0, 1);

            var onKeyboardConnectedRaised = false;
            var onKeyboardDisconnectingRaised = false;
            var onKeyboardDisconnectedRaised = false;

            var nemeioIdentifier = "MY-TEST-NEMEIO";
            var loggerFactory = new LoggerFactory();

            var keyboard = new Keyboard.Keyboard(nemeioIdentifier, new System.Version("2.0"), CommunicationType.Serial, Mock.Of<IKeyboardIO>());
            var keyboardProvider = Mock.Of<IKeyboardProvider>();
            Mock.Get(keyboardProvider)
                .Setup(x => x.GetKeyboard(It.IsAny<Func<Keyboard.Keyboard, bool>>()))
                .Returns(() => keyboard);

            var testableNemeio = new TestableNemeio(loggerFactory, nemeioIdentifier, keyboard.ProtocolVersion, It.IsAny<CommunicationType>(), Mock.Of<IKeyboardCommandExecutor>(), Mock.Of<IMonitorFactory>(), Mock.Of<IKeyboardCrashLogger>(), Mock.Of<IRetryHandler>());
            var nemeioBuilder = Mock.Of<INemeioBuilder>();
            Mock.Get(nemeioBuilder)
                .Setup(x => x.BuildAsync(It.IsAny<Keyboard.Keyboard>()))
                .ReturnsAsync(testableNemeio);

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.SessionState)
                .Returns(Core.Systems.Sessions.SessionState.Open);

            var eventStrategyFactory = Mock.Of<INemeioLayoutEventStrategyFactory>();

            var keyboardController = new KeyboardController(loggerFactory, keyboardProvider, nemeioBuilder, system, eventStrategyFactory);
            keyboardController.OnKeyboardConnected += delegate
            {
                onKeyboardConnectedRaised = true;
            };
            keyboardController.OnKeyboardDisconnecting += delegate
            {
                onKeyboardDisconnectingRaised = true;
            };
            keyboardController.OnKeyboardDisconnected += delegate
            {
                onKeyboardDisconnectedRaised = true;

                testSempahore.Release();
            };

            await keyboardController.RunAsync();

            keyboardController.Nemeio.Should().NotBeNull();
            onKeyboardConnectedRaised.Should().BeTrue();

            TestableNemeio nullNemeio = null;
            Mock.Get(nemeioBuilder)
                .Setup(x => x.BuildAsync(It.IsAny<Keyboard.Keyboard>()))
                .ReturnsAsync(nullNemeio);

            Mock.Get(keyboardProvider)
                .Raise(x => x.OnKeyboardDisconnected += null, this, new KeyboardDisconnectedEventArgs(keyboard));

            var released = await testSempahore.WaitAsync(2000);
            if (!released)
            {
                throw new InvalidOperationException("Semaphore not released");
            }

            onKeyboardDisconnectingRaised.Should().BeTrue();
            testableNemeio.StopCalled.Should().BeTrue();
            keyboardController.Nemeio.Should().BeNull();
            onKeyboardDisconnectedRaised.Should().BeTrue();
        }
    }
}
