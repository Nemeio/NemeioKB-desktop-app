using System;
using FluentAssertions;
using Nemeio.LayoutGen.Models;
using Nemeio.LayoutGen.Models.Area;
using NUnit.Framework;

namespace Nemeio.LayoutGen.Test
{
    [TestFixture]
    public class LGFourfoldKeyShould : BaseLGKeyShould
    {
        [Test]
        public void LGFourfoldKey_01_01_Constructor_WorksOk()
        {
            Assert.DoesNotThrow(() => new LGFourfoldKey(_fakeLayout, _fakePosition, _fakeSize, _fakeFont));
            Assert.Throws<ArgumentNullException>(() => new LGFourfoldKey(null, _fakePosition, _fakeSize, _fakeFont));
        }

        [Test]
        public void LGFourfoldKey_01_02_GetArea_Return_LGFourfoldArea()
        {
            var doubleKey = new LGFourfoldKey(_fakeLayout, _fakePosition, _fakeSize, _fakeFont);

            doubleKey.GetArea().GetType().Should().Be(typeof(LGFourfoldArea));
        }
    }
}
