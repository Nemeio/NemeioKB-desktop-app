using FluentAssertions;
using Nemeio.Core.Extensions;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    [TestFixture]
    public class StringExtensionsShould
    {
        [TestCase("thisIsNotAValueWithPrefixValue")]
        [TestCase("file")]
        [TestCase("://")]
        [TestCase("file://thisIsNotAFileExtension")]
        [TestCase("#file:///thisIsNotAFileExtension")]
        [TestCase("ldlc.png")]
        [TestCase("../ldlc.png")]
        public void StringExtension_RemoveFilePrefix_WithoutPrefix_NotChangeValue(string value)
        {
            var result = value.RemoveFilePrefix();

            result.Should().Be(value);
        }

        [TestCase("file:///Users/username/Library/Application Support/ApplicationName")]
        [TestCase("FILE:///Users/username/Library/Application Support/ApplicationName")]
        [TestCase("file:///C:/Users/username/AppData/Roaming/ApplicationName")]
        [TestCase("FiLe:///C:/Users/username/AppData/Roaming/ApplicationName")]
        public void StringExtension_RemoveFilePrefix_WithPrefix_ChangeValue(string value)
        {
            var result = value.RemoveFilePrefix();

            result.Should().NotBeNullOrWhiteSpace();
            result.Length.Should().Be(value.Length - StringExtensions.Prefix.Length);
        }
    }
}
