using System;
using FluentAssertions;
using Nemeio.Core.DataModels;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    [TestFixture]
    public class VersionShould
    {
        [Test]
        [TestCase("1.A.3")]
        [TestCase("B.33.3")]
        [TestCase("#.1.3")]
        public void Constructor_ThrowError_WhenBadParameters(string versionValue)
        {
            Assert.Throws<FormatException>(() => new VersionProxy(versionValue));
        }

        [Test]
        public void IsHigherThan_WithHigherVersion_ReturnTrue()
        {
            var lowerVersion = new VersionProxy("1.0.0");
            var higherVersion = new VersionProxy("2.0.0");

            higherVersion.IsHigherThan(lowerVersion).Should().BeTrue();
        }

        [Test]
        public void IsHigherThan_WithLowerVersion_ReturnFalse()
        {
            var lowerVersion = new VersionProxy("1.0.0");
            var higherVersion = new VersionProxy("2.0.0");

            lowerVersion.IsHigherThan(higherVersion).Should().BeFalse();
        }
    }
}
