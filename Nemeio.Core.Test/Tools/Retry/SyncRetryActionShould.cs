using System;
using FluentAssertions;
using Nemeio.Core.Tools.Retry;
using NUnit.Framework;

namespace Nemeio.Core.Test.Tools.Retry
{
    public sealed class SyncRetryActionShould
    {
        [Test]
        public void SyncRetryAction_Constructor_Ok()
        {
            Assert.Throws<ArgumentNullException>(() => new SyncRetryAction(null, 0, () => { }));
            Assert.Throws<ArgumentNullException>(() => new SyncRetryAction(string.Empty, 0, null));
            Assert.DoesNotThrow(() => new SyncRetryAction(string.Empty, 0, () => { }));
        }

        [Test]
        public void SyncRetryAction_ExecuteAsync_Ok()
        {
            var count = 0;
            var action = new SyncRetryAction(string.Empty, 0, () =>
            {
                count += 1;
            });

            action.Execute();

            count.Should().Be(1);
        }
    }
}
