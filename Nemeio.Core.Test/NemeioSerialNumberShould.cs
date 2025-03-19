using System;
using FluentAssertions;
using Nemeio.Core.DataModels;
using Nemeio.Core.Keyboard.SerialNumber;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    [TestFixture]
    public class NemeioSerialNumberShould
    {
        [Test]
        public void ThrowException_WhenTooSmallArray()
        {
            var badSerialNumber = new byte[] {0x01, 0x04, 0x58};

            Assert.Throws<ArgumentOutOfRangeException>(() => new NemeioSerialNumber(badSerialNumber));
        }

        [Test]
        public void ThrowException_WhenTooLargeArray()
        {
            var badSerialNumber = new byte[] { 0x01, 0x04, 0x58, 0x58, 0x09, 0xFE, 0x58, 0x32, 0x58, 0x12, 0x79, 0x58, 0x78 };

            Assert.Throws<ArgumentOutOfRangeException>(() => new NemeioSerialNumber(badSerialNumber));
        }

        [Test]
        [TestCase(new byte[] { 0x25, 0x00, 0x2F, 0x00, 0x0A, 0x50, 0x43, 0x52, 0x54, 0x38, 0x36, 0x20 })]
        [TestCase(new byte[] { 0x00, 0x38, 0x2F, 0x52, 0x0A, 0x00, 0x75, 0x54, 0x54, 0x43, 0x20, 0x38 })]
        public void CreateObject_WhenPassValidArray(byte[] serialNumber) => new NemeioSerialNumber(serialNumber);

        [Test]
        public void NemeioSerialNumber_Equals_Ok()
        {
            var serialNumberA = new NemeioSerialNumber(new byte[] { 0x25, 0x00, 0x2F, 0x00, 0x0A, 0x50, 0x43, 0x52, 0x54, 0x38, 0x36, 0x20 });
            var serialNumberB = new NemeioSerialNumber(new byte[] { 0x25, 0x00, 0x2F, 0x00, 0x0A, 0x50, 0x43, 0x52, 0x54, 0x38, 0x36, 0x20 });
            var serialNumberC = new NemeioSerialNumber(new byte[] { 0x00, 0x38, 0x2F, 0x52, 0x0A, 0x00, 0x75, 0x54, 0x54, 0x43, 0x20, 0x38 });

            serialNumberA.Equals(serialNumberB).Should().BeTrue();
            serialNumberA.Equals(serialNumberC).Should().BeFalse();
        }
    }
}
