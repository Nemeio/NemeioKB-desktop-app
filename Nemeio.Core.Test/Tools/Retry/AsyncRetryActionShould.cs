using System;
using System.Threading.Tasks;
using FluentAssertions;
using Nemeio.Core.Tools.Retry;
using NUnit.Framework;

namespace Nemeio.Core.Test.Tools.Retry
{
    public sealed class AsyncRetryActionShould
    {
        [Test]
        public void AsyncRetryAction_Constructor_Ok()
        {
            Assert.Throws<ArgumentNullException>(() => new AsyncRetryAction(null, 0, async () => await Task.Delay(100)));
            Assert.Throws<ArgumentNullException>(() => new AsyncRetryAction(string.Empty, 0, null));
            Assert.DoesNotThrow(() => new AsyncRetryAction(string.Empty, 0, async () => await Task.Delay(100)));
        }

        [Test]
        public async Task AsyncRetryAction_ExecuteAsync_Ok()
        {
            var count = 0;
            var action = new AsyncRetryAction(string.Empty, 0, async () => 
            {
                await Task.Delay(100);

                count += 1;
            });

            await action.ExecuteAsync();

            count.Should().Be(1);
        }
    }
}
