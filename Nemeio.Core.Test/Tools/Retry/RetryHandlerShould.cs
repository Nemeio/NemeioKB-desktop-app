using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Tools.Retry;
using NUnit.Framework;

namespace Nemeio.Core.Test.Tools.Retry
{
    public sealed class RetryHandlerShould
    {
        [Test]
        public void RetryHandler_Execute_WhenActionIsNull_Throws()
        {
            var loggerFactory = new LoggerFactory();
            var retryHandler = new RetryHandler(loggerFactory);

            Assert.Throws<ArgumentNullException>(() => retryHandler.Execute(null));
        }

        [Test]
        public void RetryHandler_Execute_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var retryHandler = new RetryHandler(loggerFactory);

            const uint WaitedRetryCount = 3;
            uint retryCount = 0;

            var syncAction = new SyncRetryAction("This is an action name", WaitedRetryCount, () =>
            {
                retryCount += 1;
            });

            retryHandler.Execute(syncAction);

            retryCount.Should().Be(1);
        }

        [Test]
        public void RetryHandler_Execute_WhenFailOnce_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var retryHandler = new RetryHandler(loggerFactory);

            const uint WaitedRetryCount = 3;
            uint retryCount = 0;

            var syncAction = new SyncRetryAction("This is an action name", WaitedRetryCount, () =>
            {
                retryCount += 1;

                if (retryCount == 1)
                {
                    throw new InvalidOperationException();
                }
            });

            retryHandler.Execute(syncAction);

            retryCount.Should().Be(2);
        }

        [Test]
        public void RetryHandler_Execute_WhenFailAllRetryCount_Throws()
        {
            var loggerFactory = new LoggerFactory();
            var retryHandler = new RetryHandler(loggerFactory);

            const uint WaitedRetryCount = 3;
            uint retryCount = 0;

            var syncAction = new SyncRetryAction("This is an action name", WaitedRetryCount, () =>
            {
                retryCount += 1;

                throw new InvalidOperationException();
            });

            var exception = Assert.Throws<RetryFailedException>(() => retryHandler.Execute(syncAction));

            retryCount.Should().Be(WaitedRetryCount + 1);
            exception.Should().NotBeNull();
            exception.InnerException.Should().NotBeNull();
            exception.InnerException.GetType().Should().Be(typeof(InvalidOperationException));
        }

        [Test]
        public void RetryHandler_ExecuteAsync_WhenActionIsNull_Throws()
        {
            var loggerFactory = new LoggerFactory();
            var retryHandler = new RetryHandler(loggerFactory);

            Assert.ThrowsAsync<ArgumentNullException>(() => retryHandler.ExecuteAsync(null));
        }

        [Test]
        public async Task RetryHandler_ExecuteAsync_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var retryHandler = new RetryHandler(loggerFactory);

            const uint WaitedRetryCount = 3;
            uint retryCount = 0;

            var asyncAction = new AsyncRetryAction("This is an action name", WaitedRetryCount, async () =>
            {
                retryCount += 1;

                await Task.Delay(100);
            });

            await retryHandler.ExecuteAsync(asyncAction);

            retryCount.Should().Be(1);
        }

        [Test]
        public async Task RetryHandler_ExecuteAsync_WhenFailOnce_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var retryHandler = new RetryHandler(loggerFactory);

            const uint WaitedRetryCount = 3;
            uint retryCount = 0;

            var asyncAction = new AsyncRetryAction("This is an action name", WaitedRetryCount, async () =>
            {
                retryCount += 1;

                if (retryCount == 1)
                {
                    throw new InvalidOperationException();
                }

                await Task.Delay(100);
            });

            await retryHandler.ExecuteAsync(asyncAction);

            retryCount.Should().Be(2);
        }

        [Test]
        public void RetryHandler_ExecuteAsync_WhenFailAllRetryCount_Throws()
        {
            var loggerFactory = new LoggerFactory();
            var retryHandler = new RetryHandler(loggerFactory);

            const uint WaitedRetryCount = 3;
            uint retryCount = 0;

            var asyncAction = new AsyncRetryAction("This is an action name", WaitedRetryCount, async () =>
            {
                retryCount += 1;

                throw new InvalidOperationException();
            });

           var exception = Assert.ThrowsAsync<RetryFailedException>(() => retryHandler.ExecuteAsync(asyncAction));

            retryCount.Should().Be(WaitedRetryCount + 1);
            exception.Should().NotBeNull();
            exception.InnerException.Should().NotBeNull();
            exception.InnerException.GetType().Should().Be(typeof(InvalidOperationException));
        }
    }
}
