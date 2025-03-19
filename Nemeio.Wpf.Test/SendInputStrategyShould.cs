using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Errors;
using Nemeio.Wpf.Helpers;
using Nemeio.Wpf.Models;
using NUnit.Framework;

namespace Nemeio.Wpf.Test
{
    //FIXME
    /*[TestFixture]
    public class SendInputStrategyShould
    {
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_KEYDOWN = 0;
        private const uint KEYEVENTF_UNICODE = 0x0004;
        private const uint KEYEVENTF_SCANCODE = 0x0008;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        private IntPtr _currentLayout = IntPtr.Zero;

        private static readonly List<string> EXTENDED_KEYS = new List<string>()
        {
            "Del", "Os", "Up", "Down", "Left", "Right", "VolUp", "VolDown", "VolMute"
        };

        private static readonly List<string> KEYS_SAMPLE = new List<string>()
        {
            "a", "z", "e", "p", "c", "#", "é", "1", "/", "*", "<", "ù"
        };

        [SetUp]
        public void SetUp()
        {
            var watcher = new WinSystemActiveLayoutWatcher(new LoggerFactory(), new ErrorManager());
            var layoutId = watcher.GetCurrentSystemLayoutId() as WinOsLayoutId;
            _currentLayout = layoutId.BaseLayoutHandle;
        }

        [Test]
        [TestCaseSource(nameof(KEYS_SAMPLE))]
        public void SendScancode(string key) => Assert.IsTrue(KeyHasFlag(key, KEYEVENTF_SCANCODE));

        [Test]
        public void SendUnicode()
        {
            var sendInput = new FakeSendInputStrategy();
            sendInput.Init();

            List<WinUser32.INPUT> exceptedResult = new List<WinUser32.INPUT>()
            {
                WinUser32.NewUnicodeDownInput('漢'),
                WinUser32.NewDownInput(sendInput.GetVkeyForChar('p'), _currentLayout),
                WinUser32.NewUnicodeDownInput('字'),
                WinUser32.NewDownInput(sendInput.GetVkeyForChar('a'), _currentLayout)
            };

            List<KeyboardKey> input = new List<KeyboardKey>()
            {
                new KeyboardKey("漢", true),
                new KeyboardKey("p", true),
                new KeyboardKey("字", true),
                new KeyboardKey("a", true),
            };

            sendInput.SendKeyboardInput(input, _currentLayout);

            sendInput.Inputs.Should().Equal(exceptedResult);
        }

        [Test]
        [TestCaseSource(nameof(EXTENDED_KEYS))]
        public void SendExtendedKeys(string key) => Assert.IsTrue(KeyHasFlag(key, KEYEVENTF_EXTENDEDKEY));

        private bool KeyHasFlag(string key, uint flag)
        {
            var sendInput = new FakeSendInputStrategy();
            sendInput.Init();

            List<WinUser32.INPUT> exceptedResult = new List<WinUser32.INPUT>()
            {
                VirtualKeyBuilder.CreateInput(key, false, _currentLayout)
            };

            List<KeyboardKey> actual = new List<KeyboardKey>()
            {
                new KeyboardKey(key, true)
            };

            sendInput.SendKeyboardInput(actual, _currentLayout);

            return (exceptedResult[0].union.keyboardInput.flags & flag) != 0;
        }

        class FakeSendInputStrategy : SendInputStrategy
        {
            public FakeSendInputStrategy() : base(new LoggerFactory()) { }

            public IList<WinUser32.INPUT> Inputs { get; private set; }

            public override void SendInputToWindows(IList<WinUser32.INPUT> inputsList)
            {
                Inputs = inputsList;
            }
        }
    }*/
}
