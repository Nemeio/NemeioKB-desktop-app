using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Errors;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Watchers;
using NUnit.Framework;

namespace Nemeio.Core.Test.Systems
{
    [TestFixture]
    public class SystemActiveLayoutWatcherShould
    {
        [Test]
        public void SystemActiveLayoutWatcher_Constructor_Ok()
        {
            var dummyLayoutId = new OsLayoutId("dummyOne");

            var loggerFactory = new LoggerFactory();
            var activeLayoutAdapter = Mock.Of<ISystemActiveLayoutAdapter>();
            var errorManager = Mock.Of<IErrorManager>();

            Assert.DoesNotThrow(() => new SystemActiveLayoutWatcher(loggerFactory, activeLayoutAdapter, errorManager));

            Mock.Get(activeLayoutAdapter)
                .Setup(x => x.GetCurrentSystemLayoutId())
                .Returns(dummyLayoutId);

            var activeLayoutWatcher = new SystemActiveLayoutWatcher(loggerFactory, activeLayoutAdapter, errorManager);

            //  Current active layout must be accessible
            activeLayoutWatcher.CurrentSystemLayoutId.Should().NotBeNull();
        }

        [Test]
        public async Task SystemActiveLayoutWatcher_Polling_WhenNothingChanged_Ok()
        {
            var dummyOneLayoutId = new OsLayoutId("dummyOne");

            var loggerFactory = new LoggerFactory();

            var activeLayoutAdapter = Mock.Of<ISystemActiveLayoutAdapter>();
            Mock.Get(activeLayoutAdapter)
                .Setup(x => x.GetCurrentSystemLayoutId())
                .Returns(dummyOneLayoutId);

            var errorManager = Mock.Of<IErrorManager>();

            var activeLayoutWatcher = new SystemActiveLayoutWatcher(loggerFactory, activeLayoutAdapter, errorManager);

            activeLayoutWatcher.CurrentSystemLayoutId.Should().NotBeNull();
            activeLayoutWatcher.CurrentSystemLayoutId.Should().Be(dummyOneLayoutId);

            await Task.Delay(SystemActiveLayoutWatcher.ThreadPollingTimeout + 1);

            activeLayoutWatcher.CurrentSystemLayoutId.Should().NotBeNull();
            activeLayoutWatcher.CurrentSystemLayoutId.Should().Be(dummyOneLayoutId);
        }

        [Test]
        public async Task SystemActiveLayoutWatcher_Polling_WhenActiveLayoutChanged_Ok()
        {
            var dummyOneLayoutId = new OsLayoutId("dummyOne");
            var dummyTwoLayoutId = new OsLayoutId("dummyTwo");

            var loggerFactory = new LoggerFactory();

            var activeLayoutAdapter = Mock.Of<ISystemActiveLayoutAdapter>();
            Mock.Get(activeLayoutAdapter)
                .Setup(x => x.GetCurrentSystemLayoutId())
                .Returns(dummyOneLayoutId);

            var errorManager = Mock.Of<IErrorManager>();

            var activeLayoutWatcher = new SystemActiveLayoutWatcher(loggerFactory, activeLayoutAdapter, errorManager);

            activeLayoutWatcher.CurrentSystemLayoutId.Should().NotBeNull();
            activeLayoutWatcher.CurrentSystemLayoutId.Should().Be(dummyOneLayoutId);

            var eventSemaphore = new SemaphoreSlim(0, 1);

            activeLayoutWatcher.OnSystemLayoutChanged += delegate
            {
                if (activeLayoutWatcher.CurrentSystemLayoutId.Equals(dummyTwoLayoutId))
                {
                    eventSemaphore.Release();
                }
            };

            Mock.Get(activeLayoutAdapter)
               .Setup(x => x.GetCurrentSystemLayoutId())
               .Returns(dummyTwoLayoutId);

            var waitingTime = TimeSpan.FromSeconds(2);

            var succeed = await eventSemaphore.WaitAsync(waitingTime);
            if (!succeed)
            {
                throw new InvalidOperationException($"Semaphore never release by event.");
            }

            activeLayoutWatcher.CurrentSystemLayoutId.Should().NotBeNull();
            activeLayoutWatcher.CurrentSystemLayoutId.Should().Be(dummyTwoLayoutId);
        }
    }
}
