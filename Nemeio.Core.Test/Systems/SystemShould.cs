using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Applications;
using Nemeio.Core.Systems.Hid;
using Nemeio.Core.Systems.Sessions;
using Nemeio.Core.Systems.Sleep;
using Nemeio.Core.Systems.Watchers;
using NUnit.Framework;

namespace Nemeio.Core.Test.Systems
{
    [TestFixture]
    public class SystemShould
    {
        private ISleepModeAdapter _sleepModeAdapter;

        [SetUp]
        public void SetUp()
        {
            _sleepModeAdapter = Mock.Of<ISleepModeAdapter>();
        }

        [Test]
        public void System_Constructor_Ok()
        {
            var hidInteractorRunCalled = false;

            var dummyActiveLayout = new OsLayoutId("dummy-1");
            var dummyLayoutList = new List<OsLayoutId>()
            {
                dummyActiveLayout,
                new OsLayoutId("dummy-2"),
                new OsLayoutId("dummy-3")
            };

            var loggerFactory = new LoggerFactory();
            var activeLayoutWatcher = Mock.Of<ISystemActiveLayoutWatcher>();
            Mock.Get(activeLayoutWatcher)
                .Setup(x => x.CurrentSystemLayoutId)
                .Returns(dummyActiveLayout);

            var loaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            Mock.Get(loaderAdapter)
                .Setup(x => x.Load())
                .Returns(() => dummyLayoutList);

            var systemLayoutInteractor = Mock.Of<ISystemLayoutInteractor>();

            var activeLayoutAdapter = Mock.Of<ISystemActiveLayoutAdapter>();
            var layoutsWatcher = Mock.Of<ISystemLayoutWatcher>();

            var hidInteractor = Mock.Of<ISystemHidInteractor>();
            Mock.Get(hidInteractor)
                .Setup(x => x.Run())
                .Callback(() => hidInteractorRunCalled = true);

            var foregroundAppAdapter = Mock.Of<ISystemForegroundApplicationAdapter>();
            var sessionStateWatcher = Mock.Of<ISystemSessionStateWatcher>();

            Assert.DoesNotThrow(() => new Core.Systems.System(loggerFactory, activeLayoutWatcher, loaderAdapter, systemLayoutInteractor, activeLayoutAdapter, layoutsWatcher, hidInteractor, foregroundAppAdapter, sessionStateWatcher, _sleepModeAdapter));

            var system = new Core.Systems.System(loggerFactory, activeLayoutWatcher, loaderAdapter, systemLayoutInteractor, activeLayoutAdapter, layoutsWatcher, hidInteractor, foregroundAppAdapter, sessionStateWatcher, _sleepModeAdapter);

            //  Must load layouts list
            system.Layouts.Count.Should().Be(dummyLayoutList.Count);
            system.Layouts.Should().BeEquivalentTo(dummyLayoutList);

            //  Must load active layout
            system.SelectedLayout.Should().NotBeNull();
            system.SelectedLayout.Should().Be(dummyActiveLayout);

            //  Must start HID interactor
            hidInteractorRunCalled.Should().BeTrue();
        }

        [Test]
        public void System_LayoutListUpdate_AndNotify_WhenAdapterDetectChanges_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var activeLayoutWatcher = Mock.Of<ISystemActiveLayoutWatcher>();

            var loaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            Mock.Get(loaderAdapter)
                .Setup(x => x.Load())
                .Returns(new List<OsLayoutId>() { new OsLayoutId("dummy-2"), new OsLayoutId("dummy-3") });

            var systemLayoutInteractor = Mock.Of<ISystemLayoutInteractor>();
            var activeLayoutAdapter = Mock.Of<ISystemActiveLayoutAdapter>();

            var layoutsWatcher = Mock.Of<ISystemLayoutWatcher>();
            var hidInteractor = Mock.Of<ISystemHidInteractor>();
            var foregroundAppAdapter = Mock.Of<ISystemForegroundApplicationAdapter>();
            var sessionStateWatcher = Mock.Of<ISystemSessionStateWatcher>();

            var system = new Core.Systems.System(loggerFactory, activeLayoutWatcher, loaderAdapter, systemLayoutInteractor, activeLayoutAdapter, layoutsWatcher, hidInteractor, foregroundAppAdapter, sessionStateWatcher, _sleepModeAdapter);

            system.Layouts.Count.Should().Be(loaderAdapter.Load().Count());

            Mock.Get(loaderAdapter)
                .Setup(x => x.Load())
                .Returns(new List<OsLayoutId>() { new OsLayoutId("dummy-2"), new OsLayoutId("dummy-3"), new OsLayoutId("dummy-4") });

