using System;
using FluentAssertions;
using Nemeio.LayoutGen.Models;
using NUnit.Framework;

namespace Nemeio.LayoutGen.Test
{
    [TestFixture]
    public class LGPositionShould
    {
        [Test]
        public void LGPosition_01_01_Equal_WorksOk()
        {
            var testPoint = new LGPosition(15, 15);
            var testedPoint = new LGPosition(15, 15);

            testedPoint.Equals(testPoint).Should().BeTrue();
        }

        [Test]
        public void LGPosition_01_02_NotEqual_WorksOk()
        {
            var testPoint = new LGPosition(17, 17);
            var testedPoint = new LGPosition(15, 15);

            testedPoint.Equals(testPoint).Should().BeFalse();
        }

        [Test]
        public void LGPosition_01_03_AddUp_WorksOk()
        {
            var random = new Random();
            var addPoint = new LGPosition(random.Next(), random.Next());
            var testPoint = new LGPosition(45, 32);

            var resultPoint = testPoint + addPoint;

            resultPoint.X.Should().Be(testPoint.X + addPoint.X);
            resultPoint.Y.Should().Be(testPoint.Y + addPoint.Y);
        }
    }
}
