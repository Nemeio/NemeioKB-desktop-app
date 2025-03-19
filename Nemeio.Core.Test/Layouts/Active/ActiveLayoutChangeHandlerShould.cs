using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Configurations;
using Nemeio.Core.Keyboard.Configurations.Apply;
using Nemeio.Core.Keyboard.Connection;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Layouts.Active.Requests.Base;
using Nemeio.Core.Layouts.Active.Requests.Factories;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Applications;
using Nemeio.Core.Systems.Sessions;
using Nemeio.Core.Tools.Retry;
using NUnit.Framework;

namespace Nemeio.Core.Test.Layouts.Active
{
    [TestFixture]
    internal class ActiveLayoutChangeHandlerShould
    {
        private SemaphoreSlim _testSemaphore = new SemaphoreSlim(0, 1);
        private TimeSpan _defaultTimeout = TimeSpan.FromSeconds(5);

        private ISystem _mockSystem;
        private IKeyboardController _mockKeyboardController;
        private IChangeRequestFactory _mockChangeRequestFactory;
        private IActiveLayoutSynchronizer _mockActiveLayoutSynchronizer;
        private INemeio _mockNemeio;

        [SetUp]
        public void SetUp()
        {
            _mockSystem = Mock.Of<ISystem>();
            _mockKeyboardController = Mock.Of<IKeyboardController>();
            _mockChangeRequestFactory = Mock.Of<IChangeRequestFactory>();
            _mockActiveLayoutSynchronizer = Mock.Of<IActiveLayoutSynchronizer>();
            _mockNemeio = Mock.Of<INemeio>();

            var mockPostRequest = Mock.Of<IChangeRequest>();

            Mock.Get(_mockChangeRequestFactory)
                .Setup(x => x.CreateApplicationShutdownRequest(_mockNemeio))
                .Returns(mockPostRequest);
            Mock.Get(_mockChangeRequestFactory)
                .Setup(x => x.CreateForegroundApplicationRequest(_mockNemeio, It.IsAny<Application>()))
                .Returns(mockPostRequest);
            Mock.Get(_mockChangeRequestFactory)
                .Setup(x => x.CreateHidSystemRequest(_mockNemeio))
                .Returns(mockPostRequest);
            Mock.Get(_mockChangeRequestFactory)
                .Setup(x => x.CreateHistoricRequest(_mockNemeio, It.IsAny<bool>()))
                .Returns(mockPostRequest);
            Mock.Get(_mockChangeRequestFactory)
                .Setup(x => x.CreateKeyboardSelectionRequest(_mockNemeio))
                .Returns(mockPostRequest);
            Mock.Get(_mockChangeRequestFactory)
                .Setup(x => x.CreateKeyPressRequest(_mockNemeio, It.IsAny<LayoutId>()))
                .Returns(mockPostRequest);
            Mock.Get(_mockChangeRequestFactory)
                .Setup(x => x.CreateMenuRequest(_mockNemeio, It.IsAny<LayoutId>()))
                .Returns(mockPostRequest);
            Mock.Get(_mockChangeRequestFactory)
                .Setup(x => x.CreateSessionRequest(_mockNemeio, It.IsAny<SessionState>()))
                .Returns(mockPostRequest);
        }

        [Test]
        public async Task ActiveLayoutChangeHandler_RequestApplicationShutdownAsync_AddOnQueue_Ok()
        {
            await Test_RequestAddOnQueueAsync((activeLayoutChangeHandler) =>
            {
                return activeLayoutChangeHandler.RequestApplicationShutdownAsync(_mockNemeio);
            });
        }

        [Test]
        public async Task ActiveLayoutChangeHandler_RequestForegroundApplicationChangeAsync_AddOnQueue_Ok()
        {
            await Test_RequestAddOnQueueAsync((activeLayoutChangeHandler) =>
            {
                var fakeApplication = new Application();

                return activeLayoutChangeHandler.RequestForegroundApplicationChangeAsync(_mockNemeio, fakeApplication);
            });
        }