            Mock.Get(layoutsWatcher)
                .Raise(x => x.LayoutChanged += null, EventArgs.Empty);

            system.Layouts.Count.Should().Be(loaderAdapter.Load().Count());
        }

        [Test]
        public void System_ActiveLayoutUpdate_AndNotify_WhenAdapterDetectChanges_Ok()
        {
            var firstActiveLayout = new OsLayoutId("first");
            var secondActiveLayout = new OsLayoutId("second");

            var loggerFactory = new LoggerFactory();
            var activeLayoutWatcher = Mock.Of<ISystemActiveLayoutWatcher>();
            Mock.Get(activeLayoutWatcher)
                .Setup(x => x.CurrentSystemLayoutId)
                .Returns(firstActiveLayout);

            var loaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            var systemLayoutInteractor = Mock.Of<ISystemLayoutInteractor>();
            var activeLayoutAdapter = Mock.Of<ISystemActiveLayoutAdapter>();
            var layoutsWatcher = Mock.Of<ISystemLayoutWatcher>();
            var hidInteractor = Mock.Of<ISystemHidInteractor>();
            var foregroundAppAdapter = Mock.Of<ISystemForegroundApplicationAdapter>();
            var sessionStateWatcher = Mock.Of<ISystemSessionStateWatcher>();

            var system = new Core.Systems.System(loggerFactory, activeLayoutWatcher, loaderAdapter, systemLayoutInteractor, activeLayoutAdapter, layoutsWatcher, hidInteractor, foregroundAppAdapter, sessionStateWatcher, _sleepModeAdapter);

            system.SelectedLayout.Should().Be(firstActiveLayout);

            Mock.Get(activeLayoutWatcher)
                .Setup(x => x.CurrentSystemLayoutId)
                .Returns(secondActiveLayout);

            Mock.Get(activeLayoutWatcher)
                .Raise(x => x.OnSystemLayoutChanged += null, EventArgs.Empty);

            system.SelectedLayout.Should().Be(secondActiveLayout);
        }

        [Test]
        public void System_ForegroundApplicationUpdate_AndNotify_WhenAdapterDetectChanges_Ok()
        {
            var foregroundApplicationChangedTrigger = false;
            var focusForegroundApplication = new Application() { Name = "myFocusApplication" };

            var loggerFactory = new LoggerFactory();
            var activeLayoutWatcher = Mock.Of<ISystemActiveLayoutWatcher>();
            var loaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            var systemLayoutInteractor = Mock.Of<ISystemLayoutInteractor>();
            var activeLayoutAdapter = Mock.Of<ISystemActiveLayoutAdapter>();
            var layoutsWatcher = Mock.Of<ISystemLayoutWatcher>();
            var hidInteractor = Mock.Of<ISystemHidInteractor>();
            var foregroundAppAdapter = Mock.Of<ISystemForegroundApplicationAdapter>();
            var sessionStateWatcher = Mock.Of<ISystemSessionStateWatcher>();

            var system = new Core.Systems.System(loggerFactory, activeLayoutWatcher, loaderAdapter, systemLayoutInteractor, activeLayoutAdapter, layoutsWatcher, hidInteractor, foregroundAppAdapter, sessionStateWatcher, _sleepModeAdapter);
            system.OnForegroundApplicationChanged += delegate
            {
                foregroundApplicationChangedTrigger = true;
            };

            system.ForegroundApplication.Should().BeNull();

            Mock.Get(foregroundAppAdapter)
                .Raise(x => x.OnApplicationChanged += null, this, new ApplicationChangedEventArgs(focusForegroundApplication));

            system.ForegroundApplication.Should().Be(focusForegroundApplication);
            foregroundApplicationChangedTrigger.Should().BeTrue();
        }

        [Test]
        public void System_SessionState_DefaultValue_MustBeOpen()
        {
            var loggerFactory = new LoggerFactory();
            var activeLayoutWatcher = Mock.Of<ISystemActiveLayoutWatcher>();
            var loaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            var systemLayoutInteractor = Mock.Of<ISystemLayoutInteractor>();
            var activeLayoutAdapter = Mock.Of<ISystemActiveLayoutAdapter>();
            var layoutsWatcher = Mock.Of<ISystemLayoutWatcher>();
            var hidInteractor = Mock.Of<ISystemHidInteractor>();
            var foregroundAppAdapter = Mock.Of<ISystemForegroundApplicationAdapter>();
            var sessionStateWatcher = Mock.Of<ISystemSessionStateWatcher>();

            var system = new Core.Systems.System(loggerFactory, activeLayoutWatcher, loaderAdapter, systemLayoutInteractor, activeLayoutAdapter, layoutsWatcher, hidInteractor, foregroundAppAdapter, sessionStateWatcher, _sleepModeAdapter);

            system.SessionState.Should().Be(SessionState.Open);
        }

