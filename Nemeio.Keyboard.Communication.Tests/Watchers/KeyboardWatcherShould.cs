using FluentAssertions;
using Moq;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Utils;
using Nemeio.Keyboard.Communication.Watchers;
using NUnit.Framework;

namespace Nemeio.Keyboard.Communication.Tests.Watchers
{
    [TestFixture]
    public class KeyboardWatcherShould
    {
        private class TestableKeyboardWatcher : KeyboardWatcher
        {
            public TestableKeyboardWatcher(IKeyboardVersionParser versionParser) 
                : base(versionParser) { }

            public void TestableAddKeyboard(Core.Keyboard.Keyboard keyboard) => AddKeyboard(keyboard);
            public void TestableRemoveKeyboard(Core.Keyboard.Keyboard keyboard) => RemoveKeyboard(keyboard);

            public override void Dispose() { }
        }

        private TestableKeyboardWatcher _watcher;

        [SetUp]
        public void SetUp()
        {
            var mockKeyboardVersionParser = Mock.Of<IKeyboardVersionParser>();
            Mock.Get(mockKeyboardVersionParser)
                .Setup(x => x.Parse(It.IsAny<string>()))
                .Returns(new System.Version("2.0"));

            _watcher = new TestableKeyboardWatcher(mockKeyboardVersionParser);
        }

        [Test]
        public void KeyboardWatcher_Constructor_Ok()
        {
            _watcher.Keyboards.Should().NotBeNull();
            _watcher.Keyboards.Should().BeEmpty();
        }

        [Test]
        public void KeyboardWatcher_AddKeyboard_WhenParameterIsNull_DoNothing()
        {
            _watcher.TestableAddKeyboard(null);

            _watcher.Keyboards.Should().NotBeNull();
            _watcher.Keyboards.Should().BeEmpty();
        }

        [Test]
        public void KeyboardWatcher_AddKeyboard_WhenKeyboardIsAlreadyKnown_DoNothing()
        {
            var onKeyboardConnectedCountCalled = 0;

            var keyboardIO = Mock.Of<IKeyboardIO>();
            var keyboard = new Core.Keyboard.Keyboard("myFakeId", new System.Version("2.0"), CommunicationType.Serial, keyboardIO);

            _watcher.OnKeyboardConnected += delegate
            {
                onKeyboardConnectedCountCalled += 1;
            };

            _watcher.TestableAddKeyboard(keyboard);

            _watcher.Keyboards.Should().NotBeNull();
            _watcher.Keyboards.Should().NotBeEmpty();
            _watcher.Keyboards.Count.Should().Be(1);
            onKeyboardConnectedCountCalled.Should().Be(1);

            _watcher.TestableAddKeyboard(keyboard);

            _watcher.Keyboards.Should().NotBeNull();
            _watcher.Keyboards.Should().NotBeEmpty();
            _watcher.Keyboards.Count.Should().Be(1);
            onKeyboardConnectedCountCalled.Should().Be(1);
        }

        [Test]
        public void KeyboardWatcher_AddKeyboard_Ok()
        {
            var onKeyboardConnectedCountCalled = 0;

            var keyboardIO = Mock.Of<IKeyboardIO>();
            var keyboard = new Core.Keyboard.Keyboard("myFakeId", new System.Version("2.0"), CommunicationType.Serial, keyboardIO);

            _watcher.OnKeyboardConnected += delegate
            {
                onKeyboardConnectedCountCalled += 1;
            };

            _watcher.TestableAddKeyboard(keyboard);

            _watcher.Keyboards.Should().NotBeNull();
            _watcher.Keyboards.Should().NotBeEmpty();
            _watcher.Keyboards.Count.Should().Be(1);
            onKeyboardConnectedCountCalled.Should().Be(1);
        }

        [Test]
        public void KeyboardWatcher_RemoveKeyboard_WhenParameterIsNull_DoNothing()
        {
            var keyboardIO = Mock.Of<IKeyboardIO>();
            var keyboard = new Core.Keyboard.Keyboard("myFakeId", new System.Version("2.0"), CommunicationType.Serial, keyboardIO);

            _watcher.TestableAddKeyboard(keyboard);

            _watcher.Keyboards.Should().NotBeNull();
            _watcher.Keyboards.Should().NotBeEmpty();

            _watcher.TestableRemoveKeyboard(null);

            _watcher.Keyboards.Should().NotBeNull();
            _watcher.Keyboards.Should().NotBeEmpty();
            _watcher.Keyboards.Count.Should().Be(1);
        }

        [Test]
        public void KeyboardWatcher_RemoveKeyboard_WhenKeyboardIsNotKnown_DoNothing()
        {
            var onKeyboardDisconnectedCountCalled = 0;

            var keyboardIO = Mock.Of<IKeyboardIO>();
            var keyboard = new Core.Keyboard.Keyboard("myFakeId", new System.Version("2.0"), CommunicationType.Serial, keyboardIO);
            var mySecondKeyboard = new Core.Keyboard.Keyboard("myOtherFakeId", new System.Version("2.0"), CommunicationType.Serial, keyboardIO);

            _watcher.OnKeyboardDisconnected += delegate
            {
                onKeyboardDisconnectedCountCalled += 1;
            };

            _watcher.TestableAddKeyboard(keyboard);

            _watcher.Keyboards.Should().NotBeNull();
            _watcher.Keyboards.Should().NotBeEmpty();

            _watcher.TestableRemoveKeyboard(mySecondKeyboard);

            _watcher.Keyboards.Should().NotBeNull();
            _watcher.Keyboards.Should().NotBeEmpty();
            _watcher.Keyboards.Count.Should().Be(1);
            onKeyboardDisconnectedCountCalled.Should().Be(0);
        }

        [Test]
        public void KeyboardWatcher_RemoveKeyboard_Ok()
        {
            var onKeyboardDisconnectedCountCalled = 0;

            var keyboardIO = Mock.Of<IKeyboardIO>();
            var keyboard = new Core.Keyboard.Keyboard("myFakeId", new System.Version("2.0"), CommunicationType.Serial, keyboardIO);

            _watcher.OnKeyboardDisconnected += delegate
            {
                onKeyboardDisconnectedCountCalled += 1;
            };

            _watcher.TestableAddKeyboard(keyboard);

            _watcher.Keyboards.Should().NotBeNull();
            _watcher.Keyboards.Should().NotBeEmpty();

            _watcher.TestableRemoveKeyboard(keyboard);

            _watcher.Keyboards.Should().NotBeNull();
            _watcher.Keyboards.Should().BeEmpty();
            onKeyboardDisconnectedCountCalled.Should().Be(1);
        }
    }
}
