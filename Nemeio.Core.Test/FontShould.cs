using System;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Models.Fonts;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    [TestFixture]
    public class FontShould
    {
        [Test]
        public void FontShould_01_01_Constructor_WorksOk()
        {
            const string validFontName = "Arial";

            Assert.DoesNotThrow(() => new Font(validFontName, FontSize.Medium, true, false));

            Assert.Throws<InvalidOperationException>(() => new Font("", FontSize.Medium, true, false));
            Assert.Throws<InvalidOperationException>(() => new Font(" ", FontSize.Medium, true, false));
            Assert.Throws<InvalidOperationException>(() => new Font(null, FontSize.Medium, true, false));
        }
    }
}
