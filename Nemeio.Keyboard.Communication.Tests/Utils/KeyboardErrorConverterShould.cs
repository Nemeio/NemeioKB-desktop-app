using System;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Keyboard.Tools.Utils;
using NUnit.Framework;

namespace Nemeio.Keyboard.Communication.Tests.Utils
{
    [TestFixture]
    public class KeyboardErrorConverterShould
    {
        public static Array KeyboardErrorCodes() => Enum.GetValues(typeof(KeyboardErrorCode));

        [TestCaseSource("KeyboardErrorCodes")]
        public void KeyboardErrorConverter_Convert_Ok(KeyboardErrorCode errorCode)
        {
            var errorConverter = new KeyboardErrorConverter();

            Assert.DoesNotThrow(() => errorConverter.Convert(errorCode));
        }
    }
}
