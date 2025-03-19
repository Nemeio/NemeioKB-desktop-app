
using System;
using Nemeio.Core.Keyboard.Communication.Utils;

namespace Nemeio.Keyboard.Communication.Tools.Utils
{
    public sealed class KeyboardVersionParser : IKeyboardVersionParser
    {
        public Version Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Length != 3)
            {
                throw new ArgumentOutOfRangeException($"Value must be 3 characters (e.g. '3.2'). Current value : <{value}>");
            }

            var version = new Version(value);

            return version;
        }
    }
}
