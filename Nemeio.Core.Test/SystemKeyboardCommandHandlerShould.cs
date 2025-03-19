using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Nemeio.Core.Models.SystemKeyboardCommand;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    [TestFixture]
    public class SystemKeyboardCommandHandlerShould
    {
        private SystemKeyboardCommandHandler _systemKeyboardCommandHandler;

        [SetUp]
        public void SetUp()
        {
            _systemKeyboardCommandHandler = new SystemKeyboardCommandHandler();
        }

        [Test]
        public void SystemKeyboardCommandHandler_01_01_RegisterCommand_AddNullCommand_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _systemKeyboardCommandHandler.RegisterCommand(null));
        }

        [Test]
        public void SystemKeyboardCommandHandler_01_02_RegisterCommand_AddTwiceCommand_ThrowInvalidOperationException()
        {
            var mockCommand = Mock.Of<SystemKeyboardCommand>();

            Assert.Throws<InvalidOperationException>(() => 
            {
                _systemKeyboardCommandHandler.RegisterCommand(mockCommand);
                _systemKeyboardCommandHandler.RegisterCommand(mockCommand);
            });
        }

        [Test]
        public void SystemKeyboardCommandHandler_01_03_RegisterCommand_AddCommand_WorksOk()
        {
            var mockCommand = Mock.Of<SystemKeyboardCommand>();

            Assert.DoesNotThrow(() => _systemKeyboardCommandHandler.RegisterCommand(mockCommand));
        }

        [Test]
        public void SystemKeyboardCommandHandler_02_01_Handle_NullListParameter_ReturnFalse()
        {
            var handled = _systemKeyboardCommandHandler.Handle(null);

            handled.Should().BeFalse();
        }

        [Test]
        public void SystemKeyboardCommandHandler_02_02_Handle_EmptyListParameter_ReturnFalse()
        {
            var emptyList = new List<string>();
            var handled = _systemKeyboardCommandHandler.Handle(emptyList);

            handled.Should().BeFalse();
        }

        [Test]
        public void SystemKeyboardCommandHandler_02_03_Handle_NotMatchingCommand_ReturnFalse()
        {
            var keys = new List<string>() { KeyboardLiterals.Tab };
            var simpleCommand = new TestableKeyboardCommand(KeyboardLiterals.WindowsKey);

            _systemKeyboardCommandHandler.RegisterCommand(simpleCommand);

            var handled = _systemKeyboardCommandHandler.Handle(keys);

            handled.Should().BeFalse();
            simpleCommand.ExecuteCalled.Should().BeFalse();
        }

        [Test]
        public void SystemKeyboardCommandHandler_02_04_Handle_MatchingCommand_ReturnTrue()
        {
            var waitedKey = KeyboardLiterals.WindowsKey;
            var keys = new List<string>() { waitedKey };
            var simpleCommand = new TestableKeyboardCommand(waitedKey);

            _systemKeyboardCommandHandler.RegisterCommand(simpleCommand);

            var handled = _systemKeyboardCommandHandler.Handle(keys);

            handled.Should().BeTrue();
        }

        [Test]
        public void SystemKeyboardCommandHandler_02_05_Handle_MatchingCommand_CallExecuteMethod()
        {
            var waitedKey = KeyboardLiterals.WindowsKey;
            var keys = new List<string>() { waitedKey };
            var simpleCommand = new TestableKeyboardCommand(waitedKey);

            _systemKeyboardCommandHandler.RegisterCommand(simpleCommand);
            _systemKeyboardCommandHandler.Handle(keys);

            simpleCommand.ExecuteCalled.Should().BeTrue();
        }

        private class TestableKeyboardCommand : SystemKeyboardCommand
        {
            private string _keyToFind;

            public bool ExecuteCalled { get; private set; }

            public TestableKeyboardCommand(string key)
            {
                _keyToFind = key;
            }

            public override bool IsTriggered(IList<string> keys)
            {
                return keys.Contains(_keyToFind);
            }

            public override void Execute()
            {
                ExecuteCalled = true;
            }
        }
    }
}
