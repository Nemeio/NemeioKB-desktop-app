using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Configurations;
using Nemeio.Core.Keyboard.Configurations.Add;
using Nemeio.Core.Keyboard.Configurations.Delete;
using Nemeio.Core.Keyboard.Connection;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Layouts.Synchronization;
using Nemeio.Core.Layouts.Synchronization.Contexts;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Core.Tools.Retry;
using NUnit.Framework;
using static Nemeio.Core.Layouts.Synchronization.Synchronizer;

namespace Nemeio.Core.Test.Layouts.Synchronization
{
    [TestFixture]
    public class SynchronizerShould
    {
        private class TestableNemeio : Keyboard.Nemeio, IConfigurationHolder, IAddConfigurationHolder, IDeleteConfigurationHolder
        {
            public bool InitKeyboardAsyncCalled { get; private set; }
            public bool StopCalled { get; private set; }
            public IList<LayoutIdWithHash> LayoutIdWithHashs => new List<LayoutIdWithHash>();
            public LayoutId SelectedLayoutId => null;
            public IScreen Screen => Mock.Of<IScreen>();

            public TestableNemeio(ILoggerFactory loggerFactory, string identifier, Version protocolVersion, CommunicationType type, IKeyboardCommandExecutor commandExecutor, IMonitorFactory monitorFactory, IKeyboardCrashLogger crashLogger, IRetryHandler retryHandler)
                : base(loggerFactory, identifier, protocolVersion, type, commandExecutor, monitorFactory, crashLogger, retryHandler)
            {
                _stateMachine.Configure(NemeioState.Connected)
                    .Permit(NemeioTrigger.StartSync, NemeioState.Sync);
            }

            public event EventHandler OnSelectedLayoutChanged;

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

            public async Task StartSynchronizationAsync() => await Task.Yield();
            public async Task EndSynchronizationAsync() => await Task.Yield();
            public async Task AddLayoutAsync(ILayout layout) => await Task.Yield();
            public async Task DeleteLayoutAsync(LayoutId layoutId) => await Task.Yield();

            public async Task ForceSwitchToSyncStateAsync() => await _stateMachine.FireAsync(NemeioTrigger.StartSync);
        }

        [Test]
        public async Task Synchronizer_NemeioSyncState_TriggerSynchronization_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var system = Mock.Of<ISystem>();
            var screenFactory = Mock.Of<IScreenFactory>();

            var contextFactory = Mock.Of<ISynchronizationContextFactory>();
            Mock.Get(contextFactory)
                .Setup(x => x.CreateSystemSynchronizationContext(It.IsAny<IScreen>()))
                .Returns(Mock.Of<ISynchronizationContext>());

            var testableNemeio = new TestableNemeio(loggerFactory, "myId", new Version("2.0"), It.IsAny<CommunicationType>(), Mock.Of<IKeyboardCommandExecutor>(), Mock.Of<IMonitorFactory>(), Mock.Of<IKeyboardCrashLogger>(), Mock.Of<IRetryHandler>());
            var keyboardController = Mock.Of<IKeyboardController>();
            Mock.Get(keyboardController)
                .Setup(x => x.Nemeio)
                .Returns(testableNemeio);

            var findSyncState = false;
            var synchronizer = new Synchronizer(loggerFactory, system, contextFactory, keyboardController, screenFactory);
            synchronizer.OnStateChanged += delegate
            {
                if (synchronizer.State == SynchornizerState.Syncing)
                {
                    findSyncState = true;
                }
            };

            Mock.Get(keyboardController)
                .Raise(x => x.OnKeyboardConnected += null, this, new KeyboardStatusChangedEventArgs(testableNemeio));

            await Task.Run(async () =>
            {
                await testableNemeio.ForceSwitchToSyncStateAsync();

                await Task.Delay(200);

                findSyncState.Should().BeTrue();
            });
        }

        [Test]
        public async Task Synchronizer_SystemLayoutChanged_TriggerSynchronization_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var system = Mock.Of<ISystem>();
            var screenFactory = Mock.Of<IScreenFactory>();

            var needSyncOnSystem = false;

            var systemSyncContext = Mock.Of<ISynchronizationContext>();
            Mock.Get(systemSyncContext)
                .Setup(x => x.SyncAsync())
                .Returns(Task.Delay(1000));
            Mock.Get(systemSyncContext)
                .Setup(x => x.NeedSynchronization())
                .Callback(() => needSyncOnSystem = true);

            var testableNemeio = new TestableNemeio(loggerFactory, "myId", new Version("2.0"), It.IsAny<CommunicationType>(), Mock.Of<IKeyboardCommandExecutor>(), Mock.Of<IMonitorFactory>(), Mock.Of<IKeyboardCrashLogger>(), Mock.Of<IRetryHandler>());
            var keyboardController = Mock.Of<IKeyboardController>();
            Mock.Get(keyboardController)
                .Setup(x => x.Nemeio)
                .Returns(testableNemeio);

            var contextFactory = Mock.Of<ISynchronizationContextFactory>();
            Mock.Get(contextFactory)
                .Setup(x => x.CreateSystemSynchronizationContext(It.IsAny<IScreen>()))
                .Returns(() =>
                {
                    return systemSyncContext;
                });

            var findSyncState = false;
            var synchronizer = new Synchronizer(loggerFactory, system, contextFactory, keyboardController, screenFactory);
            synchronizer.OnStateChanged += delegate
            {
                if (synchronizer.State == SynchornizerState.Syncing)
                {
                    findSyncState = true;
                }
            };

            Mock.Get(keyboardController)
                .Raise(x => x.OnKeyboardConnected += null, this, new KeyboardStatusChangedEventArgs(testableNemeio));

            await Task.Run(async () =>
            {
                Mock.Get(system)
                    .Raise(x => x.OnLayoutsChanged += null, EventArgs.Empty);

                await Task.Delay(100);

                findSyncState.Should().BeTrue();
                needSyncOnSystem.Should().BeTrue();
            });
        }
    }
}
