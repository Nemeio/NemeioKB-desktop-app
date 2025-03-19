using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Keyboard.Communication;
using NUnit.Framework;

namespace Nemeio.Core.Test.Keyboards.Communication
{
    [TestFixture]
    public class KeyboardSelectorShould
    {
        [Test]
        public void NemeioSelector_SelectKeyboard_WithSerialAndBluetooth_ChooseSerial()
        {
            var loggerFactory = new LoggerFactory();
            var selector = new KeyboardSelector(loggerFactory);

            var serialKeyboard = new Keyboard.Keyboard("serialK", new System.Version("2.0"), CommunicationType.Serial, Mock.Of<IKeyboardIO>());
            var bluetoothLEKeyboard = new Keyboard.Keyboard("bluetoothK", new System.Version("2.0"), CommunicationType.BluetoothLE, Mock.Of<IKeyboardIO>());

            var selectedKeyboard = selector.SelectKeyboard(new List<Keyboard.Keyboard>() { serialKeyboard, bluetoothLEKeyboard });

            selectedKeyboard.Should().NotBeNull();
            selectedKeyboard.Should().Be(serialKeyboard);
        }

        [Test]
        public void NemeioSelector_SelectKeyboard_WithMultipleSerial_ChooseFirst()
        {
            var loggerFactory = new LoggerFactory();
            var selector = new KeyboardSelector(loggerFactory);

            var serialFirstKeyboard = new Keyboard.Keyboard("serialK", new System.Version("2.0"), CommunicationType.Serial, Mock.Of<IKeyboardIO>());
            var seriaSecondKeyboard = new Keyboard.Keyboard("serialK2", new System.Version("2.0"), CommunicationType.Serial, Mock.Of<IKeyboardIO>());

            var selectedKeyboard = selector.SelectKeyboard(new List<Keyboard.Keyboard>() { serialFirstKeyboard, seriaSecondKeyboard });

            selectedKeyboard.Should().NotBeNull();
            selectedKeyboard.Should().Be(serialFirstKeyboard);
        }

        [Test]
        public void NemeioSelector_SelectKeyboard_WithOnlyBluetooth_ChooseBluetooth()
        {
            var loggerFactory = new LoggerFactory();
            var selector = new KeyboardSelector(loggerFactory);

            var bluetoothLEKeyboard = new Keyboard.Keyboard("bluetoothK", new System.Version("2.0"), CommunicationType.BluetoothLE, Mock.Of<IKeyboardIO>());

            var selectedKeyboard = selector.SelectKeyboard(new List<Keyboard.Keyboard>() { bluetoothLEKeyboard });

            selectedKeyboard.Should().NotBeNull();
            selectedKeyboard.Should().Be(bluetoothLEKeyboard);
        }

        [Test]
        public void NemeioSelector_SelectKeyboard_WithMultipleAndOnlyBluetooth_ChooseFirst()
        {
            var loggerFactory = new LoggerFactory();
            var selector = new KeyboardSelector(loggerFactory);

            var bluetoothFirstKeyboard = new Keyboard.Keyboard("serialK", new System.Version("2.0"), CommunicationType.BluetoothLE, Mock.Of<IKeyboardIO>());
            var bluetoothSecondKeyboard = new Keyboard.Keyboard("serialK2", new System.Version("2.0"), CommunicationType.BluetoothLE, Mock.Of<IKeyboardIO>());

            var selectedKeyboard = selector.SelectKeyboard(new List<Keyboard.Keyboard>() { bluetoothFirstKeyboard, bluetoothSecondKeyboard });

            selectedKeyboard.Should().NotBeNull();
            selectedKeyboard.Should().Be(bluetoothFirstKeyboard);
        }

        [Test]
        public void NemeioSelector_SelectKeyboard_WithNoKeyboard_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var selector = new KeyboardSelector(loggerFactory);

            var selectedKeyboard = selector.SelectKeyboard(new List<Keyboard.Keyboard>());

            selectedKeyboard.Should().BeNull();
        }
    }
}
