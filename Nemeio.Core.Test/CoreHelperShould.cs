using System;
using FluentAssertions;
using Nemeio.Core.DataModels.Locale;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    public class CoreHelperShould
    {
        [Test]
        public void CoreHelper_Extension_Trace_WithFilledArray_Works()
        {
            var filledArray = new byte[3] { 0x01, 0xFF, 0x0A };
            var result = filledArray.Trace();

            result.Should().NotBeEmpty();

            var splitResult = result.Split(',');
            splitResult.Length.Should().Be(filledArray.Length);
        }

        [Test]
        public void CoreHelper_Extension_Trace_WithEmptyArray_Works()
        {
            var emptyArray = new byte[0];
            var result = emptyArray.Trace();

            result.Should().BeEmpty();
        }

        // Generic convert object to Enum

        [Test]
        public void CoreHelpers_11_01_ConvertObjectToEnum_NullParameter_NullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => CoreHelpers.Convert<StringId>(null));
        }
    }
}
