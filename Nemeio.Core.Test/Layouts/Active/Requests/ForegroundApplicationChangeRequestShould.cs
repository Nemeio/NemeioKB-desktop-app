using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Layouts.Active.Requests;
using Nemeio.Core.Layouts.LinkedApplications;
using Nemeio.Core.Managers;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Applications;
using NUnit.Framework;

namespace Nemeio.Core.Test.Layouts.Active.Requests
{
    [TestFixture]
    public class ForegroundApplicationChangeRequestShould
    {
        [Test]
        public async Task ForegroundApplicationChangeRequest_SynchronizeAsync_WhenEventIsNull_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            var system = Mock.Of<ISystem>();
            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();
            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            var languageManager = Mock.Of<ILanguageManager>();
            var applicationLayoutManager = Mock.Of<IApplicationLayoutManager>();

            var application = new Application();
            var changeRequest = new ForegroundApplicationChangeRequest(application, loggerFactory, system, layoutLibrary, nemeio, languageManager, applicationLayoutManager);

            var result = await changeRequest.ApplyAsync(proxy, layoutHistoric, null);

            result.Should().BeNull();
        }

        [Test]
        public async Task ForegroundApplicationChangeRequest_SynchronizeAsync_WhenProxyIsNull_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            var system = Mock.Of<ISystem>();
            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            Mock.Get(layoutHistoric)
                .Setup(x => x.Historic)
                .Returns(new List<IHistoricLog>());
            var languageManager = Mock.Of<ILanguageManager>();
            var applicationLayoutManager = Mock.Of<IApplicationLayoutManager>();

            var application = new Application()
            {
                ApplicationPath = "this/is/a/fake/path"
            };
            var changeRequest = new ForegroundApplicationChangeRequest(application, loggerFactory, system, layoutLibrary, nemeio, languageManager, applicationLayoutManager);

            var result = await changeRequest.ApplyAsync(null, layoutHistoric, null);

            result.Should().BeNull();
        }

        [Test]
        public async Task ForegroundApplicationChangeRequest_SynchronizeAsync_WhenFindExceptionForFocusApplication_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var exceptionApplicationPath = @"C:\Fake\Path\Application.exe";

            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            var system = Mock.Of<ISystem>();
            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            var languageManager = Mock.Of<ILanguageManager>();
            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();

            var focusApplication = new Application()
            {
                ApplicationPath = exceptionApplicationPath
            };

            var applicationLayoutManager = Mock.Of<IApplicationLayoutManager>();
            Mock.Get(applicationLayoutManager)
                .Setup(x => x.FindException(focusApplication))
                .Returns(string.Empty);

            var application = new Application();
            var changeRequest = new ForegroundApplicationChangeRequest(application, loggerFactory, system, layoutLibrary, nemeio, languageManager, applicationLayoutManager);

            var result = await changeRequest.ApplyAsync(proxy, layoutHistoric, null);

            result.Should().BeNull();
        }

        [Test]
        public async Task ForegroundApplicationChangeRequest_SynchronizeAsync_WhenIsConfigurator_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var configuratorApplicationName = "MyConfigurator";
            var configuratorApplicationPath = @"C:\Users\nemeio\configurator.exe";

            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            var system = Mock.Of<ISystem>();
            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            Mock.Get(layoutHistoric)
                .Setup(x => x.Historic)
                .Returns(new List<IHistoricLog>());
            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();
            var applicationLayoutManager = Mock.Of<IApplicationLayoutManager>();

            var focusApplication = new Application()
            {
                ApplicationPath = configuratorApplicationPath,
                WindowTitle = configuratorApplicationName,
                Name = NemeioConstants.AppName,
                ProcessName = configuratorApplicationName
            };

            var languageManager = Mock.Of<ILanguageManager>();
            Mock.Get(languageManager)
                .Setup(x => x.GetLocalizedValue(StringId.ConfiguratorTitle))
                .Returns(configuratorApplicationName);

            var changeRequest = new ForegroundApplicationChangeRequest(focusApplication, loggerFactory, system, layoutLibrary, nemeio, languageManager, applicationLayoutManager);

            var result = await changeRequest.ApplyAsync(proxy, layoutHistoric, null);

            result.Should().BeNull();
        }

        [Test]
        public async Task ForegroundApplicationChangeRequest_SynchronizeAsync_WhenNotFindLatestAssociatedLayout_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            var system = Mock.Of<ISystem>();
            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();
            Mock.Get(layoutHistoric)
                .Setup(x => x.Historic)
                .Returns(new List<IHistoricLog>());
            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();
            var languageManager = Mock.Of<ILanguageManager>();

            var focusApplication = new Application()
            {
                ApplicationPath = @"C:\Users\myUser\myApps\myApp.exe"
            };

            var applicationLayoutManager = Mock.Of<IApplicationLayoutManager>();
            Mock.Get(applicationLayoutManager)
                .Setup(x => x.FindException(focusApplication))
                .Returns(() => null);

            var applicationFirstStrategy = new ForegroundApplicationChangeRequest(focusApplication, loggerFactory, system, layoutLibrary, nemeio, languageManager, applicationLayoutManager);

            var result = await applicationFirstStrategy.ApplyAsync(proxy, layoutHistoric, null);

            result.Should().BeNull();
        }

        [Test]
        public async Task ForegroundApplicationChangeRequest_SynchronizeAsync_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var enforceSystemLayoutCalled = false;
            var applyLayoutOnKeyboardCalled = false;

            var nemeio = Mock.Of<INemeio>();
            var layoutHistoric = new ActiveLayoutHistoric();
            var languageManager = Mock.Of<ILanguageManager>();

            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.EnforceSystemLayoutAsync(It.IsAny<OsLayoutId>()))
                .Returns(Task.Delay(1))
                .Callback(() => enforceSystemLayoutCalled = true);

            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();
            Mock.Get(proxy)
                .Setup(x => x.ApplyLayoutAsync(It.IsAny<ILayout>()))
                .Returns(Task.Delay(1))
                .Callback(() => applyLayoutOnKeyboardCalled = true);

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
            Mock.Get(englishLayout)
                .Setup(x => x.Enable)
                .Returns(true);

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
            Mock.Get(frenchLayout)
                .Setup(x => x.Enable)
                .Returns(true);
            var focusApplication = new Application()
            {
                ApplicationPath = @"C:\Users\myUser\myApps\myApp.exe"
            };

            var applicationLayoutManager = Mock.Of<IApplicationLayoutManager>();
            Mock.Get(applicationLayoutManager)
                .Setup(x => x.FindException(focusApplication))
                .Returns(() => null);
            Mock.Get(applicationLayoutManager)
                .Setup(x => x.FindLatestAssociatedLayoutId(focusApplication))
                .Returns(() => frenchLayoutId);

            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            Mock.Get(layoutLibrary)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>() { englishLayout, frenchLayout });

            layoutHistoric.Insert(new HistoricLog(englishLayout, HistoricActor.User));

            var changeRequest = new ForegroundApplicationChangeRequest(focusApplication, loggerFactory, system, layoutLibrary, nemeio, languageManager, applicationLayoutManager);

            var result = await changeRequest.ApplyAsync(proxy, layoutHistoric, englishLayout);

            result.Should().NotBeNull();
            result.Should().Be(frenchLayout);
            enforceSystemLayoutCalled.Should().BeTrue();
            applyLayoutOnKeyboardCalled.Should().BeTrue();
        }
    }
}
