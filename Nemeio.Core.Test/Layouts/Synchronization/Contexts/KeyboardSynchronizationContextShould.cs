using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Synchronization;
using Nemeio.Core.Layouts.Synchronization.Contexts;
using Nemeio.Core.Layouts.Synchronization.Contexts.State;
using Nemeio.Core.Services.Layouts;
using NUnit.Framework;

namespace Nemeio.Core.Test.Layouts.Synchronization.Contexts
{
    [TestFixture]
    public class KeyboardSynchronizationContextShould
    {
        [Test]
        public async Task KeyboardSynchronizationContext_SyncAsync_AddOnDatabase_TriggerAddOnKeyboard()
        {
            var loggerFactory = new LoggerFactory();

            var frenchLayout = Mock.Of<ILayout>();
            Mock.Get(frenchLayout)
                .Setup(x => x.Hash)
                .Returns(new LayoutHash("0403CF64-9BF8-49C5-94A9-173B7A584AC3"));
            Mock.Get(frenchLayout)
                .Setup(x => x.Enable)
                .Returns(true);

            var library = Mock.Of<ILayoutLibrary>();
            Mock.Get(library)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>()
                {
                    frenchLayout
                });

            var startSyncCalled = false;
            var endSyncCalled = false;
            var addLayoutCountCalled = 0;
            var deleteLayoutCoundCalled = 0;

