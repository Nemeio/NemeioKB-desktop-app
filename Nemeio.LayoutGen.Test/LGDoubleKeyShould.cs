using System;
using FluentAssertions;
using Nemeio.LayoutGen.Models;
using Nemeio.LayoutGen.Models.Area;
using NUnit.Framework;

namespace Nemeio.LayoutGen.Test
{
    [TestFixture]
    public class LGDoubleKeyShould : BaseLGKeyShould
    {
        [Test]
        public void LGDoubleKey_01_01_Constructor_WorksOk()
        {
            Assert.DoesNotThrow(() => new LGDoubleKey(_fakeLayout, _fakePosition, _fakeSize, _fakeFont));
            Assert.Throws<ArgumentNullException>(() => new LGDoubleKey(null, _fakePosition, _fakeSize, _fakeFont));
        }

        [Test]
        public void LGDoubleKey_01_02_GetArea_Return_LGDoubleArea()
        {
            var doubleKey = new LGDoubleKey(_fakeLayout, _fakePosition, _fakeSize, _fakeFont);

            doubleKey.GetArea().GetType().Should().Be(typeof(LGDoubleArea));
        }
    }
}
