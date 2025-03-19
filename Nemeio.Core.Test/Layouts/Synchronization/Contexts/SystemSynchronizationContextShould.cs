using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Synchronization;
using Nemeio.Core.Layouts.Synchronization.Contexts;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using NUnit.Framework;

namespace Nemeio.Core.Test.Layouts.Synchronization.Contexts
{
    [TestFixture]
    public class SystemSynchronizationContextShould
    {
        [Test]
        public async Task SystemSynchronizationContext_SyncAsync_AddLayoutOnSystem_TriggerAddLayoutOnDatabase()
        {
            var createHidsCountCalled = 0;
            var libraryAddLayoutCountCalled = 0;
            var libraryRemoveLayoutCountCalled = 0;

            var loggerFactory = new LoggerFactory();

            var frenchOsLayoutId = new OsLayoutId("frenchLayout");

            var dummyLayout = Mock.Of<ILayout>();
            Mock.Get(dummyLayout)
                .Setup(x => x.Title)
                .Returns(string.Empty);
            Mock.Get(dummyLayout)
                .Setup(x => x.LayoutId)
                .Returns(new LayoutId("3D56824E-19E2-4D64-9919-74F685AEC1DE"));
            Mock.Get(dummyLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(frenchOsLayoutId, false, true));

            var layoutFactory = Mock.Of<ILayoutFactory>();
            var system = Mock.Of<ISystem>();
            Mock.Get(layoutFactory)
                .Setup(x => x.CreateHids(It.IsAny<IEnumerable<OsLayoutId>>(), It.IsAny<IScreen>()))
                .Returns(new List<ILayout>() { dummyLayout })
                .Callback(() => createHidsCountCalled += 1);

            var systemLayoutLoaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            Mock.Get(systemLayoutLoaderAdapter)
                .Setup(x => x.Load())
                .Returns(new List<OsLayoutId>() { frenchOsLayoutId });

            var library = Mock.Of<ILayoutLibrary>();
            Mock.Get(library)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>());
            Mock.Get(library)
                .Setup(x => x.AddLayoutAsync(It.IsAny<ILayout>()))
                .ReturnsAsync(dummyLayout)
                .Callback(() => libraryAddLayoutCountCalled += 1);
            Mock.Get(library)
                .Setup(x => x.RemoveLayoutAsync(It.IsAny<ILayout>()))
                .Returns(Task.Delay(1))
                .Callback(() => libraryRemoveLayoutCountCalled += 1);

            var screen = Mock.Of<IScreen>();
            var context = new SystemSynchronizationContext(loggerFactory, system, layoutFactory, library, systemLayoutLoaderAdapter, screen);

            await context.SyncAsync();

            createHidsCountCalled.Should().Be(1);
            libraryAddLayoutCountCalled.Should().Be(1);
            libraryRemoveLayoutCountCalled.Should().Be(0);
        }

        [Test]
        public async Task SystemSynchronizationContext_SyncAsync_RemoveLayoutOnSystem_TriggerRemoveLayoutOnDatabase()
        {
            var libraryAddLayoutCountCalled = 0;
            var libraryRemoveLayoutCountCalled = 0;

            var loggerFactory = new LoggerFactory();

            var frenchOsLayoutId = new OsLayoutId("frenchLayout");

            var dummyLayout = Mock.Of<ILayout>();
            Mock.Get(dummyLayout)
                .Setup(x => x.Title)
                .Returns(string.Empty);
            Mock.Get(dummyLayout)
                .Setup(x => x.LayoutId)
                .Returns(new LayoutId("3D56824E-19E2-4D64-9919-74F685AEC1DE"));
            Mock.Get(dummyLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(frenchOsLayoutId, false, true));

            var layoutFactory = Mock.Of<ILayoutFactory>();
            var system = Mock.Of<ISystem>();
            Mock.Get(layoutFactory)
                .Setup(x => x.CreateHids(It.IsAny<IEnumerable<OsLayoutId>>(), It.IsAny<IScreen>()))
                .Returns(new List<ILayout>());

            var systemLayoutLoaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            Mock.Get(systemLayoutLoaderAdapter)
                .Setup(x => x.Load())
                .Returns(new List<OsLayoutId>());

            var library = Mock.Of<ILayoutLibrary>();
            Mock.Get(library)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>() { dummyLayout });
            Mock.Get(library)
                .Setup(x => x.AddLayoutAsync(It.IsAny<ILayout>()))
                .ReturnsAsync(dummyLayout)
                .Callback(() => libraryAddLayoutCountCalled += 1);
            Mock.Get(library)
                .Setup(x => x.RemoveLayoutAsync(It.IsAny<LayoutId>()))
                .Returns(Task.Delay(1))
                .Callback(() => libraryRemoveLayoutCountCalled += 1);

            var screen = Mock.Of<IScreen>();
            var context = new SystemSynchronizationContext(loggerFactory, system, layoutFactory, library, systemLayoutLoaderAdapter, screen);

            await context.SyncAsync();

            libraryAddLayoutCountCalled.Should().Be(0);
            libraryRemoveLayoutCountCalled.Should().Be(1);
        }
    }
}
