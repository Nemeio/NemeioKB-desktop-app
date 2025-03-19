using System;
using FluentAssertions;
using Nemeio.LayoutGen.Models;
using Nemeio.LayoutGen.Models.Area;
using NUnit.Framework;

namespace Nemeio.LayoutGen.Test
{
    [TestFixture]
    public class LGSingleKeyShould : BaseLGKeyShould
    {
        [Test]
        public void LGSingleKey_01_01_Constructor_WorksOk()
        {
            Assert.DoesNotThrow(() => new LGSingleKey(_fakeLayout, _fakePosition, _fakeSize, _fakeFont));
            Assert.Throws<ArgumentNullException>(() => new LGSingleKey(null, _fakePosition, _fakeSize, _fakeFont));
        }

        [Test]
        public void LGSingleKey_01_02_GetArea_Return_LGSingleArea()
        {
            var doubleKey = new LGSingleKey(_fakeLayout, _fakePosition, _fakeSize, _fakeFont);

            doubleKey.GetArea().GetType().Should().Be(typeof(LGSingleArea));
        }
    }
}
