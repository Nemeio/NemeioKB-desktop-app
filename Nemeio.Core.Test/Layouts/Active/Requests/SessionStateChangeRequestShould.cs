using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Layouts.Active.Requests;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Sessions;
using NUnit.Framework;

namespace Nemeio.Core.Test.Layouts.Active.Requests
{
    [TestFixture]
    public class SessionStateChangeRequestShould
    {
        [Test]
        public async Task SessionStateChangeRequest_SynchronizeAsync_SessionLock_ProxyIsNull_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var system = Mock.Of<ISystem>();
            var nemeio = Mock.Of<INemeio>();
            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();

            var sessionFirstStrategy = new SessionStateChangeRequest(SessionState.Open, loggerFactory, system, layoutLibrary, nemeio);

            var result = await sessionFirstStrategy.ApplyAsync(null, layoutHistoric, null);

            result.Should().BeNull();
        }

        [Test]
        public async Task SessionStateChangeRequest_SynchronizeAsync_SessionLock_KeyboardLayoutNotFound_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var selectedIdOnKeyboard = new LayoutId("B90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();
            Mock.Get(proxy)
                .Setup(x => x.SelectedLayoutId)
                .Returns(selectedIdOnKeyboard);

            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            Mock.Get(layoutLibrary)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>());

            var system = Mock.Of<ISystem>();
            var nemeio = Mock.Of<INemeio>();
            var sessionFirstStrategy = new SessionStateChangeRequest(SessionState.Lock, loggerFactory, system, layoutLibrary, nemeio);
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();

            var result = await sessionFirstStrategy.ApplyAsync(proxy, layoutHistoric, null);

            result.Should().BeNull();
        }

        [Test]
        public async Task SessionStateChangeRequest_SynchronizeAsync_SessionLock_ChangeSystemLayoutToDefaultLayout()
        {
            var loggerFactory = new LoggerFactory();
            var enforceSystemLayoutCalled = false;
            var defaultSystemLayout = new OsLayoutId("frenchLayout");
            var selectedIdOnKeyboard = new LayoutId("B90F3E15-1E17-426D-97A9-DE07334B9F1E");

            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();
            Mock.Get(proxy)
                .Setup(x => x.SelectedLayoutId)
                .Returns(selectedIdOnKeyboard);

            var frenchLayout = Mock.Of<ILayout>();
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(defaultSystemLayout, false, true));
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutId)
                .Returns(selectedIdOnKeyboard);

            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            Mock.Get(layoutLibrary)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>() { frenchLayout });

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.DefaultLayout)
                .Returns(defaultSystemLayout);
            Mock.Get(system)
                .Setup(x => x.EnforceSystemLayoutAsync(It.IsAny<OsLayoutId>()))
                .Returns(Task.Delay(1))
                .Callback(() => enforceSystemLayoutCalled = true);

            var nemeio = Mock.Of<INemeio>();
            var sessionFirstStrategy = new SessionStateChangeRequest(SessionState.Lock, loggerFactory, system, layoutLibrary, nemeio);
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();

            var result = await sessionFirstStrategy.ApplyAsync(proxy, layoutHistoric, null);

            result.Should().NotBeNull();
            result.Should().Be(frenchLayout);
            enforceSystemLayoutCalled.Should().BeTrue();
        }

        [Test]
        public async Task SessionStateChangeRequest_SynchronizeAsync_SessionUnlock_WhenNeverLockSession_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var enforceSystemLayoutCalled = false;
            var defaultSystemLayout = new OsLayoutId("frenchLayout");
            var selectedIdOnKeyboard = new LayoutId("B90F3E15-1E17-426D-97A9-DE07334B9F1E");

            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();
            Mock.Get(proxy)
                .Setup(x => x.SelectedLayoutId)
                .Returns(selectedIdOnKeyboard);

            var frenchLayout = Mock.Of<ILayout>();
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutId)
                .Returns(selectedIdOnKeyboard);

            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            Mock.Get(layoutLibrary)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>() { frenchLayout });

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.DefaultLayout)
                .Returns(defaultSystemLayout);
            Mock.Get(system)
                .Setup(x => x.EnforceSystemLayoutAsync(It.IsAny<OsLayoutId>()))
                .Returns(Task.Delay(1))
                .Callback(() => enforceSystemLayoutCalled = true);

            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            var nemeio = Mock.Of<INemeio>();
            var sessionFirstStrategy = new SessionStateChangeRequest(SessionState.Open, loggerFactory, system, layoutLibrary, nemeio);

            var result = await sessionFirstStrategy.ApplyAsync(proxy, layoutHistoric, null);

            result.Should().BeNull();
        }

        [Test]
        public async Task SessionStateChangeRequest_SynchronizeAsync_SessionUnlock_SetSystemLayoutToPrevious()
        {
            var loggerFactory = new LoggerFactory();
            var enforceSystemLayoutCalled = false;
            var defaultSystemLayout = new OsLayoutId("frenchLayout");
            var defaultSystemLayoutId = new LayoutId("C90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var selectedIdOnKeyboard = new LayoutId("B90F3E15-1E17-426D-97A9-DE07334B9F1E");

            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();
            Mock.Get(proxy)
                .Setup(x => x.SelectedLayoutId)
                .Returns(selectedIdOnKeyboard);

            var frenchLayout = Mock.Of<ILayout>();
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutId)
                .Returns(selectedIdOnKeyboard);
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutId)
                .Returns(defaultSystemLayoutId);
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(defaultSystemLayout, false, true));

            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            Mock.Get(layoutLibrary)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>() { frenchLayout });

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.DefaultLayout)
                .Returns(defaultSystemLayout);
            Mock.Get(system)
                .Setup(x => x.EnforceSystemLayoutAsync(It.IsAny<OsLayoutId>()))
                .Returns(Task.Delay(1))
                .Callback(() => enforceSystemLayoutCalled = true);

            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            var sessionFirstStrategy = new SessionStateChangeRequest(SessionState.Lock, loggerFactory, system, layoutLibrary, nemeio);

            await sessionFirstStrategy.ApplyAsync(proxy, layoutHistoric, null);

            sessionFirstStrategy = new SessionStateChangeRequest(SessionState.Open, loggerFactory, system, layoutLibrary, nemeio);
            var result = await sessionFirstStrategy.ApplyAsync(proxy, layoutHistoric, frenchLayout);

            result.Should().NotBeNull();
            result.Should().Be(frenchLayout);
            enforceSystemLayoutCalled.Should().BeTrue();
        }
    }
}