            var syncNemeioProxy = Mock.Of<ISynchronizableNemeioProxy>();
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.LayoutIdWithHashs)
                .Returns(new List<LayoutIdWithHash>());
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.StartSynchronizationAsync())
                .Returns(Task.Delay(1))
                .Callback(() => startSyncCalled = true);
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.EndSynchronizationAsync())
                .Returns(Task.Delay(1))
                .Callback(() => endSyncCalled = true);
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.AddLayoutAsync(It.IsAny<ILayout>()))
                .Returns(Task.Delay(1))
                .Callback(() => addLayoutCountCalled += 1);
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.DeleteLayoutAsync(It.IsAny<LayoutId>()))
                .Returns(Task.Delay(1))
                .Callback(() => deleteLayoutCoundCalled += 1);

            var context = new KeyboardSynchronizationContext(loggerFactory, library, syncNemeioProxy);

            await context.SyncAsync();

            startSyncCalled.Should().BeTrue();
            endSyncCalled.Should().BeTrue();
            addLayoutCountCalled.Should().Be(1);
            deleteLayoutCoundCalled.Should().Be(0);
        }

        [Test]
        public async Task KeyboardSynchronizationContext_SyncAsync_AddOnDatabase_TriggerAddOnKeyboard_OnlyForEnableLayouts()
        {
            var loggerFactory = new LoggerFactory();

            //  Enabled layout
            var frenchLayout = Mock.Of<ILayout>();
            Mock.Get(frenchLayout)
                .Setup(x => x.Hash)
                .Returns(new LayoutHash("0403CF64-9BF8-49C5-94A9-173B7A584AC3"));
            Mock.Get(frenchLayout)
                .Setup(x => x.Enable)
                .Returns(true);

            //  Disabled layout
            var englishLayout = Mock.Of<ILayout>();
            Mock.Get(englishLayout)
                .Setup(x => x.Hash)
                .Returns(new LayoutHash("0503CF64-9BF8-49C5-94A9-173B7A584AC3"));
            Mock.Get(englishLayout)
                .Setup(x => x.Enable)
                .Returns(false);

            var library = Mock.Of<ILayoutLibrary>();
            Mock.Get(library)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>() { frenchLayout, englishLayout });

            var startSyncCalled = false;
            var endSyncCalled = false;
            var addLayoutCountCalled = 0;
            var deleteLayoutCoundCalled = 0;

            var syncNemeioProxy = Mock.Of<ISynchronizableNemeioProxy>();
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.LayoutIdWithHashs)
                .Returns(new List<LayoutIdWithHash>());
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.StartSynchronizationAsync())
                .Returns(Task.Delay(1))
                .Callback(() => startSyncCalled = true);
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.EndSynchronizationAsync())
                .Returns(Task.Delay(1))
                .Callback(() => endSyncCalled = true);
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.AddLayoutAsync(It.IsAny<ILayout>()))
                .Returns(Task.Delay(1))
                .Callback(() => addLayoutCountCalled += 1);
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.DeleteLayoutAsync(It.IsAny<LayoutId>()))
                .Returns(Task.Delay(1))
                .Callback(() => deleteLayoutCoundCalled += 1);

            var context = new KeyboardSynchronizationContext(loggerFactory, library, syncNemeioProxy);

            await context.SyncAsync();

            startSyncCalled.Should().BeTrue();
            endSyncCalled.Should().BeTrue();
            addLayoutCountCalled.Should().Be(1);
            deleteLayoutCoundCalled.Should().Be(0);
        }

        [Test]
        public async Task KeyboardSynchronizationContext_SyncAsync_DeleteOnDatabase_TriggerDeleteOnKeyboard()
        {
            var loggerFactory = new LoggerFactory();

            var frenchLayout = new LayoutIdWithHash(new LayoutId("0403CF64-9BF8-49C5-94A9-173B7A584AC3"), new LayoutHash("0403CF64-9BF8-49C5-94A9-173B7A584AC3"));

            var library = Mock.Of<ILayoutLibrary>();
            Mock.Get(library)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>());

            var startSyncCalled = false;
            var endSyncCalled = false;
            var addLayoutCountCalled = 0;
            var deleteLayoutCoundCalled = 0;

            var syncNemeioProxy = Mock.Of<ISynchronizableNemeioProxy>();
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.LayoutIdWithHashs)
                .Returns(new List<LayoutIdWithHash>() { frenchLayout });
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.StartSynchronizationAsync())
                .Returns(Task.Delay(1))
                .Callback(() => startSyncCalled = true);
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.EndSynchronizationAsync())
                .Returns(Task.Delay(1))
                .Callback(() => endSyncCalled = true);
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.AddLayoutAsync(It.IsAny<ILayout>()))
                .Returns(Task.Delay(1))
                .Callback(() => addLayoutCountCalled += 1);
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.DeleteLayoutAsync(It.IsAny<LayoutId>()))
                .Returns(Task.Delay(1))
                .Callback(() => deleteLayoutCoundCalled += 1);

            var context = new KeyboardSynchronizationContext(loggerFactory, library, syncNemeioProxy);

            await context.SyncAsync();

            startSyncCalled.Should().BeTrue();
            endSyncCalled.Should().BeTrue();
            addLayoutCountCalled.Should().Be(0);
            deleteLayoutCoundCalled.Should().Be(1);
        }

        [Test]
        public async Task KeyboardSynchronizationContext_SyncAsync_NotifyProgress()
        {
            var loggerFactory = new LoggerFactory();

            //  Enabled layout
            var englishLayout = new LayoutIdWithHash(new LayoutId("0503CF64-9BF8-49C5-94A9-173B7A584AC3"), new LayoutHash("0503CF64-9BF8-49C5-94A9-173B7A584AC3"));
            var frenchLayout = Mock.Of<ILayout>();
            Mock.Get(frenchLayout)
                .Setup(x => x.Hash)
                .Returns(new LayoutHash("0403CF64-9BF8-49C5-94A9-173B7A584AC3"));
            Mock.Get(frenchLayout)
                .Setup(x => x.Enable)
                .Returns(true);

            var library = Mock.Of<ILayoutLibrary>();
            Mock.Get(library)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>() { frenchLayout });

            var startSyncCalled = false;
            var endSyncCalled = false;
            var addLayoutCountCalled = 0;
            var deleteLayoutCoundCalled = 0;

            var syncNemeioProxy = Mock.Of<ISynchronizableNemeioProxy>();
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.LayoutIdWithHashs)
                .Returns(new List<LayoutIdWithHash>() { englishLayout });
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.StartSynchronizationAsync())
                .Returns(Task.Delay(1))
                .Callback(() => startSyncCalled = true);
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.EndSynchronizationAsync())
                .Returns(Task.Delay(1))
                .Callback(() => endSyncCalled = true);
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.AddLayoutAsync(It.IsAny<ILayout>()))
                .Returns(Task.Delay(1))
                .Callback(() => addLayoutCountCalled += 1);
            Mock.Get(syncNemeioProxy)
                .Setup(x => x.DeleteLayoutAsync(It.IsAny<LayoutId>()))
                .Returns(Task.Delay(1))
                .Callback(() => deleteLayoutCoundCalled += 1);

            var stateChangeToStart = false;
            var stateChangeToProgress = false;
            var stateChangeToEnd = false;

            uint modificationCount = 0;
            var nbProgress = 0;

            var context = new KeyboardSynchronizationContext(loggerFactory, library, syncNemeioProxy);
            context.State.StateChanged += delegate
            {
                if (context.State.State == SynchronizationState.Start)
                {
                    stateChangeToStart = true;
                    modificationCount = context.State.ModificationCount;
                }

                if (context.State.State == SynchronizationState.InProgress)
                {
                    stateChangeToProgress = true;
                }

                if (context.State.State == SynchronizationState.End)
                {
                    stateChangeToEnd = true;
                }
            };
            context.State.ProgressionChanged += delegate
            {
                nbProgress += 1;
            };

            await context.SyncAsync();

            startSyncCalled.Should().BeTrue();
            endSyncCalled.Should().BeTrue();
            addLayoutCountCalled.Should().Be(1);
            deleteLayoutCoundCalled.Should().Be(1);

            stateChangeToStart.Should().BeTrue();
            stateChangeToProgress.Should().BeTrue();
            stateChangeToEnd.Should().BeTrue();

            modificationCount.Should().Be(2);
            nbProgress.Should().Be(2);
        }
    }
}
