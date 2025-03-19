using System;
using FluentAssertions;
using Nemeio.LayoutGen.Models;
using NUnit.Framework;

namespace Nemeio.LayoutGen.Test
{
    [TestFixture]
    public class LGSizeShould
    {
        [Test]
        public void LGSize_01_01_Constructor_WorksOk()
        {
            const int width = 100;
            const int height = 25;

            var square = new LGSize(width);
            square.Width.Should().Be(width);
            square.Height.Should().Be(width);

            var newLGSize = new LGSize(width, height);
            newLGSize.Width.Should().Be(width);
            newLGSize.Height.Should().Be(height);
        }

        [Test]
        public void LGSize_01_02_Substract_WorksOk()
        {
            var random = new Random();
            var defaultLGSize = random.Next(100);
            float substract = random.Next(defaultLGSize);

            var testLGSize = new LGSize(defaultLGSize);
            var resultLGSize = testLGSize - substract;

            resultLGSize.Width.Should().Be(defaultLGSize - substract);
            resultLGSize.Height.Should().Be(defaultLGSize - substract);
        }

        [Test]
        public void LGSize_01_03_Divide_WorksOk()
        {
            var random = new Random();
            var defaultLGSize = random.Next(100);
            float divider = random.Next(defaultLGSize);

            var testLGSize = new LGSize(defaultLGSize);
            var resultLGSize = testLGSize / divider;

            float expectedLGSize = defaultLGSize / divider;

            resultLGSize.Width.Should().Be(expectedLGSize);
            resultLGSize.Height.Should().Be(expectedLGSize);
        }
    }
}
