using System;
using Nemeio.Keyboard.Communication.Tools.Utils;
using NUnit.Framework;

namespace Nemeio.Keyboard.Communication.Tools.Tests
{
    [TestFixture]
    internal class KeyboardVersionParserShould
    {
        [TestCase("")]
        [TestCase(null)]
        public void KeyboardVersionParser_Parse_WhenValueIsNullOrEmpty_Throws(string value)
        {
            var keyboardVersionParser = new KeyboardVersionParser();

            Assert.Throws<ArgumentNullException>(() => keyboardVersionParser.Parse(value));
        }

        [TestCase("02.03")]
        [TestCase("2")]
        public void KeyboardVersionParser_Parse_WhenValueLengthIsInvalid_Throws(string value)
        {
            var keyboardVersionParser = new KeyboardVersionParser();

            Assert.Throws<ArgumentOutOfRangeException>(() => keyboardVersionParser.Parse(value));
        }

        [Test]
        public void KeyboardVersionParser_Parse_Ok()
        {
            var validValue = "3.2";
            var keyboardVersionParser = new KeyboardVersionParser();

            Assert.DoesNotThrow(() => keyboardVersionParser.Parse(validValue));
        }
    }
}