        [Test]
        public void System_SessionStateUpdate_AndNotify_WhenAdapterDetectChanges_Ok()
        {
            var sessionStateChangeTriggered = false;

            var loggerFactory = new LoggerFactory();
            var activeLayoutWatcher = Mock.Of<ISystemActiveLayoutWatcher>();
            var loaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            var systemLayoutInteractor = Mock.Of<ISystemLayoutInteractor>();
            var activeLayoutAdapter = Mock.Of<ISystemActiveLayoutAdapter>();
            var layoutsWatcher = Mock.Of<ISystemLayoutWatcher>();
            var hidInteractor = Mock.Of<ISystemHidInteractor>();
            var foregroundAppAdapter = Mock.Of<ISystemForegroundApplicationAdapter>();
            var sessionStateWatcher = Mock.Of<ISystemSessionStateWatcher>();

            var system = new Core.Systems.System(loggerFactory, activeLayoutWatcher, loaderAdapter, systemLayoutInteractor, activeLayoutAdapter, layoutsWatcher, hidInteractor, foregroundAppAdapter, sessionStateWatcher, _sleepModeAdapter);
            system.OnSessionStateChanged += delegate
            {
                sessionStateChangeTriggered = true;
            };

            system.SessionState.Should().Be(SessionState.Open);

            Mock.Get(sessionStateWatcher)
                .Raise(x => x.StateChanged += null, this, new SessionStateChangedEventArgs(SessionState.Lock));

            system.SessionState.Should().Be(SessionState.Lock);
            sessionStateChangeTriggered.Should().BeTrue();
        }

        [Test]
        public void System_SleepMode_DefaultValue_MustBeAwake()
        {
            var loggerFactory = new LoggerFactory();
            var activeLayoutWatcher = Mock.Of<ISystemActiveLayoutWatcher>();
            var loaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            var systemLayoutInteractor = Mock.Of<ISystemLayoutInteractor>();
            var activeLayoutAdapter = Mock.Of<ISystemActiveLayoutAdapter>();
            var layoutsWatcher = Mock.Of<ISystemLayoutWatcher>();
            var hidInteractor = Mock.Of<ISystemHidInteractor>();
            var foregroundAppAdapter = Mock.Of<ISystemForegroundApplicationAdapter>();
            var sessionStateWatcher = Mock.Of<ISystemSessionStateWatcher>();

            var system = new Core.Systems.System(loggerFactory, activeLayoutWatcher, loaderAdapter, systemLayoutInteractor, activeLayoutAdapter, layoutsWatcher, hidInteractor, foregroundAppAdapter, sessionStateWatcher, _sleepModeAdapter);

            system.SleepMode.Should().Be(SleepMode.Awake);
        }

        [Test]
        public void System_SleepModeUpdate_AndNotify_WhenAdapterDetectChanges_Ok()
        {
            var sleepModeChangeTriggered = false;

            var loggerFactory = new LoggerFactory();
            var activeLayoutWatcher = Mock.Of<ISystemActiveLayoutWatcher>();
            var loaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            var systemLayoutInteractor = Mock.Of<ISystemLayoutInteractor>();
            var activeLayoutAdapter = Mock.Of<ISystemActiveLayoutAdapter>();
            var layoutsWatcher = Mock.Of<ISystemLayoutWatcher>();
            var hidInteractor = Mock.Of<ISystemHidInteractor>();
            var foregroundAppAdapter = Mock.Of<ISystemForegroundApplicationAdapter>();
            var sessionStateWatcher = Mock.Of<ISystemSessionStateWatcher>();

            var system = new Core.Systems.System(loggerFactory, activeLayoutWatcher, loaderAdapter, systemLayoutInteractor, activeLayoutAdapter, layoutsWatcher, hidInteractor, foregroundAppAdapter, sessionStateWatcher, _sleepModeAdapter);
            system.OnSleepModeChanged += delegate
            {
                sleepModeChangeTriggered = true;
            };

            system.SleepMode.Should().Be(SleepMode.Awake);

            Mock.Get(_sleepModeAdapter)
                .Raise(x => x.OnSleepModeChanged += null, this, new SleepModeChangedEventArgs(SleepMode.Sleep));

            system.SleepMode.Should().Be(SleepMode.Sleep);
            sleepModeChangeTriggered.Should().BeTrue();
        }

