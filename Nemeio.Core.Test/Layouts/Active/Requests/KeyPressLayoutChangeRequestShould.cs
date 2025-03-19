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
    public class KeyPressLayoutChangeRequestShould
    {
        [Test]
        public async Task KeyPressLayoutChangeRequest_SynchronizeAsync_SetSystemLayout_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var enforceSystemLayoutCalled = false;
            var layoutId = new LayoutId("B90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var osLayoutId = new OsLayoutId("frenchLayout");
            var layout = Mock.Of<ILayout>();
            Mock.Get(layout)
                .Setup(x => x.LayoutId)
                .Returns(layoutId);
            Mock.Get(layout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(osLayoutId, false, true));

            var frenchLayout = Mock.Of<ILayout>();
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutId)
                .Returns(layoutId);
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

            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            var changeRequest = new KeyPressLayoutChangeRequest(layout,loggerFactory, system, layoutLibrary, nemeio);

            var result = await changeRequest.ApplyAsync(null, layoutHistoric, null);

            result.Should().NotBeNull();
            enforceSystemLayoutCalled.Should().BeTrue();
        }

        [Test]
        public async Task KeyPressLayoutChangeRequest_SynchronizeAsync_SetSystemLayout_AndKeyboardWhenPlugged_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var keyboardApplyCalled = false;
            var enforceSystemLayoutCalled = false;
            var layoutId = new LayoutId("B90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var osLayoutId = new OsLayoutId("frenchLayout");
            var layout = Mock.Of<ILayout>();
            Mock.Get(layout)
                .Setup(x => x.LayoutId)
                .Returns(layoutId);
            Mock.Get(layout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(osLayoutId, false, true));

            var frenchLayout = Mock.Of<ILayout>();
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutId)
                .Returns(layoutId);
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
                .Setup(x => x.ApplyLayoutAsync(It.IsAny<ILayout>()))
                .Returns(Task.Delay(1))
                .Callback(() => keyboardApplyCalled = true);

            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            var changeRequest = new KeyPressLayoutChangeRequest(layout,loggerFactory, system, layoutLibrary, nemeio);

            var result = await changeRequest.ApplyAsync(proxy, layoutHistoric, null);

            result.Should().NotBeNull();
            enforceSystemLayoutCalled.Should().BeTrue();
            keyboardApplyCalled.Should().BeTrue();
        }
    }
}
