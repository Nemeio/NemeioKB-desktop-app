using System;
using Nemeio.Core.DataModels;
using NUnit.Framework;

namespace Nemeio.Core.Test.DataModels
{
    [TestFixture]
    class KeyboardFailureShould
    {
        [Test]
        public void InvalidRegistryNumberThrowsArgumentException()
        {
            UInt32[] invalidRegitrySize = new UInt32[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12  };
            Assert.Throws<ArgumentException>(() => new KeyboardFailure(0, invalidRegitrySize, 0, 0, 0, 0));
            Assert.Throws<ArgumentException>(() => new KeyboardFailure(0, invalidRegitrySize, 0, 0, 0, 0, 0));
        }
    }
}
