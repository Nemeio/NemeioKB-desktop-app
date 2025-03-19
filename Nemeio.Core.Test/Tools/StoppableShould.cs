using FluentAssertions;
using Nemeio.Core.Tools.Stoppable;
using NUnit.Framework;

namespace Nemeio.Core.Test.Tools
{
    [TestFixture]
    public class StoppableShould
    {
        private class TestableStoppable : Stoppable { }

        [Test]
        public void Stoppable_Constructor_Ok()
        {
            var stoppable = new TestableStoppable();

            stoppable.Started.Should().BeTrue();
        }

        [Test]
        public void Stoppable_Stop_Ok()
        {
            var onStopRaisedCalled = false;

            var stoppable = new TestableStoppable();
            stoppable.OnStopRaised += delegate
            {
                onStopRaisedCalled = true;
            };

            stoppable.Started.Should().BeTrue();
            stoppable.Stop();
            stoppable.Started.Should().BeFalse();
            onStopRaisedCalled.Should().BeTrue();
        }
    }
}