        [Test]
        public async Task ActiveLayoutChangeHandler_RequestHidSystemChangeAsync_AddOnQueue_Ok()
        {
            await Test_RequestAddOnQueueAsync((activeLayoutChangeHandler) =>
            {
                return activeLayoutChangeHandler.RequestHidSystemChangeAsync(_mockNemeio);
            });
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ActiveLayoutChangeHandler_RequestHistoricChangeAsync_AddOnQueue_Ok(bool value)
        {
            await Test_RequestAddOnQueueAsync((activeLayoutChangeHandler) =>
            {
                return activeLayoutChangeHandler.RequestHistoricChangeAsync(_mockNemeio, value);
            });
        }

        [Test]
        public async Task ActiveLayoutChangeHandler_RequestKeyboardSelectionChangeAsync_AddOnQueue_Ok()
        {
            await Test_RequestAddOnQueueAsync((activeLayoutChangeHandler) =>
            {
                return activeLayoutChangeHandler.RequestKeyboardSelectionChangeAsync(_mockNemeio);
            });
        }

        [Test]
        public async Task ActiveLayoutChangeHandler_RequestKeyPressChangeAsync_AddOnQueue_Ok()
        {
            await Test_RequestAddOnQueueAsync((activeLayoutChangeHandler) =>
            {
                var id = new LayoutId("D06A7566-B89D-43B3-B4D5-64BCB5EA241C");

                return activeLayoutChangeHandler.RequestKeyPressChangeAsync(_mockNemeio, id);
            });
        }

        [Test]
        public async Task ActiveLayoutChangeHandler_RequestMenuChangeAsync_AddOnQueue_Ok()
        {
            await Test_RequestAddOnQueueAsync((activeLayoutChangeHandler) =>
            {
                var id = new LayoutId("D06A7566-B89D-43B3-B4D5-64BCB5EA241C");

                return activeLayoutChangeHandler.RequestMenuChangeAsync(_mockNemeio, id);
            });
        }

        [Test]
        public async Task ActiveLayoutChangeHandler_RequestSessionChangeAsync_AddOnQueue_Ok([Values] SessionState state)
        {
            await Test_RequestAddOnQueueAsync((activeLayoutChangeHandler) =>
            {
                return activeLayoutChangeHandler.RequestSessionChangeAsync(_mockNemeio, state);
            });
        }

        [Test]
        public async Task ActiveLayoutChangeHandler_WhenSystemLayoutChanged_AndAlreadySyncing_PostRequest_DoNothing()
        {
            var postRequestAsyncCalled = false;

            Mock.Get(_mockSystem)
                .Setup(x => x.SessionState)
                .Returns(SessionState.Open);

            Mock.Get(_mockActiveLayoutSynchronizer)
                .Setup(x => x.PostRequestAsync(It.IsAny<IChangeRequest>()))
                .Callback<IChangeRequest>((request) =>
                {
                    postRequestAsyncCalled = true;
                })
                .Returns(Task.Delay(1));

            Mock.Get(_mockActiveLayoutSynchronizer)
                .Setup(x => x.Syncing)
                .Returns(true);

            var loggerFactory = new LoggerFactory();
            var activeLayoutChangeHandler = new ActiveLayoutChangeHandler(loggerFactory, _mockSystem, _mockChangeRequestFactory, _mockActiveLayoutSynchronizer);

            Mock.Get(_mockSystem)
                .Raise(x => x.OnSelectedLayoutChanged += null, EventArgs.Empty);

            await Task.Delay(_defaultTimeout);

            postRequestAsyncCalled.Should().BeFalse();
        }

        [Test]
        public async Task ActiveLayoutChangeHandler_WhenKeyboardLayoutChanged_AndAlreadySyncing_PostRequest_DoNothing()
        {
            var postRequestAsyncCalled = false;

            Mock.Get(_mockSystem)
                .Setup(x => x.SessionState)
                .Returns(SessionState.Open);

            Mock.Get(_mockActiveLayoutSynchronizer)
                .Setup(x => x.PostRequestAsync(It.IsAny<IChangeRequest>()))
                .Callback<IChangeRequest>((request) =>
                {
                    postRequestAsyncCalled = true;
                })
                .Returns(Task.Delay(1));

            Mock.Get(_mockActiveLayoutSynchronizer)
                .Setup(x => x.Syncing)
                .Returns(true);

            var loggerFactory = new LoggerFactory();
            var activeLayoutChangeHandler = new ActiveLayoutChangeHandler(loggerFactory, _mockSystem, _mockChangeRequestFactory, _mockActiveLayoutSynchronizer);

            var commandExecutor = Mock.Of<IKeyboardCommandExecutor>();
            var monitorFactory = Mock.Of<IMonitorFactory>();
            var crashLogger = Mock.Of<IKeyboardCrashLogger>();
            var retryHandler = Mock.Of<IRetryHandler>();
            var nemeio = new TestableNemeio(loggerFactory, "0001", new Version("3.2"), CommunicationType.Serial, commandExecutor, monitorFactory, crashLogger, retryHandler);

            Mock.Get(_mockKeyboardController)
                .Setup(x => x.Connected)
                .Returns(true);

            Mock.Get(_mockKeyboardController)
                .Setup(x => x.Nemeio)
                .Returns(nemeio);

            Mock.Get(_mockKeyboardController)
                .Raise(x => x.OnKeyboardConnected += null, new KeyboardStatusChangedEventArgs(nemeio));

            nemeio.ForceOnSelectedLayoutChanged();

            await Task.Delay(_defaultTimeout);

            postRequestAsyncCalled.Should().BeFalse();
        }

        private async Task Test_RequestAddOnQueueAsync(Func<ActiveLayoutChangeHandler, Task> requestTask)
        {
            IChangeRequest postedRequest = null;

            var loggerFactory = new LoggerFactory();
            Mock.Get(_mockSystem)
                .Setup(x => x.SessionState)
                .Returns(SessionState.Open);

            Mock.Get(_mockActiveLayoutSynchronizer)
                .Setup(x => x.PostRequestAsync(It.IsAny<IChangeRequest>()))
                .Callback<IChangeRequest>((request) =>
                {
                    postedRequest = request;
                    _testSemaphore.Release();
                })
                .Returns(Task.Delay(1));

            var activeLayoutChangeHandler = new ActiveLayoutChangeHandler(loggerFactory, _mockSystem, _mockChangeRequestFactory, _mockActiveLayoutSynchronizer);

            await requestTask(activeLayoutChangeHandler);

            await WaitOrThrowAsync(_defaultTimeout);

            postedRequest.Should().NotBeNull();
        }

        private async Task WaitOrThrowAsync(TimeSpan timeout)
        {
            var isOk = await _testSemaphore.WaitAsync(timeout);
            if (!isOk)
            {
                throw new InvalidOperationException("Semaphore release by timeout");
            }
        }

        private sealed class TestableNemeio : Keyboard.Nemeio, IConfigurationHolder, IApplyConfigurationHolder
        {
            public TestableNemeio(ILoggerFactory loggerFactory, string identifier, Version protocolVersion, CommunicationType type, IKeyboardCommandExecutor commandExecutor, IMonitorFactory monitorFactory, IKeyboardCrashLogger crashLogger, IRetryHandler retryHandler)
                : base(loggerFactory, identifier, protocolVersion, type, commandExecutor, monitorFactory, crashLogger, retryHandler) { }

            public IScreen Screen => throw new NotImplementedException();
            public IList<LayoutIdWithHash> LayoutIdWithHashs => throw new NotImplementedException();
            public LayoutId SelectedLayoutId { get; private set; } = new LayoutId("7053067E-93D9-49B4-94D8-5776FDE8D14F");

            public event EventHandler OnSelectedLayoutChanged;

            protected override Task InitKeyboardAsync()
            {
                //  Nothing to do here

                return Task.Delay(1);
            }

            public Task StartSynchronizationAsync()
            {
                throw new NotImplementedException();
            }

            public Task EndSynchronizationAsync()
            {
                throw new NotImplementedException();
            }

            public void ForceOnSelectedLayoutChanged() => OnSelectedLayoutChanged?.Invoke(this, EventArgs.Empty);

            public Task ApplyLayoutAsync(ILayout layout)
            {
                //  Nothing to do here

                return Task.Delay(1);
            }
        }
    }
}