        [Test]
        public void System_Stop_Ok()
        {
            var activeLayoutWatcherStopCalled = false;
            var hidInteractorStopCalled = false;

            var loggerFactory = new LoggerFactory();
            var activeLayoutWatcher = Mock.Of<ISystemActiveLayoutWatcher>();
            Mock.Get(activeLayoutWatcher)
                .Setup(x => x.Stop())
                .Callback(() => activeLayoutWatcherStopCalled = true);

            var loaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            var systemLayoutInteractor = Mock.Of<ISystemLayoutInteractor>();
            var activeLayoutAdapter = Mock.Of<ISystemActiveLayoutAdapter>();
            var layoutsWatcher = Mock.Of<ISystemLayoutWatcher>();

            var hidInteractor = Mock.Of<ISystemHidInteractor>();
            Mock.Get(hidInteractor)
                .Setup(x => x.Stop())
                .Callback(() => hidInteractorStopCalled = true);

            var foregroundAppAdapter = Mock.Of<ISystemForegroundApplicationAdapter>();
            var sessionStateWatcher = Mock.Of<ISystemSessionStateWatcher>();

            var system = new Core.Systems.System(loggerFactory, activeLayoutWatcher, loaderAdapter, systemLayoutInteractor, activeLayoutAdapter, layoutsWatcher, hidInteractor, foregroundAppAdapter, sessionStateWatcher, _sleepModeAdapter);

            system.Stop();

            activeLayoutWatcherStopCalled.Should().BeTrue();
            hidInteractorStopCalled.Should().BeTrue();
        }

        [Test]
        public async Task System_EnforceSystemLayoutAsync_WhenTargetIsSameHasCurrent_DoNothing_Ok()
        {
            var onSelectedLayoutChangedCalled = false;
            var dummyOne = new OsLayoutId("dummy1");

            var loggerFactory = new LoggerFactory();

            var activeLayoutWatcher = Mock.Of<ISystemActiveLayoutWatcher>();
            Mock.Get(activeLayoutWatcher)
                .Setup(x => x.CurrentSystemLayoutId)
                .Returns(dummyOne);

            var loaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            var systemLayoutInteractor = Mock.Of<ISystemLayoutInteractor>();
            var activeLayoutAdapter = Mock.Of<ISystemActiveLayoutAdapter>();
            var layoutsWatcher = Mock.Of<ISystemLayoutWatcher>();
            var hidInteractor = Mock.Of<ISystemHidInteractor>();
            var foregroundAppAdapter = Mock.Of<ISystemForegroundApplicationAdapter>();
            var sessionStateWatcher = Mock.Of<ISystemSessionStateWatcher>();

            var system = new Core.Systems.System(loggerFactory, activeLayoutWatcher, loaderAdapter, systemLayoutInteractor, activeLayoutAdapter, layoutsWatcher, hidInteractor, foregroundAppAdapter, sessionStateWatcher, _sleepModeAdapter);
            system.OnSelectedLayoutChanged += delegate
            {
                onSelectedLayoutChangedCalled = true;
            };

            await system.EnforceSystemLayoutAsync(dummyOne);

            system.SelectedLayout.Should().Be(dummyOne);
            onSelectedLayoutChangedCalled.Should().BeFalse();
        }

        [Test]
        public async Task System_EnforceSystemLayoutAsync_WithNewTarget_UpdateSystemLayout_Ok()
        {
            var onSelectedLayoutChangedCalled = false;
            var dummyOne = new OsLayoutId("dummy1");
            var dummyTwo = new OsLayoutId("dummy2");

            var loggerFactory = new LoggerFactory();

            var activeLayoutWatcher = Mock.Of<ISystemActiveLayoutWatcher>();
            Mock.Get(activeLayoutWatcher)
                .Setup(x => x.CurrentSystemLayoutId)
                .Returns(dummyOne);

            var loaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            var systemLayoutInteractor = Mock.Of<ISystemLayoutInteractor>();
            var activeLayoutAdapter = Mock.Of<ISystemActiveLayoutAdapter>();
            var layoutsWatcher = Mock.Of<ISystemLayoutWatcher>();
            var hidInteractor = Mock.Of<ISystemHidInteractor>();
            var foregroundAppAdapter = Mock.Of<ISystemForegroundApplicationAdapter>();
            var sessionStateWatcher = Mock.Of<ISystemSessionStateWatcher>();

            var system = new Core.Systems.System(loggerFactory, activeLayoutWatcher, loaderAdapter, systemLayoutInteractor, activeLayoutAdapter, layoutsWatcher, hidInteractor, foregroundAppAdapter, sessionStateWatcher, _sleepModeAdapter);
            system.OnSelectedLayoutChanged += delegate
            {
                onSelectedLayoutChangedCalled = true;
            };

            await system.EnforceSystemLayoutAsync(dummyTwo);

            system.SelectedLayout.Should().Be(dummyTwo);
            onSelectedLayoutChangedCalled.Should().BeTrue();
        }
    }
}
