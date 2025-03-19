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
    internal class ApplicationShutdownChangeRequestShould
    {
        [Test]
        public async Task ApplicationShutdownChangeRequest_SynchronizeAsync_WhenLastSynchronizedLayoutIsNull_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            var system = Mock.Of<ISystem>();
            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();
            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            var applicationShutdownChangeRequest = new ApplicationShutdownChangeRequest(loggerFactory, system, layoutLibrary, nemeio);

            var result = await applicationShutdownChangeRequest.ApplyAsync(proxy, layoutHistoric, null);

            result.Should().BeNull();
        }

        [Test]
        public async Task ApplicationShutdownChangeRequest_SynchronizeAsync_WhenAssociatedHidIsNull_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var englishLayoutId = new LayoutId("D90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var englishOsLayoutId = new OsLayoutId("englishLayout");

            var customEnglishLayoutHash = new LayoutHash("E90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var customEnglishLayoutId = new LayoutId("E90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var customEnglishLayout = Mock.Of<ILayout>();
            Mock.Get(customEnglishLayout)
                .Setup(x => x.LayoutId)
                .Returns(customEnglishLayoutId);
            Mock.Get(customEnglishLayout)
                .Setup(x => x.Hash)
                .Returns(customEnglishLayoutHash);
            Mock.Get(customEnglishLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(englishOsLayoutId, false, true));
            Mock.Get(customEnglishLayout)
                .Setup(x => x.AssociatedLayoutId)
                .Returns(englishLayoutId);

            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            Mock.Get(layoutLibrary)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>() { customEnglishLayout });

            var system = Mock.Of<ISystem>();
            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();
            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            var applicationShutdownChangeRequest = new ApplicationShutdownChangeRequest(loggerFactory, system, layoutLibrary, nemeio);

            var result = await applicationShutdownChangeRequest.ApplyAsync(proxy, layoutHistoric, customEnglishLayout);

            result.Should().BeNull();
        }

        [Test]
        public async Task ApplicationShutdownChangeRequest_SynchronizeAsync_SetSystemLayout()
        {
            var loggerFactory = new LoggerFactory();
            var enforceSystemLayoutCalled = false;

            var englishLayoutHash = new LayoutHash("D90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var englishLayoutId = new LayoutId("D90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var englishOsLayoutId = new OsLayoutId("englishLayout");
            var englishLayout = Mock.Of<ILayout>();
            Mock.Get(englishLayout)
                .Setup(x => x.LayoutId)
                .Returns(englishLayoutId);
            Mock.Get(englishLayout)
                .Setup(x => x.Hash)
                .Returns(englishLayoutHash);
            Mock.Get(englishLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(englishOsLayoutId, false, true));

            var customEnglishLayoutHash = new LayoutHash("E90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var customEnglishLayoutId = new LayoutId("E90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var customEnglishLayout = Mock.Of<ILayout>();
            Mock.Get(customEnglishLayout)
                .Setup(x => x.LayoutId)
                .Returns(customEnglishLayoutId);
            Mock.Get(customEnglishLayout)
                .Setup(x => x.Hash)
                .Returns(customEnglishLayoutHash);
            Mock.Get(customEnglishLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(englishOsLayoutId, false, true));
            Mock.Get(customEnglishLayout)
                .Setup(x => x.AssociatedLayoutId)
                .Returns(englishLayoutId);

            var frenchLayoutHash = new LayoutHash("B90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var frenchLayoutId = new LayoutId("B90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var frenchOsLayoutId = new OsLayoutId("frenchLayout");
            var frenchLayout = Mock.Of<ILayout>();
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutId)
                .Returns(frenchLayoutId);
            Mock.Get(frenchLayout)
                .Setup(x => x.Hash)
                .Returns(frenchLayoutHash);
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(frenchOsLayoutId, false, true));

            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            Mock.Get(layoutLibrary)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>() { frenchLayout, englishLayout, customEnglishLayout });

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.EnforceSystemLayoutAsync(It.IsAny<OsLayoutId>()))
                .Returns(Task.Delay(1))
                .Callback(() => enforceSystemLayoutCalled = true);

            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            var applicationShutdownChangeRequest = new ApplicationShutdownChangeRequest(loggerFactory, system, layoutLibrary, nemeio);

            var result = await applicationShutdownChangeRequest.ApplyAsync(null, layoutHistoric, customEnglishLayout);

            result.Should().NotBeNull();
            result.Should().Be(englishLayout);
            enforceSystemLayoutCalled.Should().BeTrue();
        }

        [Test]
        public async Task ApplicationShutdownChangeRequest_SynchronizeAsync_SetSystemLayout_AndSetKeyboardLayout()
        {
            var loggerFactory = new LoggerFactory();
            var enforceSystemLayoutCalled = false;
            var applyLayoutOnKeyboardCalled = false;

            var englishLayoutHash = new LayoutHash("D90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var englishLayoutId = new LayoutId("D90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var englishOsLayoutId = new OsLayoutId("englishLayout");
            var englishLayout = Mock.Of<ILayout>();
            Mock.Get(englishLayout)
                .Setup(x => x.LayoutId)
                .Returns(englishLayoutId);
            Mock.Get(englishLayout)
                .Setup(x => x.Hash)
                .Returns(englishLayoutHash);
            Mock.Get(englishLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(englishOsLayoutId, false, true));

            var customEnglishLayoutHash = new LayoutHash("E90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var customEnglishLayoutId = new LayoutId("E90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var customEnglishLayout = Mock.Of<ILayout>();
            Mock.Get(customEnglishLayout)
                .Setup(x => x.LayoutId)
                .Returns(customEnglishLayoutId);
            Mock.Get(customEnglishLayout)
                .Setup(x => x.Hash)
                .Returns(customEnglishLayoutHash);
            Mock.Get(customEnglishLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(englishOsLayoutId, false, true));
            Mock.Get(customEnglishLayout)
                .Setup(x => x.AssociatedLayoutId)
                .Returns(englishLayoutId);

            var frenchLayoutHash = new LayoutHash("B90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var frenchLayoutId = new LayoutId("B90F3E15-1E17-426D-97A9-DE07334B9F1E");
            var frenchOsLayoutId = new OsLayoutId("frenchLayout");
            var frenchLayout = Mock.Of<ILayout>();
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutId)
                .Returns(frenchLayoutId);
            Mock.Get(frenchLayout)
                .Setup(x => x.Hash)
                .Returns(frenchLayoutHash);
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(frenchOsLayoutId, false, true));

            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            Mock.Get(layoutLibrary)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>() { frenchLayout, englishLayout, customEnglishLayout });

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.EnforceSystemLayoutAsync(It.IsAny<OsLayoutId>()))
                .Returns(Task.Delay(1))
                .Callback(() => enforceSystemLayoutCalled = true);

            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();
            Mock.Get(proxy)
                .Setup(x => x.SelectedLayoutId)
                .Returns(frenchLayoutId);
            Mock.Get(proxy)
                .Setup(x => x.ApplyLayoutAsync(It.IsAny<ILayout>()))
                .Returns(Task.Delay(1))
                .Callback(() => applyLayoutOnKeyboardCalled = true);

            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            var applicationShutdownChangeRequest = new ApplicationShutdownChangeRequest(loggerFactory, system, layoutLibrary, nemeio);

            var result = await applicationShutdownChangeRequest.ApplyAsync(proxy, layoutHistoric, customEnglishLayout);

            result.Should().NotBeNull();
            result.Should().Be(englishLayout);
            enforceSystemLayoutCalled.Should().BeTrue();
            applyLayoutOnKeyboardCalled.Should().BeTrue();
        }
    }
}
