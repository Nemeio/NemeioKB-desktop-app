using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Watchers;
using NUnit.Framework;

namespace Nemeio.Core.Test.Systems
{
    [TestFixture]
    public class SystemLayoutWatcherShould
    {
        [Test]
        public void SystemLayoutWatcher_Constructor_Ok()
        {
            var loadCalled = false;
            var systemLayouts = new List<OsLayoutId>()
            {
                new OsLayoutId("layoutOne"),
                new OsLayoutId("layoutTwo"),
                new OsLayoutId("layoutThree")
            };

            var loggerFactory = new LoggerFactory();

            var systemLayoutLoaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            Mock.Get(systemLayoutLoaderAdapter)
                .Setup(x => x.Load())
                .Returns(systemLayouts)
                .Callback(() => loadCalled = true);

            Assert.DoesNotThrow(() => new SystemLayoutWatcher(loggerFactory, systemLayoutLoaderAdapter));

            var systemLayoutWatcher = new SystemLayoutWatcher(loggerFactory, systemLayoutLoaderAdapter);

            //  Layouts must be loaded at started
            loadCalled.Should().BeTrue();
        }

        [Test]
        public void SystemLayoutWatcher_Load_Ok()
        {
            var systemLayouts = new List<OsLayoutId>()
            {
                new OsLayoutId("layoutOne"),
                new OsLayoutId("layoutTwo"),
                new OsLayoutId("layoutThree")
            };

            var loggerFactory = new LoggerFactory();

            var systemLayoutLoaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            Mock.Get(systemLayoutLoaderAdapter)
                .Setup(x => x.Load())
                .Returns(systemLayouts); ;

            var systemLayoutWatcher = new SystemLayoutWatcher(loggerFactory, systemLayoutLoaderAdapter);

            var layoutLoaded = systemLayoutWatcher.Load();

            layoutLoaded.Should().NotBeNull();
            layoutLoaded.Count().Should().Be(systemLayouts.Count);
            layoutLoaded.Should().BeEquivalentTo(systemLayouts.Select(x => x.ToString()));
        }

        [Test]
        public async Task SystemLayoutWatcher_Polling_WhenNothingChanged_Ok()
        {
            var layoutListChangedRaised = false;
            var systemLayouts = new List<OsLayoutId>()
            {
                new OsLayoutId("layoutOne"),
                new OsLayoutId("layoutTwo"),
                new OsLayoutId("layoutThree")
            };

            var loggerFactory = new LoggerFactory();

            var systemLayoutLoaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            Mock.Get(systemLayoutLoaderAdapter)
                .Setup(x => x.Load())
                .Returns(systemLayouts); ;

            var systemLayoutWatcher = new SystemLayoutWatcher(loggerFactory, systemLayoutLoaderAdapter);
            systemLayoutWatcher.LayoutChanged += delegate
            {
                layoutListChangedRaised = true;
            };

            var beforeChangesCount = systemLayouts.Count;

            var layoutLoaded = systemLayoutWatcher.Load();
            layoutLoaded.Should().NotBeNull();
            layoutLoaded.Count().Should().Be(beforeChangesCount);

            await Task.Delay(SystemLayoutWatcher.CheckInterval.Add(new System.TimeSpan(0, 0, 1)));

            layoutLoaded = systemLayoutWatcher.Load();
            layoutLoaded.Should().NotBeNull();
            layoutLoaded.Count().Should().Be(beforeChangesCount);
            layoutListChangedRaised.Should().BeFalse();
        }

        [Test]
        public async Task SystemLayoutWatcher_Polling_WhenLayoutAdded_RaiseEvent_Ok()
        {
            var layoutListChangedRaised = false;
            var systemLayouts = new List<OsLayoutId>()
            {
                new OsLayoutId("layoutOne"),
                new OsLayoutId("layoutTwo"),
                new OsLayoutId("layoutThree")
            };

            var loggerFactory = new LoggerFactory();

            var systemLayoutLoaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            Mock.Get(systemLayoutLoaderAdapter)
                .Setup(x => x.Load())
                .Returns(systemLayouts);

            var systemLayoutWatcher = new SystemLayoutWatcher(loggerFactory, systemLayoutLoaderAdapter);
            systemLayoutWatcher.LayoutChanged += delegate
            {
                layoutListChangedRaised = true;
            };

            var beforeChangesCount = systemLayouts.Count;

            var layoutLoaded = systemLayoutWatcher.Load();
            layoutLoaded.Should().NotBeNull();
            layoutLoaded.Count().Should().Be(beforeChangesCount);

            systemLayouts.Add(new OsLayoutId("layoutFour"));

            Mock.Get(systemLayoutLoaderAdapter)
                .Setup(x => x.Load())
                .Returns(systemLayouts);

            await Task.Delay(SystemLayoutWatcher.CheckInterval.Add(new System.TimeSpan(0, 0, 1)));

            var afterChangesCount = systemLayouts.Count;

            layoutLoaded = systemLayoutWatcher.Load();
            layoutLoaded.Should().NotBeNull();
            layoutLoaded.Count().Should().Be(afterChangesCount);
            layoutListChangedRaised.Should().BeTrue();
            afterChangesCount.Should().BeGreaterThan(beforeChangesCount);
        }

        [Test]
        public async Task SystemLayoutWatcher_Polling_WhenLayoutDeleted_RaiseEvent_Ok()
        {
            var layoutListChangedRaised = false;
            var systemLayouts = new List<OsLayoutId>()
            {
                new OsLayoutId("layoutOne"),
                new OsLayoutId("layoutTwo"),
                new OsLayoutId("layoutThree")
            };

            var loggerFactory = new LoggerFactory();

            var systemLayoutLoaderAdapter = Mock.Of<ISystemLayoutLoaderAdapter>();
            Mock.Get(systemLayoutLoaderAdapter)
                .Setup(x => x.Load())
                .Returns(systemLayouts);

            var systemLayoutWatcher = new SystemLayoutWatcher(loggerFactory, systemLayoutLoaderAdapter);
            systemLayoutWatcher.LayoutChanged += delegate
            {
                layoutListChangedRaised = true;
            };

            var beforeChangesCount = systemLayouts.Count;

            var layoutLoaded = systemLayoutWatcher.Load();
            layoutLoaded.Should().NotBeNull();
            layoutLoaded.Count().Should().Be(beforeChangesCount);

            systemLayouts.RemoveAt(0);

            Mock.Get(systemLayoutLoaderAdapter)
                .Setup(x => x.Load())
                .Returns(systemLayouts);

            await Task.Delay(SystemLayoutWatcher.CheckInterval.Add(new System.TimeSpan(0, 0, 1)));

            var afterChangesCount = systemLayouts.Count;

            layoutLoaded = systemLayoutWatcher.Load();
            layoutLoaded.Should().NotBeNull();
            layoutLoaded.Count().Should().Be(afterChangesCount);
            layoutListChangedRaised.Should().BeTrue();
            afterChangesCount.Should().BeLessThan(beforeChangesCount);
        }
    }
}
