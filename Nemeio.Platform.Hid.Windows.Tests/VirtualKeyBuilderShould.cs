using System;
using System.Collections.Generic;
using FluentAssertions;
using Nemeio.Core;
using Nemeio.Platform.Hid.Windows.Keyboards;
using Nemeio.Windows.Win32;
using NUnit.Framework;

namespace Nemeio.Platform.Hid.Windows.Tests
{
    [TestFixture]
    public class VirtualKeyBuilderShould
    {
        [Test]
        public void VirtualKeyBuilder_01_01_Constructor_InitVariables()
        {
            var virtualKeyBuilder = new TestableVirtuaKeyBuilder();

            virtualKeyBuilder.KeyboardLayout.Should().Be(IntPtr.Zero);
            virtualKeyBuilder.InputUpList.Should().NotBeNull();
            virtualKeyBuilder.InputUpList.Should().BeEmpty();
            virtualKeyBuilder.InputDownList.Should().NotBeNull();
            virtualKeyBuilder.InputDownList.Should().BeEmpty();
        }

        [Test]
        public void VirtualKeyBuilder_02_01_Reset_CleanVariables()
        {
            var azertyKeyboard = new IntPtr(0x00000000040c040c);

            var virtualKeyBuilder = new TestableVirtuaKeyBuilder();
            virtualKeyBuilder.Reset(azertyKeyboard);

            virtualKeyBuilder.KeyboardLayout.Should().Be(azertyKeyboard);
            virtualKeyBuilder.InputUpList.Should().NotBeNull();
            virtualKeyBuilder.InputUpList.Should().BeEmpty();
            virtualKeyBuilder.InputDownList.Should().NotBeNull();
            virtualKeyBuilder.InputDownList.Should().BeEmpty();
        }

        [TestCase(KeyboardLiterals.WindowsKey, false)]
        [TestCase("a", true)]
        [TestCase("^", false)]
        public void VirtualKeyBuilder_03_01_AddKey_CreateNewInput(string key, bool pressed)
        {
            var virtualKeyBuilder = new TestableVirtuaKeyBuilder();

            virtualKeyBuilder.AddKey(key, pressed);

            var result = virtualKeyBuilder.Build();
            result.Count.Should().Be(1);
        }

        class TestableVirtuaKeyBuilder : VirtualKeyBuilder
        {
            public IntPtr KeyboardLayout => _keyboardLayout;

            public List<WinUser32.INPUT> InputDownList => _inputDownList;

            public List<WinUser32.INPUT> InputUpList => _inputUpList;
        }
    }
}
