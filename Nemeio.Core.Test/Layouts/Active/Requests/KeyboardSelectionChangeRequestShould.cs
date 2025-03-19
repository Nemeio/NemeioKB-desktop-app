using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Layouts.Active.Requests;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using NUnit.Framework;

namespace Nemeio.Core.Test.Layouts.Active.Requests
{
    [TestFixture]
    public class KeyboardSelectionChangeRequestShould
    {
        [Test]
        public async Task KeyboardSelectionChangeRequest_SynchronizeAsync_WhenProxyIsNull_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            var system = Mock.Of<ISystem>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();

            var nemeio = Mock.Of<INemeio>();
            var changeRequest = new KeyboardSelectionChangeRequest(loggerFactory, system, layoutLibrary, nemeio);

            var result = await changeRequest.ApplyAsync(null, layoutHistoric, null);

            result.Should().BeNull();
        }

        [Test]
        public async Task KeyboardSelectionChangeRequest_SynchronizeAsync_WhenKeyboardLayoutIsNotFound_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var enforceSystemLayoutCalled = false;

            var selectedLayoutId = new LayoutId("B90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var layoutId = new LayoutId("B90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var osLayoutId = new OsLayoutId("frenchLayout");

            var frenchLayout = Mock.Of<ILayout>();
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutId)
                .Returns(layoutId);
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutId)
                .Returns(selectedLayoutId);
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(osLayoutId, false, true));

            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            Mock.Get(layoutLibrary)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>());

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.EnforceSystemLayoutAsync(It.IsAny<OsLayoutId>()))
                .Returns(Task.Delay(1))
                .Callback(() => enforceSystemLayoutCalled = true);

            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();
            Mock.Get(proxy)
                .Setup(x => x.SelectedLayoutId)
                .Returns(selectedLayoutId);

            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            var changeRequest = new KeyboardSelectionChangeRequest(loggerFactory, system, layoutLibrary, nemeio);

            var result = await changeRequest.ApplyAsync(proxy, layoutHistoric, null);

            result.Should().BeNull();
            enforceSystemLayoutCalled.Should().BeFalse();
        }

        [Test]
        public async Task KeyboardSelectionChangeRequest_SynchronizeAsync_SetSystemLayout()
        {
            var loggerFactory = new LoggerFactory();
            var enforceSystemLayoutCalled = false;

            var selectedLayoutId = new LayoutId("B90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var layoutId = new LayoutId("B90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var osLayoutId = new OsLayoutId("frenchLayout");

            var frenchLayout = Mock.Of<ILayout>();
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutId)
                .Returns(layoutId);
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutId)
                .Returns(selectedLayoutId);
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(osLayoutId, false, true));

            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            Mock.Get(layoutLibrary)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>() { frenchLayout });

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.EnforceSystemLayoutAsync(It.IsAny<OsLayoutId>()))
                .Returns(Task.Delay(1))
                .Callback(() => enforceSystemLayoutCalled = true);

            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();
            Mock.Get(proxy)
                .Setup(x => x.SelectedLayoutId)
                .Returns(selectedLayoutId);

            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            var changeRequest = new KeyboardSelectionChangeRequest(loggerFactory, system, layoutLibrary, nemeio);

            var result = await changeRequest.ApplyAsync(proxy, layoutHistoric, null);

            result.Should().NotBeNull();
            enforceSystemLayoutCalled.Should().BeTrue();
        }
    }
}
